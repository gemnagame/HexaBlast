using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance = null;
    
    //UI
    public Text m_clearConditionText;   //게임 클리어 조건 텍스트(예:팽이 12개 제거)
    public Text m_moveLimitCountText;   //이동 횟수 제한 텍스트
    public ResultPopup m_resultPopup;   //게임 결과 팝업(게임 클리어/게임 오버)

    //object pool
    public GameObject m_frameOrigin;    //프레임 생성할 때 기준이 되는 GameObject (프레임 : 블럭 뒤의 육각틀)
    public GameObject m_blockOrigin;    //블럭 생성할 때 기준이 되는 GameObject
    public Transform m_frameAreaTrans;  //프레임 생성할 위치
    public Transform m_blockAreaTrans;  //블럭 생성할 위치

    List<List<Frame>> m_allFrameList = new List<List<Frame>>(); //생성한 프레임 목록
    List<Block> m_allBlockList = new List<Block>();             //생성한 블럭 목록

    //ready
    bool m_isGameReady = false;//게임이 준비된 상태인지 여부

    //swap
    bool m_isSwapping = false;//두 블럭이 스왑 중인지 여부
    Frame m_frameStart = null;
    int m_moveCompleteCheckCount = 0;

    //matching
    int m_matchingStraightCount = 0;                        //3매치 게임이라 2(기준 블럭 포함+1)이상부터 매칭 처리
    List<Frame> m_tempMatchingList = new List<Frame>();    
    List<Frame> m_matchingList = new List<Frame>();         //매칭 블럭 목록
    List<Frame> m_matchingNeighborList = new List<Frame>(); //매칭 블럭 주변 목록
    List<Frame> m_needToRemoveTopList = new List<Frame>();  //제거될 팽이 목록 (매칭 블럭 주변 목록에서 제거될 팽이 찾은 것)
    
    //drop
    bool m_isDropping = false;                              //블럭들이 드롭 중인지 여부
    Queue<Block> m_removedBlockQueue = new Queue<Block>();  //제거 목록(매칭 블럭, 제거될 팽이 목록) 담는 큐(담았다가 드롭시켜 재사용)

    //clear, over
    int m_removedTopCount = 0;  //제거된 팽이 수
    int m_moveCount = 0;        //이동 횟수

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Init();
        CreateObjectOnce();
        HideResultPopup();
    }

    void Init()
    {
        //ready
        m_isGameReady = false;

        //swap
        m_isSwapping = false;
        m_frameStart = null;
        m_moveCompleteCheckCount = 0;

        //matching
        m_matchingStraightCount = 0;
        m_tempMatchingList.Clear();
        m_matchingList.Clear();
        m_matchingNeighborList.Clear();
        m_needToRemoveTopList.Clear();

        //drop
        m_isDropping = false;
        m_removedBlockQueue.Clear();

        //clear, over
        m_removedTopCount = 0;
        m_moveCount = 0;
    }

    void CreateObjectOnce()
    {
        //1회만 생성, 이후 게임 재시작시 생성된 목록 재사용

        if (m_allBlockList.Count > 0 || m_allFrameList.Count > 0)
        {
            return;
        }

        if (m_frameOrigin == null || m_blockOrigin == null || m_frameAreaTrans == null || m_blockAreaTrans == null)
        {
            return;
        }

        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            m_allFrameList.Add(new List<Frame>());

            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                GameObject obj = Instantiate(m_frameOrigin, m_frameAreaTrans);
                if (obj)
                {
                    Frame frame = obj.GetComponent<Frame>();
                    m_allFrameList[i].Add(frame);
                }

                obj = Instantiate(m_blockOrigin, m_blockAreaTrans);
                if (obj)
                {
                    Block block = obj.GetComponent<Block>();
                    m_allBlockList.Add(block);
                }
            }
        }
    }

    void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        Init();
        SetMapDesign();
        SetRemovedTopCountText();
        SetMoveLimitCountText();
    }

    void SetMapDesign()
    {
        //기존에 디자인해둔 맵에 맞춰 프레임, 블럭 설정(위치, 타입)
        //todo 현재는 테스트 단계라 소스코드(Const.MAPDESIGN[])에 들어가 있는데 파일로 따로 빼야함

        int count = 0;
        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                Vector3 position = Util.CalcPositionByIndex(i, j);

                int indexX = Const.MAPSIZE_Y - 1 - j;
                BlockType blockType = Const.MAPDESIGN[indexX, i];                

                m_allFrameList[i][j].Init(blockType != BlockType.NONE, new Index(i, j));
                m_allFrameList[i][j].SetPosition(position);
                m_allFrameList[i][j].SetBlock(m_allBlockList[count]);

                m_allBlockList[count].Init(blockType);
                m_allBlockList[count].SetPosition(position);

                count++;
            }
        }

        m_isGameReady = true;
    }

    void OnDestroy()
    {
        //생성한 프레임 목록, 블럭 목록 제거

        for (int i = 0; i < m_allFrameList.Count; ++i)
        {        
            for (int j = 0; j < m_allFrameList[i].Count; ++j)
            {
                if (m_allFrameList[i][j])
                {
                    Destroy(m_allFrameList[i][j].gameObject);
                }
            }
        }

        for (int i = 0; i < m_allBlockList.Count; ++i)
        {
            if (m_allBlockList[i])
            {
                Destroy(m_allBlockList[i].gameObject);
            }
        }
    }

    public void FramePointerDown(Frame frameStart)
    {
        if (TouchBlocked())
        {
            return;
        }

        m_frameStart = frameStart;
    }

    public void FramePointerUp()
    {
        if (TouchBlocked())
        {
            return;
        }

        m_frameStart = null;
    }

    public void FramePointerEnter(Frame frameEnd)
    {
        if (TouchBlocked() || 
            m_frameStart == null || frameEnd == null)
        {
            return;
        }

        if (m_frameStart.GetBlockType() == BlockType.NONE || 
            frameEnd.GetBlockType() == BlockType.NONE)
        {
            return;
        }

        if (m_frameStart != frameEnd)
        {
            //스왑
            SwapAndCheckMatching(m_frameStart, frameEnd);

            m_frameStart = null;
        }
    }

    void SwapAndCheckMatching(Frame frame1, Frame frame2)
    {
        //스왑 후 매칭이 있는지 체크하고 EndSwapAction 호출

        m_isSwapping = true;
        m_moveCompleteCheckCount = 0;

        bool isMatching = false;        

        Block block1 = frame1.GetBlock();
        frame1.SetEmpty();
        Block block2 = frame2.GetBlock();
        frame2.SetEmpty();
        frame1.SetBlock(block2);
        frame2.SetBlock(block1);
        frame1.GetBlock().StartMove(frame1.GetPosition(),
            ()=> EndSwapAction(isMatching, frame1, frame2));
        frame2.GetBlock().StartMove(frame2.GetPosition(),
            ()=> EndSwapAction(isMatching, frame1, frame2));

        isMatching |= CheckMatching(frame1);
        isMatching |= CheckMatching(frame2);
    }

    void EndSwapAction(bool isMatching, Frame frame1, Frame frame2)
    {
        m_moveCompleteCheckCount++;
        if (m_moveCompleteCheckCount == 2)
        {
            m_moveCompleteCheckCount = 0;

            //매칭이 있으면
            if (isMatching)
            {
                AfterCheckMatching();

                m_moveCount++;
                SetMoveLimitCountText();

                m_isSwapping = false;
            }
            else//매칭이 없으면 제자리로
            {
                SwapBack(frame1, frame2);
            }
        }
    }

    void SwapBack(Frame frame1, Frame frame2)
    {
        m_moveCompleteCheckCount = 0;

        Block block1 = frame1.GetBlock();
        frame1.SetEmpty();
        Block block2 = frame2.GetBlock();
        frame2.SetEmpty();
        frame1.SetBlock(block2);
        frame2.SetBlock(block1);
        frame1.GetBlock().StartMove(frame1.GetPosition(),
            ()=> EndSwapBackAction());
        frame2.GetBlock().StartMove(frame2.GetPosition(),
            ()=> EndSwapBackAction());
    }

    void EndSwapBackAction()
    {
        m_moveCompleteCheckCount++;
        if (m_moveCompleteCheckCount == 2)
        {
            m_moveCompleteCheckCount = 0;

            m_isSwapping = false;
        }
    }

    bool CheckMatching(Frame frame)
    {
        bool isMatching = false;

        BlockType checkBlockType = frame.GetBlockType();
        Index index = frame.GetIndex();

        //1. Square

        Index index_leftUp              = Util.CalcIndex(index, Direction.LEFTUP);
        Index index_leftUp_up           = Util.CalcIndex(index_leftUp, Direction.UP);
        Index index_up                  = Util.CalcIndex(index, Direction.UP);
        Index index_rightUp             = Util.CalcIndex(index, Direction.RIGHTUP);
        Index index_rightUp_up          = Util.CalcIndex(index_rightUp, Direction.UP);
        Index index_rightUp_rightDown   = Util.CalcIndex(index_rightUp, Direction.RIGHTDOWN);
        Index index_rightDown           = Util.CalcIndex(index, Direction.RIGHTDOWN);
        Index index_rightDown_down      = Util.CalcIndex(index_rightDown, Direction.DOWN);
        Index index_down                = Util.CalcIndex(index, Direction.DOWN);
        Index index_leftDown            = Util.CalcIndex(index, Direction.LEFTDOWN);
        Index index_leftDown_down       = Util.CalcIndex(index_leftDown, Direction.DOWN);
        Index index_leftDown_leftUp     = Util.CalcIndex(index_leftDown, Direction.LEFTUP);

        //LeftUp, Up
        //+ LeftUp_Up
        isMatching |= CheckMatchingSquare(checkBlockType, index_leftUp, index_up, index_leftUp_up);
        //+ RightUp
        isMatching |= CheckMatchingSquare(checkBlockType, index_leftUp, index_up, index_rightUp);

        //Up, RightUp
        //+ RightUp_Up
        isMatching |= CheckMatchingSquare(checkBlockType, index_up, index_rightUp, index_rightUp_up);
        //+ RightDown
        isMatching |= CheckMatchingSquare(checkBlockType, index_up, index_rightUp, index_rightDown);

        //RightUp, RightDown
        //+ RightUp_RightDown
        isMatching |= CheckMatchingSquare(checkBlockType, index_rightUp, index_rightDown, index_rightUp_rightDown);
        //+ Down
        isMatching |= CheckMatchingSquare(checkBlockType, index_rightUp, index_rightDown, index_down);

        //RightDown, Down
        //+ RightDown_Down
        isMatching |= CheckMatchingSquare(checkBlockType, index_rightDown, index_down, index_rightDown_down);
        //+ LeftDown
        isMatching |= CheckMatchingSquare(checkBlockType, index_rightDown, index_down, index_leftDown);

        //Down, LeftDown
        //+ LeftDown_Down
        isMatching |= CheckMatchingSquare(checkBlockType, index_down, index_leftDown, index_leftDown_down);
        //+ LeftUp
        isMatching |= CheckMatchingSquare(checkBlockType, index_down, index_leftDown, index_leftUp);

        //LeftDown, LeftUp
        //+ LeftDown_LeftUp
        isMatching |= CheckMatchingSquare(checkBlockType, index_leftDown, index_leftUp, index_leftDown_leftUp);
        //+ Up
        isMatching |= CheckMatchingSquare(checkBlockType, index_leftDown, index_leftUp, index_up);


        //2. Straight

        //Up + Down
        isMatching |= CheckMatchingStraight(checkBlockType, index, Direction.UP, Direction.DOWN);

        //LeftUp + RightDown
        isMatching |= CheckMatchingStraight(checkBlockType, index, Direction.LEFTUP, Direction.RIGHTDOWN);

        //LeftDown + RightUp
        isMatching |= CheckMatchingStraight(checkBlockType, index, Direction.LEFTDOWN, Direction.RIGHTUP);

        if (isMatching)
        {
            //기준이 되는 프레임도 추가
            AddMatchingList(frame);  
        }

        return isMatching;
    }

    bool CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction1, Direction direction2)
    {
        //직선 매칭 체크

        m_matchingStraightCount = 0;
        m_tempMatchingList.Clear();
        CheckMatchingStraight(checkBlockType, index, direction1);
        CheckMatchingStraight(checkBlockType, index, direction2);
        if (m_matchingStraightCount >= 2)
        {
            AddMatchingList(m_tempMatchingList);
            return true;
        }

        return false;
    }

    void CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction)
    {
        //입력받은 방향따라 재귀로 반복 체크

        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.TOP)
        {
            return;
        }

        Index calcIndex = Util.CalcIndex(index, direction);
        if (Util.IsOutOfIndex(calcIndex, Const.MAPSIZE_X, Const.MAPSIZE_Y))
        {
            return;
        }

        BlockType blockType = m_allFrameList[calcIndex.X][calcIndex.Y].GetBlockType();
        if (blockType == checkBlockType)
        {
            m_matchingStraightCount++;
            m_tempMatchingList.Add(m_allFrameList[calcIndex.X][calcIndex.Y]);

            CheckMatchingStraight(checkBlockType, calcIndex, direction);
        }
    }

    bool CheckMatchingSquare(BlockType checkBlockType, Index index1, Index index2, Index index3)
    {
        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.TOP)
        {
            return false;
        }

        if (Util.IsOutOfIndex(index1, Const.MAPSIZE_X, Const.MAPSIZE_Y) ||
            Util.IsOutOfIndex(index2, Const.MAPSIZE_X, Const.MAPSIZE_Y) ||
            Util.IsOutOfIndex(index3, Const.MAPSIZE_X, Const.MAPSIZE_Y))
        {
            return false;
        }

        BlockType blockType1 = m_allFrameList[index1.X][index1.Y].GetBlockType();
        BlockType blockType2 = m_allFrameList[index2.X][index2.Y].GetBlockType();
        BlockType blockType3 = m_allFrameList[index3.X][index3.Y].GetBlockType();
        if (blockType1 == checkBlockType &&
            blockType2 == checkBlockType &&
            blockType3 == checkBlockType)
        {
            m_tempMatchingList.Clear();
            m_tempMatchingList.Add(m_allFrameList[index1.X][index1.Y]);
            m_tempMatchingList.Add(m_allFrameList[index2.X][index2.Y]);
            m_tempMatchingList.Add(m_allFrameList[index3.X][index3.Y]);
            AddMatchingList(m_tempMatchingList);

            return true;
        }

        return false;
    }

    void AddMatchingList(List<Frame> frameList)
    {
        for(int i = 0; i < frameList.Count; ++i)
        {
            AddMatchingList(frameList[i]);
        }
    }

    void AddMatchingList(Frame frame)
    {
        if (m_matchingList.Contains(frame) == false)
        {
            m_matchingList.Add(frame);
        }
    }

    void RemoveBlockList()
    {
        //매칭 블럭, 제거될 팽이 목록 제거
        //todo 함수로 따로 빼야할듯? 코드 중복임

        Vector3 position = Util.CalcPositionByIndex(Const.ENTRANCE_UP_INDEX);                

        for (int i = 0; i < m_matchingList.Count; ++i)
        {
            Block block = m_matchingList[i].GetBlock();
            m_removedBlockQueue.Enqueue(block);
            block.SetPosition(position);
            block.Init(Util.GetRandomBlockType());
            m_matchingList[i].SetEmpty();
        }

        m_matchingList.Clear();


        for (int i = 0; i < m_needToRemoveTopList.Count; ++i)
        {
            Block block = m_needToRemoveTopList[i].GetBlock();
            m_removedBlockQueue.Enqueue(block);
            block.SetPosition(position);
            block.Init(Util.GetRandomBlockType());
            m_needToRemoveTopList[i].SetEmpty();
        }

        m_needToRemoveTopList.Clear();
    }

    IEnumerator CO_DropAllBlock()
    {
        m_isDropping = true;

        for (int j = 0; j < Const.MAPSIZE_Y; ++j)
        {
            //i가 홀수일때 먼저(홀수일때 위치가 더 낮음)
            for (int i = 1; i < Const.MAPSIZE_X; i += 2)
            {
                var frame = m_allFrameList[i][j];
                DropBlock(frame, true, false, false);
            }

            for (int i = 0; i < Const.MAPSIZE_X; i += 2)
            {
                var frame = m_allFrameList[i][j];
                DropBlock(frame, true, false, false);
            }

            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(CO_DropNewBlock());

    }

    IEnumerator CO_DropNewBlock()
    {
        //새로 추가할 블럭 없으면 드롭 종료
        if (m_removedBlockQueue.Count == 0)
        {
            m_isDropping = false;

            AllCheckMatching();

            yield break;
        }

        Frame entranceFrame = GetFrameByIndex(Const.ENTRANCE_INDEX);
        if (entranceFrame.IsEmpty() == false)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.1f);

        //새 블럭 추가(제거된 블럭의 타입을 바꿔서 재사용)
        Block newblock = m_removedBlockQueue.Dequeue();
        entranceFrame.SetBlock(newblock);
        entranceFrame.GetBlock().StartMove(entranceFrame.GetPosition(),
            () =>
            {
                DropBlock(entranceFrame);
                StartCoroutine(CO_DropNewBlock());
            });       
    }

    void DropBlock(Frame frame, bool useDown = true, bool useLeftDown = true, bool useRightDown = true)
    {
        if (frame.IsMoveable() == false)
        {
            return;
        }

        /*
             * 아래쪽 프레임 인덱스부터 돌면서 아래 셋 중 하나 해당하면 해당 위치로 이동
            1.내 아래쪽 비었음
            2.왼쪽아래 비었음 + 왼쪽위 블럭이 이동불가
            3.오른쪽아래 비었음 + 오른쪽위 블럭이 이동불가
             */

        Index index = frame.GetIndex();

        Frame frame_up = GetFrameByDir(index, Direction.UP);
        Frame frame_down = GetFrameByDir(index, Direction.DOWN);
        Frame frame_leftDown = GetFrameByDir(index, Direction.LEFTDOWN);
        Frame frame_leftUp = GetFrameByDir(index, Direction.LEFTUP);
        Frame frame_rightDown = GetFrameByDir(index, Direction.RIGHTDOWN);
        Frame frame_rightUp = GetFrameByDir(index, Direction.RIGHTUP);


        //맨위는 예외적으로 전방향
        bool isTop = !(frame_up && frame_up.IsMoveable());
        if (isTop)
        {
            useLeftDown = true;
            useRightDown = true;
        }

        //아래로 이동
        if (useDown &&
            frame_down && frame_down.IsEmpty())
        {
            MoveBlockToFrame(frame, frame_down,
                () => DropBlock(frame_down, useDown, useLeftDown, useRightDown));
        }//좌하단으로 이동
        else if (useLeftDown &&
            frame_leftDown && frame_leftDown.IsEmpty() &&
            (frame_leftUp == null || frame_leftUp.IsMoveable() == false))
        {
            MoveBlockToFrame(frame, frame_leftDown,
                () => DropBlock(frame_leftDown, useDown, useLeftDown, useRightDown));
        }//우하단으로 이동
        else if (useRightDown &&
            frame_rightDown && frame_rightDown.IsEmpty() &&
            (frame_rightUp == null || frame_rightUp.IsMoveable() == false))
        {
            MoveBlockToFrame(frame, frame_rightDown,
                () => DropBlock(frame_rightDown, useDown, useLeftDown, useRightDown));
        }
    }

    Frame GetFrameByDir(Index index, Direction dir)
    {
        //todo 기존에 calcindex로 하던거 GetFrameByDir 함수로 변경 가능하면 변경하자
        Index calcIndex = Util.CalcIndex(index, dir);
        return GetFrameByIndex(calcIndex);
    }

    Frame GetFrameByIndex(Index index)
    {
        if (Util.IsOutOfIndex(index, Const.MAPSIZE_X, Const.MAPSIZE_Y))
        {
            return null;
        }

        return m_allFrameList[index.X][index.Y];
    }

    public bool TouchBlocked()
    {
        //유저 입력 막기
        return m_isGameReady == false || m_isSwapping || m_isDropping;
    }

    void MoveBlockToFrame(Frame fromFrame, Frame toFrame, Action endMovevAction)
    {
        toFrame.SetBlock(fromFrame.GetBlock());
        fromFrame.SetEmpty();       
        toFrame.GetBlock().StartMove(toFrame.GetPosition(), endMovevAction);
    }

    void MatchingSideEffect()
    {
        //매칭 블럭 주변에 매칭 효과를 준다(팽이는 두번 받으면 제거됨)

        m_matchingNeighborList.Clear();

        for (int i = 0; i < m_matchingList.Count; ++i)
        {
            Index index = m_matchingList[i].GetIndex();

            MatchingSideEffect(GetFrameByDir(index, Direction.LEFTUP));
            MatchingSideEffect(GetFrameByDir(index, Direction.UP));
            MatchingSideEffect(GetFrameByDir(index, Direction.RIGHTUP));
            MatchingSideEffect(GetFrameByDir(index, Direction.RIGHTDOWN));
            MatchingSideEffect(GetFrameByDir(index, Direction.DOWN));
            MatchingSideEffect(GetFrameByDir(index, Direction.LEFTDOWN));
        }
    }

    void MatchingSideEffect(Frame frame)
    {
        if (frame && frame.IsEmpty() == false &&
            m_matchingNeighborList.Contains(frame) == false)
        {
            m_matchingNeighborList.Add(frame);

            Block block = frame.GetBlock();
            bool needToRemove = block.AddMatchingNeighborCount();
            if(needToRemove)
            {
                if (m_needToRemoveTopList.Contains(frame) == false)
                {
                    m_needToRemoveTopList.Add(frame);

                    m_removedTopCount++;
                    SetRemovedTopCountText();
                }
            }
        }
    }

    public void On_Restart()
    {
        if(TouchBlocked())
        {
            return;
        }

        GameStart();
    }    

    void AfterCheckMatching()
    {
        //매칭 효과
        MatchingSideEffect();

        //제거
        RemoveBlockList();
        
        //드롭
        StartCoroutine(CO_DropAllBlock());
    }

    void AllCheckMatching()
    {
        bool isMatching = false;

        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                isMatching |= CheckMatching(m_allFrameList[i][j]);
            }
        }

        if(isMatching)
        {
            AfterCheckMatching();
        }
    }

    void SetRemovedTopCountText()
    {
        if(m_clearConditionText == null)
        {
            return;
        }

        int count = Const.CLEAR_CONDITION - m_removedTopCount;
        count = count <= 0 ? 0 : count;

        m_clearConditionText.text = count.ToString();

        if (m_removedTopCount >= Const.CLEAR_CONDITION)
        {
            ShowResultPopup(GameResult.GAME_CLEAR);
        }
    }

    void SetMoveLimitCountText()
    {
        if(m_moveLimitCountText == null)
        {
            return;
        }

        int count = Const.MOVE_LIMIT_COUNT - m_moveCount;
        count = count <= 0 ? 0 : count;

        m_moveLimitCountText.text = count.ToString();

        if (m_moveCount >= Const.MOVE_LIMIT_COUNT)
        {
            ShowResultPopup(GameResult.GAME_OVER);
        }
    }

    void HideResultPopup()
    {
        if (m_resultPopup == null)
        {
            return;
        }

        m_resultPopup.Hide();
    }

    void ShowResultPopup(GameResult gameResult)
    {
        if(m_resultPopup == null)
        {
            return;
        }

        if(gameResult == GameResult.GAME_CLEAR)
        {
            m_resultPopup.Show(Const.GAME_CLEAR_TEXT);
        }
        else if(gameResult == GameResult.GAME_OVER)
        {
            m_resultPopup.Show(Const.GAME_OVER_TEXT);
        }
    }
}
