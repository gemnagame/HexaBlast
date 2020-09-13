using UnityEngine;

public class LobbyPage : MonoBehaviour
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
        if(GameManager.Instance)
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
        Application.Quit();
    }
}
