using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance = null;

    public GameObject m_frameOrigin;
    public GameObject m_blockOrigin;
    public Transform m_frameAreaTrans;
    public Transform m_blockAreaTrans;

    List<List<Frame>> m_allFrameList = new List<List<Frame>>();
    List<Block> m_allBlockList = new List<Block>();

    bool m_isGameReady = false;

    //swap
    bool m_isSwapping = false;
    Frame m_frameStart = null;
    int m_moveCompleteCount = 0;

    //matching
    List<Frame> m_matchingFrameList = new List<Frame>();    
    List<Frame> m_tempMatchingFrameList = new List<Frame>();
    int m_matchingStraightCount = 0;//pyk 함수 내 static 변수로 둘까

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

        CreateObjectOnce();        
    }

    void CreateObjectOnce()
    {
        if(m_allBlockList.Count > 0 || m_allFrameList.Count > 0)
        {
            Debug.LogError("CreateObjectOnce : m_allBlockList.Count > 0 || m_allFrameList.Count > 0");
            return;
        }

        if (m_frameOrigin == null || m_blockOrigin == null || m_frameAreaTrans == null || m_blockAreaTrans == null)
        {
            return;
        }

        RectTransform rectTransform = m_frameOrigin.GetComponent<RectTransform>();
        if (rectTransform)
        {
            rectTransform.sizeDelta = new Vector2(Const.FRAME_IMAGE_WIDTH, Const.FRAME_IMAGE_HEIGHT);
        }

        //1회만 생성, 이후 게임 재시작시 생성된 블럭들 재이용
        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            m_allFrameList.Add(new List<Frame>());

            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                Vector3 position = Util.CalcPositionByIndex(i, j);

                GameObject obj = Instantiate(m_frameOrigin, position, Quaternion.identity, m_frameAreaTrans);
                if (obj)
                {
                    Frame frame = obj.GetComponent<Frame>();
                    m_allFrameList[i].Add(frame);
                }

                //obj = Instantiate(m_blockOrigin, position, Quaternion.identity, m_blockAreaTrans);
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
        SetMapDesign();
    }

    void SetMapDesign()
    {
        int cou = 0;
        for (int i = 0; i < Const.MAPSIZE_X; ++i)
        {
            for (int j = 0; j < Const.MAPSIZE_Y; ++j)
            {
                BlockType blockType = BlockType.NONE;

                int indexX = Const.MAPSIZE_Y - 1 - j;
                //if(indexX >= 0 && indexX < m_mapSizeY)
                //if(Util.IsOutOfIndex(indexX, m_mapSizeY) == false)//pyk 이부분 맞는지 체크 필요
                {
                    blockType = Const.MAPDESIGN[indexX, i];
                }

                m_allFrameList[i][j].Init(blockType != BlockType.NONE, new Index(i, j));
                m_allFrameList[i][j].SetBlock(m_allBlockList[cou]);

                m_allBlockList[cou].Init(blockType);
                m_allBlockList[cou].SetPosition(m_allFrameList[i][j].GetPosition());

                cou++;
            }
        }

        m_isGameReady = true;
    }

    //void Update()
    //{

    //}

    void OnDestroy()
    {
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
        if (m_isGameReady == false || m_isSwapping)
        {
            return;
        }

        Debug.Log("FramePointerDown");

        m_frameStart = frameStart;
    }

    public void FramePointerUp()
    {
        if (m_isGameReady == false || m_isSwapping)
        {
            return;
        }

        Debug.Log("FramePointerUp");

        m_frameStart = null;
    }

    public void FramePointerEnter(Frame frameEnd)
    {
        if (m_isGameReady == false || m_isSwapping || 
            m_frameStart == null || frameEnd == null)
        {
            return;
        }

        Debug.Log("FramePointerEnter");

        if (m_frameStart.GetBlockType() == BlockType.NONE || 
            frameEnd.GetBlockType() == BlockType.NONE)
        {
            return;
        }

        if (m_frameStart != frameEnd)
        {
            SwapAndCheckMatching(m_frameStart, frameEnd);

            m_frameStart = null;
        }
    }

    void SwapAndCheckMatching(Frame frame1, Frame frame2)
    {
        m_isSwapping = true;
        Debug.Log("스왑 시작");
        m_moveCompleteCount = 0;

        bool isMatching = false;        

        Block block1 = frame1.GetBlock();
        frame1.SetEmpty();
        Block block2 = frame2.GetBlock();
        frame2.SetEmpty();
        frame1.SetBlock(block2);
        frame2.SetBlock(block1);
        frame1.GetBlock().StartMove(frame1.GetPosition(),
            () => EndSwapAction(isMatching, frame1, frame2));
        frame2.GetBlock().StartMove(frame2.GetPosition(),
            () => EndSwapAction(isMatching, frame1, frame2));

        isMatching |= CheckMatching(frame1);
        isMatching |= CheckMatching(frame2);
    }

    void EndSwapAction(bool isMatching, Frame frame1, Frame frame2)
    {
        m_moveCompleteCount++;
        if (m_moveCompleteCount == 2)
        {
            m_moveCompleteCount = 0;

            if (isMatching)
            { 
                RemoveMathcingList();
                m_isSwapping = false;
                Debug.Log("스왑 종료");

                FillEmptyArea();
            }
            else
            {
                SwapBack(frame1, frame2);
            }
        }
    }

    void SwapBack(Frame frame1, Frame frame2)
    {
        m_moveCompleteCount = 0;

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
        m_moveCompleteCount++;
        if (m_moveCompleteCount == 2)
        {
            m_moveCompleteCount = 0;

            m_isSwapping = false;
            Debug.Log("스왑 종료");
        }
    }

    bool CheckMatching(Frame frame)
    {
        //m_matchingFrameList.Clear();
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
            AddMatchingList(new List<Frame> { frame });            
        }

        return isMatching;
    }

    bool CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction1, Direction direction2)
    {
        m_matchingStraightCount = 0;
        m_tempMatchingFrameList.Clear();
        CheckMatchingStraight(checkBlockType, index, direction1);
        CheckMatchingStraight(checkBlockType, index, direction2);
        if (m_matchingStraightCount >= 2)
        {
            AddMatchingList(m_tempMatchingFrameList);
            return true;
        }

        return false;
    }

    void CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction)//재귀
    {
        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.TOP)
        {
            return;
        }

        Index newIndex = Util.CalcIndex(index, direction);
        if (Util.IsOutOfIndex(newIndex, Const.MAPSIZE_X, Const.MAPSIZE_Y))
        {
            return;
        }

        BlockType blockType = m_allFrameList[newIndex.X][newIndex.Y].GetBlockType();
        if (blockType == checkBlockType)
        {
            m_matchingStraightCount++;
            m_tempMatchingFrameList.Add(m_allFrameList[newIndex.X][newIndex.Y]);

            CheckMatchingStraight(checkBlockType, newIndex, direction);
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
            m_tempMatchingFrameList.Clear();
            m_tempMatchingFrameList.Add(m_allFrameList[index1.X][index1.Y]);
            m_tempMatchingFrameList.Add(m_allFrameList[index2.X][index2.Y]);
            m_tempMatchingFrameList.Add(m_allFrameList[index3.X][index3.Y]);
            AddMatchingList(m_tempMatchingFrameList);

            return true;
        }

        return false;
    }

    void AddMatchingList(List<Frame> frameList)
    {
        for(int i = 0; i < frameList.Count; ++i)
        {
            //중복 방지
            if(m_matchingFrameList.Contains(frameList[i]) == false)
            {
                m_matchingFrameList.Add(frameList[i]);
            }
        }
    }

    void RemoveMathcingList()
    {
        for(int i = 0; i < m_matchingFrameList.Count; ++i)
        {
            m_matchingFrameList[i].GetBlock().SetPosition(new Vector3(0, 500, 0));
            m_matchingFrameList[i].SetEmpty();
        }

        m_matchingFrameList.Clear();
    }

    void FillEmptyArea()
    {
        for (int i = 0; i < m_allFrameList.Count; ++i)
        {
            for (int j = 0; j < m_allFrameList[i].Count; ++j)
            {
                if (m_allFrameList[i][j])
                {
                    //빈 곳 채우기

                }
            }
        }
    }
}
