using UnityEngine;
using UnityEngine.UI;

public class ColorBlock : MonoBehaviour
{
    public Text m_debugLabel;
    public Image m_imageBlock;

    bool m_use = false;
    BlockType m_blockType = BlockType.NONE;

    public void Init(BlockType blockType, int indexX, int indexY)
    {
        m_use = blockType != BlockType.NONE;
        
        gameObject.SetActive(m_use);

        SetType(blockType);

        if(m_debugLabel)
        {
            m_debugLabel.text = indexX + "," + indexY;
        }
    }

    void SetType(BlockType blockType)
    {
        if (m_imageBlock == null)
        {
            return;
        }

        m_blockType = blockType;
        Color color = Color.white;

        switch (blockType)
        {
            case BlockType.RED:
                {
                    color = Color.red;
                    break;
                }
            case BlockType.ORANGE:
                {
                    color = new Color(1, 128f/255f, 0, 1);
                    break;
                }
            case BlockType.YELLOW:
                {
                    color = Color.yellow;
                    break;
                }
            case BlockType.GREEN:
                {
                    color = Color.green;
                    break;
                }
            case BlockType.BLUE:
                {
                    color = Color.blue;
                    break;
                }
            case BlockType.PURPLE:
                {
                    color = Color.magenta;
                    break;
                }
        }

        m_imageBlock.color = color;        
    }
}