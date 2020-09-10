using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField]
    Text m_resultText = null;

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

    public void OnClick_Restart()
    {
        GameManager.Instance?.GameRestart();

        Hide();
    }
}
