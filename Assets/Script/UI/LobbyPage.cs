using UnityEngine;

public class LobbyPage : MonoBehaviour
{
    [SerializeField]
    Ingame m_ingame = null;

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Start()
    {
        m_ingame?.GameStart();
        Hide();
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }
}
