using UnityEngine;

public class LobbyPage : MonoBehaviour
{
    private void Start()
    {
        Show();
    }

    public void Show()
    {
        SoundManager.instance?.Play_BGM(SoundManager.BGMType.LOBBY);

        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Start()
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

    public void OnClick_Quit()
    {
        SoundManager.instance?.PlayAudio(SoundManager.AudioType.BUTTONCLICK);

        Application.Quit();
    }
}
