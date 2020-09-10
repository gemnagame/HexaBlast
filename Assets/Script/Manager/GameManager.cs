using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    Ingame m_ingame = null;
    [SerializeField]
    LobbyUI m_lobbyUI = null;
    [SerializeField]
    IngameUI m_ingameUI = null;      //인게임 UI
    [SerializeField]
    ResultPopup m_resultPopup = null;//게임 결과 팝업(게임 클리어/게임 오버)

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_lobbyUI?.Show();
        m_resultPopup?.Hide();
    }

    public void GameStart()
    {
        m_ingame?.GameStart();
    }

    public void GameRestart()
    {
        m_ingame?.GameRestart();
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void SetRemovedTopCountText(int count)
    {
        m_ingameUI?.SetRemovedTopCountText(count);
    }

    public void SetMoveLimitCountText(int count)
    {
        m_ingameUI?.SetMoveLimitCountText(count);
    }

    public void ShowResultPopup(GameResult gameResult)
    {
        if (gameResult == GameResult.GAME_CLEAR)
        {
            m_resultPopup?.Show(Const.GAME_CLEAR_TEXT);
        }
        else if (gameResult == GameResult.GAME_OVER)
        {
            m_resultPopup?.Show(Const.GAME_OVER_TEXT);
        }
    }
}
