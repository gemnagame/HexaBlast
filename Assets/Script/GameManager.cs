using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //test
    [SerializeField]
    bool m_useGameOverTimer = true;

    //UI
    [SerializeField]
    IngameUI m_ingameUI = null;
    [SerializeField]
    ResultPopup m_resultPopup = null;

    //object pool
    [SerializeField]
    GameObject m_frameOrigin = null;
    [SerializeField]
    GameObject m_blockOrigin = null;
    [SerializeField]
    Transform m_frameAreaTrans = null;
    [SerializeField]
    Transform m_blockAreaTrans = null;

    List<List<Frame>> m_allFrameList = new List<List<Frame>>(); //생성한 프레임 목록
    List<Block> m_allBlockList = new List<Block>();             //생성한 블럭 목록

    //guide //todo 가이드 작업
    [SerializeField]
    GameObject[] m_matchingGuide = new GameObject[2];
    [SerializeField]
    Transform m_matchingGuideTrans = null;

    //game state
    GameState m_gameState = GameState.NONE;

    //swap
    bool m_isSwapping = false;
    Frame m_frameStart = null;
    int m_moveCompleteCheckCount = 0;

    //matching
    List<Frame> m_matchingList = new List<Frame>();             //매칭 블럭 목록
    List<Frame> m_needToRemoveGarbageList = new List<Frame>();  //제거될 쓰레기 목록 (매칭 블럭 주변 목록에서 제거될 쓰레기 찾은 것)
    
    //drop
    bool m_isDropping = false;
    Queue<Block> m_removedBlockQueue = new Queue<Block>();  //제거 목록(매칭 블럭 + 제거될 쓰레기 목록) 담는 큐(담았다가 드롭시켜 재사용)

    //score
    int m_score = 0;

    //timer
    float m_gameOverTimer = Const.GAME_OVER_CHECK_TIME;

    void Awake()
    {
        Instance = this;

        Init();
        CreateObjectOnce();
    }

    private void Update()
    {
        if(IsTouchDisabled() == false && m_useGameOverTimer && m_gameState != GameState.GAMEOVER)
        {
            SetGameOverTimer(m_gameOverTimer - Time.deltaTime);
        }
    }

    void Init()
    {
        //guide
        m_matchingGuideTrans?.gameObject.SetActive(false);

        //game state
        m_gameState = GameState.NONE;

        //swap
        m_isSwapping = false;
        m_frameStart = null;
        m_moveCompleteCheckCount = 0;

        //matching
        m_matchingList.Clear();
        m_needToRemoveGarbageList.Clear();

        //drop
        m_isDropping = false;
        m_removedBlockQueue.Clear();

        //score
        SetScore(0);

        //timer
        SetGameOverTimer(Const.GAME_OVER_CHECK_TIME);
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
                    obj.name = "Frame" + i + j;
                    Frame frame = obj.GetComponent<Frame>();
                    m_allFrameList[i].Add(frame);
                }

                obj = Instantiate(m_blockOrigin, m_blockAreaTrans);
                if (obj)
                {
                    obj.name = "Block" + i + j;
                    Block block = obj.GetComponent<Block>();
                    m_allBlockList.Add(block);
                }
            }
        }
    }

    public bool GameStart()
    {
        StopAllCoroutines();

        Init();
        SetMapDesign();

        if (m_gameState == GameState.READY)
        {
            //todo 현재는 바로 시작인데, 화면터치시 게임시작하거나 카운트다운 후 시작하거나 하도록 바꿀 예정
            m_gameState = GameState.PLAY;

            SoundManager.instance?.PlayAudio(SoundManager.AudioType.GAMESTART);
            SoundManager.instance?.Play_BGM(SoundManager.BGMType.INGAME);

            return true;
        }
                
        return false;
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

                m_allBlockList[count].Init(blockType, position);
                m_allFrameList[i][j].Init(new Index(i, j), position, m_allBlockList[count]);

                count++;
            }
        }

        m_gameState = GameState.READY;
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

    public void OnPointerDown_Frame(Frame frameStart)
    {
        if (IsTouchDisabled())
        {
            return;
        }

        m_frameStart = frameStart;
    }

    public void OnPointerUp_Frame()
    {
        if (IsTouchDisabled())
        {
            return;
        }

        m_frameStart = null;
    }

    public void OnPointerEnter_Frame(Frame frameEnd)
    {
        if (IsTouchDisabled() || 
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
        SoundManager.instance?.PlayAudio(SoundManager.AudioType.SLIDE);

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

        List<Frame> tempMatchingList = new List<Frame>();
        CheckMatchingStraight(checkBlockType, index, direction1, tempMatchingList);
        CheckMatchingStraight(checkBlockType, index, direction2, tempMatchingList);
        if (tempMatchingList.Count >= 2)
        {
            AddMatchingList(tempMatchingList);
            return true;
        }

        return false;
    }

    void CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction, List<Frame> tempMatchingList)
    {
        //입력받은 방향따라 재귀로 반복 체크

        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.GARBAGE)
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
            tempMatchingList.Add(m_allFrameList[calcIndex.X][calcIndex.Y]);

            CheckMatchingStraight(checkBlockType, calcIndex, direction, tempMatchingList);
        }
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
        //매칭 블럭, 제거될 쓰레기 목록 제거
        //todo 함수로 따로 빼야할듯? 코드 중복임

        Vector3 position = Util.CalcPositionByIndex(Const.ENTRANCE_UP_INDEX);                

        for (int i = 0; i < m_matchingList.Count; ++i)
        {
            Block block = m_matchingList[i].GetBlock();
            m_removedBlockQueue.Enqueue(block);
            block.Init(Util.GetRandomBlockType(), position);
            m_matchingList[i].SetEmpty();
        }

        m_matchingList.Clear();


        for (int i = 0; i < m_needToRemoveGarbageList.Count; ++i)
        {
            Block block = m_needToRemoveGarbageList[i].GetBlock();
            m_removedBlockQueue.Enqueue(block);
            block.Init(Util.GetRandomBlockType(), position);
            m_needToRemoveGarbageList[i].SetEmpty();
        }

        m_needToRemoveGarbageList.Clear();

        SoundManager.instance?.PlayAudio(SoundManager.AudioType.REMOVE);
        SetScore(m_score + m_removedBlockQueue.Count);
        SetGameOverTimer(Const.GAME_OVER_CHECK_TIME);
    }

    IEnumerator CO_DropAllBlock(bool useDown = true, bool useLeftDown = true, bool useRightDown = true)
    {
        m_isDropping = true;

        for (int j = 0; j < Const.MAPSIZE_Y; ++j)
        {
            //i가 홀수일때 먼저(홀수일때 위치가 더 낮음)
            for (int i = 1; i < Const.MAPSIZE_X; i += 2)
            {
                var frame = m_allFrameList[i][j];
                DropBlock(frame, useDown, useLeftDown, useRightDown);
            }

            for (int i = 0; i < Const.MAPSIZE_X; i += 2)
            {
                var frame = m_allFrameList[i][j];
                DropBlock(frame, useDown, useLeftDown, useRightDown);
            }

            yield return new WaitForSeconds(Const.BLOCK_DROP_LINE_WAIT);
        }

        StartCoroutine(CO_InputNewBlock());

    }

    IEnumerator CO_InputNewBlock()
    {
        //새로 추가할 블럭 없으면 드롭 종료
        if (m_removedBlockQueue.Count == 0)
        {
            m_isDropping = false;

            AllCheckMatching();
            //SetMatchableFrames();//todo 가이드 작업

            yield break;
        }

        Frame entranceFrame = GetFrameByIndex(Const.ENTRANCE_INDEX);
        if (entranceFrame.IsEmpty() == false)
        {
            if (m_removedBlockQueue.Count > 0)
            {
                StartCoroutine(CO_DropAllBlock());
            }
            else
            {
                Debug.Log("입구 막힘");
            }

            yield break;
        }

        yield return new WaitForSeconds(Const.BLOCK_DROP_NEW_WAIT);

        //새 블럭 추가(제거된 블럭의 타입을 바꿔서 재사용)
        Block newblock = m_removedBlockQueue.Dequeue();
        entranceFrame.SetBlock(newblock);
        entranceFrame.GetBlock().StartMove(entranceFrame.GetPosition(),
            () =>
            {
                DropBlock(entranceFrame);
                StartCoroutine(CO_InputNewBlock());
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

    bool IsTouchDisabled()
    {
        //유저 입력 막기
        return m_gameState != GameState.PLAY || m_isSwapping || m_isDropping;
    }

    void MoveBlockToFrame(Frame fromFrame, Frame toFrame, Action endMovevAction)
    {
        toFrame.SetBlock(fromFrame.GetBlock());
        fromFrame.SetEmpty();       
        toFrame.GetBlock().StartMove(toFrame.GetPosition(), endMovevAction);
    }

    void MatchingSideEffect()
    {
        //매칭 블럭 주변에 매칭 효과를 준다(쓰레기는 두번 받으면 제거됨)

        List<Frame> matchingNeighborList = new List<Frame>();

        for (int i = 0; i < m_matchingList.Count; ++i)
        {
            Index index = m_matchingList[i].GetIndex();

            MatchingSideEffect(GetFrameByDir(index, Direction.LEFTUP), matchingNeighborList);
            MatchingSideEffect(GetFrameByDir(index, Direction.UP), matchingNeighborList);
            MatchingSideEffect(GetFrameByDir(index, Direction.RIGHTUP), matchingNeighborList);
            MatchingSideEffect(GetFrameByDir(index, Direction.RIGHTDOWN), matchingNeighborList);
            MatchingSideEffect(GetFrameByDir(index, Direction.DOWN), matchingNeighborList);
            MatchingSideEffect(GetFrameByDir(index, Direction.LEFTDOWN), matchingNeighborList);
        }
    }

    void MatchingSideEffect(Frame frame, List<Frame> matchingNeighborList)
    {
        if (frame && frame.IsEmpty() == false &&
            matchingNeighborList.Contains(frame) == false)
        {
            matchingNeighborList.Add(frame);

            Block block = frame.GetBlock();
            bool needToRemove = block.AddMatchingNeighborCount();
            if(needToRemove)
            {
                if (m_needToRemoveGarbageList.Contains(frame) == false)
                {
                    m_needToRemoveGarbageList.Add(frame);
                }
            }
        }
    }

    void AfterCheckMatching()
    {
        //매칭 효과
        MatchingSideEffect();

        //제거
        RemoveBlockList();
        
        //드롭
        StartCoroutine(CO_DropAllBlock(true, false, false));
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

    void SetMatchableFrames()//todo 가이드 작업
    {
        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                var matchableFrames = FindMathableFrames(m_allFrameList[i][j]);
                if(matchableFrames.Count == 2)
                {
                    //Debug.Log(machableFrames[0].GetBlockType() + ", " + machableFrames[1].GetBlockType());
                    //Debug.Log(machableFrames[0].GetIndex().X + ", " + machableFrames[0].GetIndex().Y);
                    //Debug.Log(machableFrames[1].GetIndex().X + ", " + machableFrames[1].GetIndex().Y);
                    
                    m_matchingGuide[0].transform.localPosition = matchableFrames[0].GetPosition();
                    m_matchingGuide[1].transform.localPosition = matchableFrames[1].GetPosition();

                    return;
                }
            }
        }
    }

    List<Frame> FindMathableFrames(Frame frame)//todo 가이드 작업
    {
        List<Frame> machableFrames = new List<Frame>();
        BlockType checkBlockType = frame.GetBlockType();

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Frame sameType = IsSameType(checkBlockType, frame.GetIndex(), direction);
            if(sameType)
            {
                Index sideFrameindex = Util.CalcIndex(sameType.GetIndex(), direction);
                foreach (Direction direction2 in Enum.GetValues(typeof(Direction)))
                {
                    Frame sameType2 = IsSameType(checkBlockType, sideFrameindex, direction2);
                    if (sameType2 && sameType2 != sameType)
                    {
                        machableFrames.Add(GetFrameByIndex(sideFrameindex));
                        machableFrames.Add(sameType2);
                        return machableFrames;
                    }
                }
            }
        }

        return machableFrames;
    }

    Frame IsSameType(BlockType checkBlockType, Index index, Direction direction)//todo frame 두개 인자로 받아 비교
    {
        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.GARBAGE)
        {
            return null;
        }

        Frame frame = GetFrameByDir(index, direction);
        if (frame != null && frame.GetBlockType() == checkBlockType)
        {
            return frame;
        }

        return null;
    }

    void SetScore(int score)
    {
        m_score = score;
        m_ingameUI?.SetScore(score);
    }

    void SetGameOverTimer(float time)
    {
        if(time <= 0f)
        {
            time = 0f;
            m_resultPopup?.Show();
            m_gameState = GameState.GAMEOVER;
        }

        m_gameOverTimer = time;
        m_ingameUI?.SetTimerGauge(time);
    }
}
