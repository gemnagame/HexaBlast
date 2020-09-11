using UnityEngine;

public class Frame : MonoBehaviour
{
    Index m_index;
    Block m_block = null;

    public void Init(bool active, Index index)
    {
        m_index = index;
        m_block = null;

        gameObject.SetActive(active);
    }

    public void SetBlock(Block block)
    {
        if (m_block != null)
        {
            return;
        }

        m_block = block;
    }

    public Block GetBlock()
    {
        return m_block;
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }    

    public BlockType GetBlockType()
    {
        if(m_block == null)
        {
            return BlockType.NONE;
        }

        return m_block.GetBlockType();
    }

    public void SetEmpty()
    {
        m_block = null;
    }

    public bool IsEmpty()
    {
        return m_block == null;
    }

    public bool IsMoveable()
    {
        return m_block && m_block.GetBlockType() != BlockType.NONE;
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Index GetIndex()
    {
        return m_index;
    }    
}
