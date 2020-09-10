using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField]
    Text m_clearConditionText = null;//게임 클리어 조건 텍스트(예:팽이 12개 제거)
    [SerializeField]
    Text m_moveLimitCountText = null;//이동 횟수 제한 텍스트

    public void SetRemovedTopCountText(int count)
    {
        if (m_clearConditionText == null)
        {
            return;
        }

        m_clearConditionText.text = count.ToString();
    }

    public void SetMoveLimitCountText(int count)
    {
        if (m_moveLimitCountText == null)
        {
            return;
        }

        m_moveLimitCountText.text = count.ToString();
    }

    public void On_Restart()
    {
        IngameManager.Instance?.Restart();        
    }
}
