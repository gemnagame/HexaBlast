using UnityEngine;
using UnityEngine.UI;

public class FrameBlock : MonoBehaviour
{
    public Text m_debugLabel;

    bool m_active = false;
    int m_indexX = 0;
    int m_indexY = 0;
    ColorBlock m_colorBlock = null;   

    public void Init(bool active, int indexX, int indexY)
    {
        m_active = active;
        m_indexX = indexX;
        m_indexY = indexY;

        gameObject.SetActive(active);

        if(m_debugLabel)
        {
            m_debugLabel.text = indexX + "," + indexY;
        }
    }

    public void SetColorBlock(ColorBlock colorBlock)//pyk  아래 함수들 getta setta로 수정?
    {
        m_colorBlock = colorBlock;        
    }

    public ColorBlock GetColorBlock()
    {
        return m_colorBlock;
    }

    public BlockType GetColorBlockType()
    {
        if(m_colorBlock == null)
        {
            return BlockType.NONE;
        }

        return m_colorBlock.GetBlockType();
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Vector2 GetIndex()
    {
        return new Vector2(m_indexX, m_indexY);
    }

    public void On_PointerDown()
    {
        IngameManager.Instance.FrameBlockPointerDown(this);
    }

    public void On_PointerUp()//다른 오브젝트 엔터 전에 마우스 떼면 끝나도록(버그 방지)
    {
        IngameManager.Instance.FrameBlockPointerUp();
    }

    public void On_PointerEnter()
    {
        IngameManager.Instance.FrameBlockPointerEnter(this);
    }
}
