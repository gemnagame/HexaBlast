using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField]
    Text m_scoreText = null;
    [SerializeField]
    Image m_timerGaugeImage = null;

    public void SetScore(int count)
    {
        if (m_scoreText == null)
        {
            return;
        }

        m_scoreText.text = count.ToString();
    }

    public void SetTimerGauge(float time)
    {
        if(m_timerGaugeImage)
        {
            m_timerGaugeImage.fillAmount = time / Const.GAME_OVER_CHECK_TIME;
        }
    }
}
