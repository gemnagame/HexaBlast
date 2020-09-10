using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Start()
    {
        GameManager.Instance?.GameStart();
        Hide();
    }

    public void OnClick_Quit()
    {
        GameManager.Instance?.GameQuit();
    }
}
