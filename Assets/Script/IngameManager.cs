using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public GameObject m_frameBlockOrigin;
    public GameObject m_colorBlockOrigin;
    public Transform m_puzzleAreaTrans;

    List<FrameBlock> m_frameBlockList = new List<FrameBlock>();
    float m_imageWidth = 70f;
    float m_imageHeight = 70f;

    List<ColorBlock> m_colorBlockList = new List<ColorBlock>();

    List<BlockType> m_colorBlockDesign = new List<BlockType>
    {
        BlockType.NONE, BlockType.BLUE, BlockType.GREEN, BlockType.YELLOW, BlockType.NONE, BlockType.NONE,
        BlockType.NONE, BlockType.TOP, BlockType.PURPLE, BlockType.PURPLE, BlockType.ORANGE, BlockType.NONE,
        BlockType.TOP, BlockType.YELLOW, BlockType.YELLOW, BlockType.TOP, BlockType.GREEN, BlockType.NONE,
        BlockType.TOP, BlockType.PURPLE, BlockType.BLUE, BlockType.YELLOW, BlockType.RED, BlockType.PURPLE,
        BlockType.TOP, BlockType.GREEN, BlockType.RED, BlockType.ORANGE, BlockType.YELLOW, BlockType.NONE,
        BlockType.NONE, BlockType.PURPLE, BlockType.PURPLE, BlockType.TOP, BlockType.RED, BlockType.NONE,
        BlockType.NONE, BlockType.GREEN, BlockType.YELLOW, BlockType.GREEN, BlockType.NONE, BlockType.NONE
    };


    void Awake()
    {
        if(m_frameBlockOrigin == null || m_frameBlockOrigin == null || m_puzzleAreaTrans == null)
        {
            return;
        }

        RectTransform rectTransform = m_frameBlockOrigin.GetComponent<RectTransform>();
        if(rectTransform)
        {
            m_imageWidth = rectTransform.rect.width;
            m_imageHeight = rectTransform.rect.height;
        }

        for (int i = 0; i < Const.SIZE_X; ++i)
        {
            for (int j = 0; j < Const.SIZE_Y; ++j)
            {
                //pyk use가 true일때만 블럭들 instantiate하면 될듯

                Vector3 position = GetPosition(i, j);

                BlockType blockType = BlockType.NONE;
                int index = i * Const.SIZE_Y + j;
                Debug.Log(i + "," + j + "=" + index);
                if (index < m_colorBlockDesign.Count)
                {
                    blockType = m_colorBlockDesign[index];
                }

                GameObject obj = Instantiate(m_frameBlockOrigin, position, Quaternion.identity, m_puzzleAreaTrans);
                if (obj)
                {
                    FrameBlock frameBlock = obj.GetComponent<FrameBlock>();
                    m_frameBlockList.Add(frameBlock);
                    frameBlock.Init(blockType != BlockType.NONE, i, j);
                }

                obj = Instantiate(m_colorBlockOrigin, position, Quaternion.identity, m_puzzleAreaTrans);
                if (obj)
                {
                    ColorBlock colorBlock = obj.GetComponent<ColorBlock>();
                    m_colorBlockList.Add(colorBlock);
                    colorBlock.Init(blockType, i, j);
                }
            }
        }
    }

    Vector3 GetPosition(int indexX, int indexY)//pyk space x, y 따로 상수값 두지말고 직접 계산하자(가운데정렬)
    {
        float addPosY = indexX % 2 == 0 ? m_imageHeight / 2 : 0;
        Vector3 position = new Vector3(
            indexX * m_imageWidth + Const.SPACE_X,
            indexY * m_imageHeight + Const.SPACE_Y + addPosY,
            0);

        return position;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        for(int i = 0; i < m_frameBlockList.Count; ++i)
        {
            if (m_frameBlockList[i])
            {
                Destroy(m_frameBlockList[i].gameObject);
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
}
