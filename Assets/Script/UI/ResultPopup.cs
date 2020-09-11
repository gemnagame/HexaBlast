using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField]
    Ingame m_ingame = null;

    [SerializeField]
    Text m_resultText = null;

    public void Show(GameResult gameResult)
    {
        if(m_resultText == null)
        {
            return;
        }

        gameObject.SetActive(true);

        string resultText = string.Empty;
        if (gameResult == GameResult.GAME_CLEAR)
        {
            resultText = Const.GAME_CLEAR_TEXT;
        }
        else if (gameResult == GameResult.GAME_OVER)
        {
            resultText = Const.GAME_OVER_TEXT;
        }

        m_resultText.text = resultText;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Restart()
    {
        bool success = m_ingame.GameRestart();

        if(success)
        {
            Hide();
        }
    }
}
