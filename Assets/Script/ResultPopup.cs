using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    public Text m_resultText;

    public void Show(string result)
    {
        if(m_resultText == null)
        {
            return;
        }

        gameObject.SetActive(true);

        m_resultText.text = result;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void On_Restart()
    {
        if(IngameManager.Instance.TouchBlocked() == false)
        {
            IngameManager.Instance.On_Restart();

            Hide();
        }
    }
}
