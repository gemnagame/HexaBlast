using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance = null;

    public GameObject m_frameOrigin;
    public GameObject m_blockOrigin;
    public Transform m_frameAreaTrans;
    public Transform m_blockAreaTrans;

    bool m_isGameReady = false;
    bool m_isSwapping = false;

    List<List<Frame>> m_allFrameList = new List<List<Frame>>();
    float m_imageWidth = 70f;
    float m_imageHeight = 70f;

    List<Block> m_allBlockList = new List<Block>();

    //BlockType[,] m_mapDesign = new BlockType[,]
    //    { 
    //        {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.PURPLE, BlockType.NONE, BlockType.NONE, BlockType.NONE},
    //        {BlockType.NONE, BlockType.ORANGE, BlockType.GREEN, BlockType.RED, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
    //        {BlockType.YELLOW, BlockType.PURPLE, BlockType.TOP, BlockType.YELLOW, BlockType.ORANGE, BlockType.TOP, BlockType.GREEN},
    //        {BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.BLUE, BlockType.RED, BlockType.PURPLE, BlockType.YELLOW},
    //        {BlockType.BLUE, BlockType.TOP, BlockType.YELLOW, BlockType.PURPLE, BlockType.GREEN, BlockType.PURPLE, BlockType.GREEN},
    //        {BlockType.NONE, BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.TOP, BlockType.NONE, BlockType.NONE}
    //    };
    BlockType[,] m_mapDesign = new BlockType[,]
        {
            {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.RED, BlockType.NONE, BlockType.NONE, BlockType.NONE},
            {BlockType.NONE, BlockType.YELLOW, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
            {BlockType.PURPLE, BlockType.TOP, BlockType.PURPLE, BlockType.BLUE, BlockType.PURPLE, BlockType.PURPLE, BlockType.GREEN},
            {BlockType.TOP, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.YELLOW, BlockType.GREEN, BlockType.YELLOW},
            {BlockType.GREEN, BlockType.BLUE, BlockType.BLUE, BlockType.TOP, BlockType.BLUE, BlockType.GREEN, BlockType.GREEN},
            {BlockType.NONE, BlockType.NONE, BlockType.BLUE, BlockType.TOP, BlockType.PURPLE, BlockType.NONE, BlockType.NONE}
        };
    int m_mapSizeX = 0;
    int m_mapSizeY = 0;

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
            m_imageWidth = rectTransform.rect.width;
            m_imageHeight = rectTransform.rect.height;
        }

        m_mapSizeX = m_mapDesign.GetLength(1);//7
        m_mapSizeY = m_mapDesign.GetLength(0);//6

        //1회만 생성, 이후 게임 재시작시 생성된 블럭들 재이용
        for (int i = 0; i < m_mapSizeX; ++i)
        {
            m_allFrameList.Add(new List<Frame>());

            for (int j = 0; j < m_mapSizeY; ++j)
            {
                Vector3 position = CalcPositionByIndex(i, j);

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
        for (int i = 0; i < m_mapSizeX; ++i)
        {
            for (int j = 0; j < m_mapSizeY; ++j)
            {
                BlockType blockType = BlockType.NONE;

                int indexX = m_mapSizeY - 1 - j;
                //if(indexX >= 0 && indexX < m_mapSizeY)
                //if(IsOutOfIndex(indexX, m_mapSizeY) == false)//pyk 이부분 맞는지 체크 필요
                {
                    blockType = m_mapDesign[indexX, i];
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

    Vector3 CalcPositionByIndex(int indexX, int indexY)//pyk SPACE_X, Y 따로 상수값 두지말고 직접 계산하자(가운데정렬)
    {
        float addPosY = indexX % 2 == 0 ? m_imageHeight / 2 : 0;
        Vector3 position = new Vector3(
            indexX * m_imageWidth + Const.SPACE_X,
            indexY * m_imageHeight + Const.SPACE_Y + addPosY,
            0);

        return position;
    }

    Frame m_frameStart = null;
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

    int m_moveCompleteCount = 0;
    void SwapAndCheckMatching(Frame frame1, Frame frame2)
    {
        m_isSwapping = true;
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
            }
            else
            {
                JustSwap(frame1, frame2);
            }

            m_isSwapping = false;
        }
    }

    void JustSwap(Frame frame1, Frame frame2)
    {
        Block block1 = frame1.GetBlock();
        frame1.SetEmpty();
        Block block2 = frame2.GetBlock();
        frame2.SetEmpty();
        frame1.SetBlock(block2);
        frame2.SetBlock(block1);
        frame1.GetBlock().StartMove(frame1.GetPosition());
        frame2.GetBlock().StartMove(frame2.GetPosition());
    }

    //bool m_isMatching = false;
    List<Frame> m_matchingFrameList = new List<Frame>();
    int m_matchingStraightCount = 0;
    List<Frame> m_tempMatchingFrameList = new List<Frame>();     

    bool CheckMatching(Frame frame)
    {
        //m_matchingFrameList.Clear();
        bool isMatching = false;

        BlockType checkBlockType = frame.GetBlockType();
        Index index = frame.GetIndex();

        //1. Square

        Index index_leftUp              = CalcIndex(index, Direction.LEFTUP);
        Index index_leftUp_up           = CalcIndex(index_leftUp, Direction.UP);
        Index index_up                  = CalcIndex(index, Direction.UP);
        Index index_rightUp             = CalcIndex(index, Direction.RIGHTUP);
        Index index_rightUp_up          = CalcIndex(index_rightUp, Direction.UP);
        Index index_rightUp_rightDown   = CalcIndex(index_rightUp, Direction.RIGHTDOWN);
        Index index_rightDown           = CalcIndex(index, Direction.RIGHTDOWN);
        Index index_rightDown_down      = CalcIndex(index_rightDown, Direction.DOWN);
        Index index_down                = CalcIndex(index, Direction.DOWN);
        Index index_leftDown            = CalcIndex(index, Direction.LEFTDOWN);
        Index index_leftDown_down       = CalcIndex(index_leftDown, Direction.DOWN);
        Index index_leftDown_leftUp     = CalcIndex(index_leftDown, Direction.LEFTUP);

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
            //    m_matchingFrameList.Add(frame);
            //    RemoveMathcingList();
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

        Index newIndex = CalcIndex(index, direction);
        if (IsOutOfIndex(newIndex, m_mapSizeX, m_mapSizeY))
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

        if (IsOutOfIndex(index1, m_mapSizeX, m_mapSizeY) ||
            IsOutOfIndex(index2, m_mapSizeX, m_mapSizeY) ||
            IsOutOfIndex(index3, m_mapSizeX, m_mapSizeY))
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

    Index CalcIndex(Index index, Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                {
                    index.Y += 1;
                    break;
                }
            case Direction.DOWN:
                {
                    index.Y -= 1;
                    break;
                }
            case Direction.LEFTUP:
                {
                    index.Y += (index.X % 2 == 0 ? 1 : 0);
                    index.X -= 1;
                    break;
                }
            case Direction.LEFTDOWN:
                {
                    index.Y -= (index.X % 2 == 0 ? 0 : 1);
                    index.X -= 1;
                    break;
                }
            case Direction.RIGHTUP:
                {
                    index.Y += (index.X % 2 == 0 ? 1 : 0);
                    index.X += 1;

                    break;
                }
            case Direction.RIGHTDOWN:
                {
                    index.Y -= (index.X % 2 == 0 ? 0 : 1);
                    index.X += 1;
                    break;
                }
        }

        return index;
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

    bool IsOutOfIndex(Index index, int arraySizeX, int arraySizeY)
    {
        if(index.X < 0 || index.X >= arraySizeX ||
            index.Y < 0 || index.Y >= arraySizeY)
        {
            return true;
        }

        return false;
    }
}
