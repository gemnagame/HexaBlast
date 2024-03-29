﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField]
    Image m_imageBlock = null;

    BlockType m_blockType = BlockType.NONE;
    int m_matchingNeighborCount = 0;

    //move
    bool m_moving = false;
    Vector3 m_moveStartPosition = Vector3.zero;
    Vector3 m_moveTargetPosition = Vector3.zero;
    Action m_endMoveAction = null;
    float m_moveStartTime = 0f;

    public void Init(BlockType blockType, Vector3 position)
    {
        SetPosition(position);
        SetBlockType(blockType);        

        m_matchingNeighborCount = 0;

        SetMove(false, Vector3.zero, Vector3.zero, null, 0f);
    }

    void Update()
    {
        ProcessMove();
    }    

    public void StartMove(Vector3 moveTargetPosition, Action endMoveAction)
    {
        if(m_moving)
        {
            return;
        }

        SetMove(true, transform.localPosition, moveTargetPosition, endMoveAction, Time.time);
    }

    public BlockType GetBlockType()
    {
        return m_blockType;
    }

    public bool AddMatchingNeighborCount()//return : need to remove
    {
        if (m_blockType != BlockType.GARBAGE)
        {
            return false;
        }

        m_matchingNeighborCount++;

        switch (m_matchingNeighborCount)
        {
            case 1:
                {
                    if (m_imageBlock)
                    {
                        m_imageBlock.color = Color.gray;
                    }

                    return false;
                }
            case 2:
                {
                    m_matchingNeighborCount = 0;
                    return true;
                }
        }

        return false;
    }

    void SetBlockType(BlockType blockType)
    {
        m_blockType = blockType;
        gameObject.SetActive(blockType != BlockType.NONE);

        if (m_imageBlock)
        {
            Sprite sprite = SpriteManager.Instance?.GetBlcokSprite(m_blockType);
            if (sprite)
            {
                m_imageBlock.sprite = sprite;
                m_imageBlock.color = Color.white;
            }
        }             
    }

    void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    void SetMove(bool moving, Vector3 moveStartPosition, Vector3 moveTargetPosition,
        Action endMoveAction, float moveStartTime)
    {
        m_moving = moving;
        m_moveStartPosition = moveStartPosition;
        m_moveTargetPosition = moveTargetPosition;
        m_endMoveAction = endMoveAction;
        m_moveStartTime = moveStartTime;
    }

    void ProcessMove()
    {
        if (m_moving)
        {
            float movedTime = Time.time - m_moveStartTime;
            transform.localPosition = Vector3.Lerp(m_moveStartPosition, m_moveTargetPosition, movedTime / Const.BLOCK_MOVE_TIME);

            if (transform.localPosition == m_moveTargetPosition)
            {
                m_moving = false;

                if (m_endMoveAction != null)
                {
                    m_endMoveAction();
                }
            }
        }
    }
}