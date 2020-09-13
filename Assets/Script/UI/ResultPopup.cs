using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField]
    LobbyPage m_lobbyPage = null;

    [SerializeField]
    Text m_resultText = null;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        SoundManager.instance?.PlayAudio(SoundManager.AudioType.GAMEOVER);

        gameObject.SetActive(true);

        if (m_resultText)
        {
            m_resultText.text = Const.GAME_OVER_TEXT;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Restart()
    {
        SoundManager.instance?.PlayAudio(SoundManager.AudioType.BUTTONCLICK);

        if (GameManager.Instance)
        {
            bool success = GameManager.Instance.GameStart();
            if (success)
            {
                Hide();
            }
        }
    }

    public void OnClick_Exit()
    {
        SoundManager.instance?.PlayAudio(SoundManager.AudioType.BUTTONCLICK);

        m_lobbyPage?.Show();
        Hide();
    }
}
