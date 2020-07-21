using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{
    public Text m_debugLabel;//pyk 제거

    bool m_active = false;
    Index m_index;
    Block m_block = null;

    public void Init(bool active, Index index)
    {
        m_active = active;
        m_index = index;
        m_block = null;

        gameObject.SetActive(active);

        if (m_debugLabel)
        {
            m_debugLabel.text = index.X + "," + index.Y;
        }
    }

    public void SetBlock(Block block)
    {
        if (m_block != null)
        {
            Debug.LogError("SetBlock() : m_block != null, " + m_index.X + ", " + m_index.Y + ", " + m_block.GetBlockType() + ", " + block.GetBlockType());            

            return;
        }

        m_block = block;
        block.SetIndex(m_index);
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
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

    public bool IsMoveable()
    {
        return m_block && m_block.GetBlockType() != BlockType.NONE;
    }

    public bool IsActive()
    {
        return m_active;
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Index GetIndex()
    {
        return m_index;
    }

    public void On_PointerDown()
    {
        IngameManager.Instance.FramePointerDown(this);
    }

    public void On_PointerUp()//다른 오브젝트 엔터 전에 마우스 떼면 끝나도록(버그 방지)
    {
        IngameManager.Instance.FramePointerUp();
    }

    public void On_PointerEnter()
    {
        IngameManager.Instance.FramePointerEnter(this);
    }
}
