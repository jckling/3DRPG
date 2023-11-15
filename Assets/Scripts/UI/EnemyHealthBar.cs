using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform healthBarPoint;

    public bool alwaysVisible;
    public float visibleTime;
    private float visibleTimeLeft;

    private Image healthSlider;
    private Transform healthBarUI;
    private Transform cam;
    private CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBar += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                healthBarUI = Instantiate(healthBarPrefab, canvas.transform).transform;
                healthSlider = healthBarUI.GetChild(0).GetComponent<Image>();
                healthBarUI.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void LateUpdate()
    {
        if (healthBarUI == null) return;

        healthBarUI.position = healthBarPoint.position;
        healthBarUI.forward = -cam.forward;
        if (visibleTimeLeft <= 0 && !alwaysVisible)
        {
            healthBarUI.gameObject.SetActive(false);
        }
        else
        {
            visibleTimeLeft -= Time.deltaTime;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            Destroy(healthBarUI.gameObject);
        }

        healthBarUI.gameObject.SetActive(true);
        visibleTimeLeft = visibleTime;
        healthSlider.fillAmount = (float)currentHealth / maxHealth;
    }
}