using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField]
    Text m_scoreText = null;

    public void SetScore(int count)
    {
        if (m_scoreText == null)
        {
            return;
        }

        m_scoreText.text = count.ToString();
    }
}
