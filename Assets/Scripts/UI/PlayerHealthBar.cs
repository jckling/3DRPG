using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image healthSlider;
    public Image expSlider;

    #region Event Functions

    private void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playerStats.CurrentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    #endregion

    private void UpdateHealth()
    {
        healthSlider.fillAmount = (float)GameManager.Instance.playerStats.CurrentHealth /
                                  GameManager.Instance.playerStats.MaxHealth;
    }

    private void UpdateExp()
    {
        expSlider.fillAmount = (float)GameManager.Instance.playerStats.CurrentExp /
                               GameManager.Instance.playerStats.BaseExp;
    }
}