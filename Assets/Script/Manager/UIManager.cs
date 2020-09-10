using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    IngameUI m_ingameUI = null;      //인게임 UI
    [SerializeField]
    ResultPopup m_resultPopup = null;//게임 결과 팝업(게임 클리어/게임 오버)

    private void Awake()
    {
        Instance = this;

        HideResultPopup();
    }

    public void SetRemovedTopCountText(int count)
    {
        m_ingameUI?.SetRemovedTopCountText(count);
    }

    public void SetMoveLimitCountText(int count)
    {
        m_ingameUI?.SetMoveLimitCountText(count);
    }

    public void HideResultPopup()
    {
        m_resultPopup?.Hide();
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
