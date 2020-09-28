using UnityEngine;

public class Frame : MonoBehaviour
{
    Index m_index;
    Block m_block = null;

    public void Init(Index index, Vector3 position, Block block)
    {
        m_index = index;

        SetPosition(position);
        SetBlock(block);
    }

    public void SetBlock(Block block)
    {
        if (m_block != null)
        {
            return;
        }

        m_block = block;
        gameObject.SetActive(GetBlockType() != BlockType.NONE);
    }

    public Block GetBlock()
    {
        return m_block;
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

    public bool IsMoveable()//todo 이거 왜 m_moveing은 체크를 안할까...? 이유가 있겠지.. 다시 살펴보자
    {
        return GetBlockType() != BlockType.NONE;
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Index GetIndex()
    {
        return m_index;
    }

    void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}
