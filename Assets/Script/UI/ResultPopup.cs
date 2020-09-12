using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField]
    Text m_resultText = null;

    public void Show()
    {
        if(m_resultText == null)
        {
            return;
        }

        gameObject.SetActive(true);

        m_resultText.text = Const.GAME_OVER_TEXT;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Restart()
    {
        bool success = GameManager.Instance.GameRestart();

        if(success)
        {
            Hide();
        }
    }
}
