using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance = null;

    public GameObject m_frameBlockOrigin;
    public GameObject m_colorBlockOrigin;
    public Transform m_frameBlockAreaTrans;
    public Transform m_colorBlockAreaTrans;

    bool m_gameReady = false;

    List<List<FrameBlock>> m_frameBlockList = new List<List<FrameBlock>>();
    float m_imageWidth = 70f;
    float m_imageHeight = 70f;

    List<ColorBlock> m_colorBlockList = new List<ColorBlock>();

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

        if (m_frameBlockOrigin == null || m_colorBlockOrigin == null || m_frameBlockAreaTrans == null || m_colorBlockAreaTrans == null)
        {
            return;
        }

        RectTransform rectTransform = m_frameBlockOrigin.GetComponent<RectTransform>();
        if (rectTransform)
        {
            m_imageWidth = rectTransform.rect.width;
            m_imageHeight = rectTransform.rect.height;
        }

        m_mapSizeX = m_mapDesign.GetLength(1);//7
        m_mapSizeY = m_mapDesign.GetLength(0);//6


        //pyk for문 함수로 빼자. 1회만 생성, 이후 게임 재시작시 생성된 블럭들 재이용
        for (int i = 0; i < m_mapSizeX; ++i)
        {
            m_frameBlockList.Add(new List<FrameBlock>());

            for (int j = 0; j < m_mapSizeY; ++j)
            {
                Vector3 position = CalcPositionByIndex(i, j);

                GameObject obj = Instantiate(m_frameBlockOrigin, position, Quaternion.identity, m_frameBlockAreaTrans);
                if (obj)
                {
                    FrameBlock frameBlock = obj.GetComponent<FrameBlock>();
                    m_frameBlockList[i].Add(frameBlock);
                }

                //obj = Instantiate(m_colorBlockOrigin, position, Quaternion.identity, m_puzzleAreaTrans);
                obj = Instantiate(m_colorBlockOrigin, m_colorBlockAreaTrans);
                if (obj)
                {
                    ColorBlock colorBlock = obj.GetComponent<ColorBlock>();
                    m_colorBlockList.Add(colorBlock);
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

                m_frameBlockList[i][j].Init(blockType != BlockType.NONE, new Index(i, j));
                m_frameBlockList[i][j].SetColorBlock(m_colorBlockList[cou]);

                m_colorBlockList[cou].Init(blockType);
                m_colorBlockList[cou].SetPosition(m_frameBlockList[i][j].GetPosition());

                cou++;
            }
        }

        m_gameReady = true;
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        Debug.Log("m_frameBlockList.Count : " + m_frameBlockList.Count);
        for (int i = 0; i < m_frameBlockList.Count; ++i)
        {
            Debug.Log("m_frameBlockList[i].Count : " + m_frameBlockList[i].Count);
            for (int j = 0; j < m_frameBlockList[i].Count; ++j)
            {
                if (m_frameBlockList[i][j])
                {
                    Destroy(m_frameBlockList[i][j].gameObject);
                }
            }
        }

        Debug.Log("m_colorBlockList.Count : " + m_colorBlockList.Count);
        for (int i = 0; i < m_colorBlockList.Count; ++i)
        {
            if (m_colorBlockList[i])
            {
                Destroy(m_colorBlockList[i].gameObject);
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

    FrameBlock m_frameBlockStart = null;
    public void FrameBlockPointerDown(FrameBlock frameBlockStart)
    {
        if (m_gameReady == false)
        {
            return;
        }

        m_frameBlockStart = frameBlockStart;
    }

    public void FrameBlockPointerUp()
    {
        if (m_gameReady == false)
        {
            return;
        }

        m_frameBlockStart = null;
    }

    public void FrameBlockPointerEnter(FrameBlock frameBlockEnd)
    {
        if (m_gameReady == false || m_frameBlockStart == null || frameBlockEnd == null)
        {
            return;
        }

        if(m_frameBlockStart.GetColorBlockType() == BlockType.NONE || 
            frameBlockEnd.GetColorBlockType() == BlockType.NONE)
        {
            return;
        }

        if (m_frameBlockStart != frameBlockEnd)
        {
            Swap(m_frameBlockStart, frameBlockEnd);

            m_frameBlockStart = null;
        }
    }

    void Swap(FrameBlock frameBlock1, FrameBlock frameBlock2)
    {
        ColorBlock colorBlock1 = frameBlock1.GetColorBlock();
        ColorBlock colorBlock2 = frameBlock2.GetColorBlock();
        frameBlock1.SetColorBlock(colorBlock2);
        frameBlock2.SetColorBlock(colorBlock1);
        frameBlock1.GetColorBlock().SetPosition(frameBlock1.GetPosition());
        frameBlock2.GetColorBlock().SetPosition(frameBlock2.GetPosition());

        //swap한 이후 매칭 체크, swap 이외에도 위치 변경 있을 경우 매칭 체크
        CheckMatching(frameBlock1);
        CheckMatching(frameBlock2);
    }

    bool m_matching = false;
    List<FrameBlock> m_matchingList = new List<FrameBlock>();
    int m_tempMatchingCount = 0;
    List<FrameBlock> m_tempMatchingList = new List<FrameBlock>();     

    void CheckMatching(FrameBlock frameBlock)
    {
        m_matchingList.Clear();
        m_matching = false;

        BlockType checkBlockType = frameBlock.GetColorBlockType();
        Index index = frameBlock.GetIndex();

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
        CheckMatchingSquare(checkBlockType, index_leftUp, index_up, index_leftUp_up);
        //+ RightUp
        CheckMatchingSquare(checkBlockType, index_leftUp, index_up, index_rightUp);

        //Up, RightUp
        //+ RightUp_Up
        CheckMatchingSquare(checkBlockType, index_up, index_rightUp, index_rightUp_up);
        //+ RightDown
        CheckMatchingSquare(checkBlockType, index_up, index_rightUp, index_rightDown);

        //RightUp, RightDown
        //+ RightUp_RightDown
        CheckMatchingSquare(checkBlockType, index_rightUp, index_rightDown, index_rightUp_rightDown);
        //+ Down
        CheckMatchingSquare(checkBlockType, index_rightUp, index_rightDown, index_down);

        //RightDown, Down
        //+ RightDown_Down
        CheckMatchingSquare(checkBlockType, index_rightDown, index_down, index_rightDown_down);
        //+ LeftDown
        CheckMatchingSquare(checkBlockType, index_rightDown, index_down, index_leftDown);

        //Down, LeftDown
        //+ LeftDown_Down
        CheckMatchingSquare(checkBlockType, index_down, index_leftDown, index_leftDown_down);
        //+ LeftUp
        CheckMatchingSquare(checkBlockType, index_down, index_leftDown, index_leftUp);

        //LeftDown, LeftUp
        //+ LeftDown_LeftUp
        CheckMatchingSquare(checkBlockType, index_leftDown, index_leftUp, index_leftDown_leftUp);
        //+ Up
        CheckMatchingSquare(checkBlockType, index_leftDown, index_leftUp, index_up);


        //2. Straight

        //Up + Down
        CheckMatchingStraight(checkBlockType, index, Direction.UP, Direction.DOWN);

        //LeftUp + RightDown
        CheckMatchingStraight(checkBlockType, index, Direction.LEFTUP, Direction.RIGHTDOWN);

        //LeftDown + RightUp
        CheckMatchingStraight(checkBlockType, index, Direction.LEFTDOWN, Direction.RIGHTUP);

        //기준 블럭 포함하여 매칭블록 전체 제거
        if (m_matching)
        {
            m_matchingList.Add(frameBlock);
            RemoveMathcingList();
        }
    }

    void CheckMatchingStraight(BlockType checkBlockType, Index index, Direction direction1, Direction direction2)
    {
        m_tempMatchingCount = 0;
        m_tempMatchingList.Clear();
        CheckMatchingStraight(checkBlockType, index, direction1);
        CheckMatchingStraight(checkBlockType, index, direction2);
        if (m_tempMatchingCount >= 2)
        {
            AddMatchingList();
            m_matching = true;
        }
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

        BlockType blockType = m_frameBlockList[newIndex.X][newIndex.Y].GetColorBlockType();
        if (blockType == checkBlockType)
        {
            m_tempMatchingCount++;
            m_tempMatchingList.Add(m_frameBlockList[newIndex.X][newIndex.Y]);

            CheckMatchingStraight(checkBlockType, newIndex, direction);
        }
    }

    void CheckMatchingSquare(BlockType checkBlockType, Index index1, Index index2, Index index3)
    {
        if (checkBlockType == BlockType.NONE || checkBlockType == BlockType.TOP)
        {
            return;
        }

        if (IsOutOfIndex(index1, m_mapSizeX, m_mapSizeY) ||
            IsOutOfIndex(index2, m_mapSizeX, m_mapSizeY) ||
            IsOutOfIndex(index3, m_mapSizeX, m_mapSizeY))
        {
            return;
        }

        BlockType blockType1 = m_frameBlockList[index1.X][index1.Y].GetColorBlockType();
        BlockType blockType2 = m_frameBlockList[index2.X][index2.Y].GetColorBlockType();
        BlockType blockType3 = m_frameBlockList[index3.X][index3.Y].GetColorBlockType();
        if (blockType1 == checkBlockType &&
            blockType2 == checkBlockType &&
            blockType3 == checkBlockType)
        {
            m_tempMatchingList.Clear();
            m_tempMatchingList.Add(m_frameBlockList[index1.X][index1.Y]);
            m_tempMatchingList.Add(m_frameBlockList[index2.X][index2.Y]);
            m_tempMatchingList.Add(m_frameBlockList[index3.X][index3.Y]);

            AddMatchingList();
            m_matching = true;
        }
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

    void AddMatchingList()
    {
        for(int i = 0; i < m_tempMatchingList.Count; ++i)
        {
            //중복 방지
            if(m_matchingList.Contains(m_tempMatchingList[i]) == false)
            {
                m_matchingList.Add(m_tempMatchingList[i]);
            }
        }
    }

    void RemoveMathcingList()
    {
        for(int i = 0; i < m_matchingList.Count; ++i)
        {
            m_matchingList[i].GetColorBlock().SetPosition(new Vector3(0, 500, 0));
            m_matchingList[i].SetColorBlock(null);//pyk
        }
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
