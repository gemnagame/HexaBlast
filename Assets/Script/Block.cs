using System;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public Image m_imageBlock;
    public Text m_debugLabel;//pyk 제거

    BlockType m_blockType = BlockType.NONE;
    int m_bombCount = 0;

    //move
    bool m_moving = false;
    Vector3 m_moveStartPosition = Vector3.zero;
    Vector3 m_moveTargetPosition = Vector3.zero;
    Action m_endMoveAction = null;    
    float m_moveStartTime;

    Index m_index;//pyk 제거

    public void Init(BlockType blockType)
    {
        SetBlockType(blockType);

        gameObject.SetActive(true);// m_use);//pyk test

        m_bombCount = 0;        
    }

    void Update()
    {
        if (m_moving)
        {
            //transform.position = Vector3.MoveTowards(transform.position, m_moveTargetPosition, Time.deltaTime * Const.BLOCK_MOVE_SPEED);

            //m_movedTime += Time.deltaTime;
            float movedTime = Time.time - m_moveStartTime;
            transform.position = Vector3.Lerp(m_moveStartPosition, m_moveTargetPosition, movedTime / Const.BLOCK_MOVE_TIME);

            if (transform.position == m_moveTargetPosition)
            {
                m_moving = false;

                if (m_endMoveAction != null)
                {
                    m_endMoveAction();
                }
            }
        }    
    }

    public void SetIndex(Index index)//pyk 제거
    {
        m_index = index;

        if (m_debugLabel)
        {
            m_debugLabel.text = index.X + "," + index.Y;
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void StartMove(Vector3 moveTargetPosition, Action endMoveAction)
    {
        if(m_moving)
        {
            Debug.LogError("StartMove() : m_moving");            
            return;
        }

        m_moveStartTime = Time.time;
        m_moving = true;
        m_moveStartPosition = transform.position;
        m_moveTargetPosition = moveTargetPosition;
        m_endMoveAction = endMoveAction;
    }

    public BlockType GetBlockType()
    {
        return m_blockType;
    }       

    void SetBlockType(BlockType blockType)
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

    public bool AddBombCount()//return : need to remove
    {
        if (m_blockType != BlockType.TOP)
        {
            return false;
        }
        
        m_bombCount++;

        switch (m_bombCount)
        {
            case 1:
                {
                    m_imageBlock.color = Color.gray;
                    return false;
                }
            case 2:
                {
                    m_bombCount = 0;
                    return true;
                }
        }

        return false;
    }
}