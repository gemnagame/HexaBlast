using UnityEngine;

public class LobbyPage : MonoBehaviour
{
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
        Application.Quit();
    }
}
