using UnityEngine;
using UnityEngine.UI;

public class FrameBlock : MonoBehaviour
{
    public Text m_debugLabel;

    bool m_use;

    public void Init(bool use, int indexX, int indexY)
    {
        m_use = use;
        gameObject.SetActive(use);        

        if(m_debugLabel)
        {
            m_debugLabel.text = indexX + "," + indexY;
        }
    }
}
