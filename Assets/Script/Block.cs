using System;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public Image m_imageBlock;

    BlockType m_blockType = BlockType.NONE;
    bool m_moving = false;
    Vector3 m_moveTargetPosition = Vector3.zero;
    Action m_endMoveAction = null;

    public void Init(BlockType blockType)
    {
        SetType(blockType);

        gameObject.SetActive(true);// m_use);//pyk test
    }

    void Update()
    {
        if (m_moving)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_moveTargetPosition, Time.deltaTime * Const.BLOCK_MOVE_SPEED);

            if(transform.localPosition == m_moveTargetPosition)
            {
                m_moving = false;

                if (m_endMoveAction != null)
                {
                    m_endMoveAction();
                }
            }
        }    
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void StartMove(Vector3 moveTargetPosition, Action endMoveAction = null)
    {
        if(m_moving)
        {
            Debug.LogError("StartMove() : m_moving");
            return;
        }

        m_moving = true;
        m_moveTargetPosition = moveTargetPosition;
        m_endMoveAction = endMoveAction;
    }

    public BlockType GetBlockType()
    {
        return m_blockType;
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
            case BlockType.TOP:
                {
                    color = Color.black;
                    break;
                }
        }

        m_imageBlock.color = color;        
    }
}