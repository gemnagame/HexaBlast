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

    BlockType[,] m_mapDesign = new BlockType[,]
        { 
            {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.PURPLE, BlockType.NONE, BlockType.NONE, BlockType.NONE},
            {BlockType.NONE, BlockType.ORANGE, BlockType.GREEN, BlockType.RED, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
            {BlockType.YELLOW, BlockType.PURPLE, BlockType.TOP, BlockType.YELLOW, BlockType.ORANGE, BlockType.TOP, BlockType.GREEN},
            {BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.BLUE, BlockType.RED, BlockType.PURPLE, BlockType.YELLOW},
            {BlockType.BLUE, BlockType.TOP, BlockType.YELLOW, BlockType.PURPLE, BlockType.GREEN, BlockType.PURPLE, BlockType.GREEN},
            {BlockType.NONE, BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.TOP, BlockType.NONE, BlockType.NONE}
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

        DontDestroyOnLoad(gameObject);


        if (m_frameBlockOrigin == null || m_colorBlockOrigin == null || m_frameBlockAreaTrans == null || m_colorBlockAreaTrans == null)
        {
            return;
        }

        RectTransform rectTransform = m_frameBlockOrigin.GetComponent<RectTransform>();
        if(rectTransform)
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
            m_frameBlockList.Add(new List<FrameBlock>());

            for (int j = 0; j < m_mapSizeY; ++j)
            {
                BlockType blockType = BlockType.NONE;

                int indexX = m_mapSizeY - 1 - j;
                if(indexX >= 0 && indexX < m_mapSizeY)
                {
                    blockType = m_mapDesign[indexX, i];
                }

                m_frameBlockList[i][j].Init(blockType != BlockType.NONE, i, j);
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
        for(int i = 0; i < m_frameBlockList.Count; ++i)
        {
            for(int j = 0; j < m_frameBlockList[i].Count; ++j)
            {
                if (m_frameBlockList[i][j])
                {
                    Destroy(m_frameBlockList[i][j].gameObject);
                }
            }
        }

        for(int i = 0; i < m_colorBlockList.Count; ++i)
        {
            if(m_colorBlockList[i])
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

        if(m_frameBlockStart != frameBlockEnd)
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
        Vector2 index1 = frameBlock1.GetIndex();
        Vector2 index2 = frameBlock2.GetIndex();
        CheckMatching(frameBlock1);
        CheckMatching(frameBlock2);
    }

    int m_matchingCount = 0;

    void CheckMatching(FrameBlock frameBlock)
    {
        BlockType blockType = frameBlock.GetColorBlockType();
        Vector2 index = frameBlock.GetIndex();

        //up
        m_matchingCount = 0;
        CheckMatching(blockType, (int)index.x, (int)index.y, 0, 1);
        Debug.Log(index + " UP : " + m_matchingCount);

        //down
        m_matchingCount = 0;
        CheckMatching(blockType, (int)index.x, (int)index.y, 0, -1);
        Debug.Log(index + " DOWN : " + m_matchingCount);
    }

    void CheckMatching(BlockType blockType, int indexX, int indexY, int addX, int addY)
    {
        indexX += addX;
        indexY += addY;

        if (indexX < 0 || indexX >= m_mapSizeX || indexY < 0 || indexY >= m_mapSizeY)
        {
            return;
        }

        if (blockType != m_frameBlockList[indexX][indexY].GetColorBlockType())
        {
            return;
        }

        m_matchingCount++;

        CheckMatching(blockType, indexX, indexY, addX, addY);
    }
}
