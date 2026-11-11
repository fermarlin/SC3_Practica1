
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health generatorHealth;

    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        if (generatorHealth == null)
            return;

        generatorHealth.OnHealthChanged += UpdateHealthUI;

        RefreshUI();
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnDisable()
    {
        if (generatorHealth != null)
        {
            generatorHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void RefreshUI()
    {
        if (generatorHealth == null)
            return;

        UpdateHealthUI(
            generatorHealth.CurrentHealth,
            generatorHealth.MaxHealth
        );
    }

    private void UpdateHealthUI(int current, int max)
    {
        float healthPercentage =
            max > 0 ? (float)current / max : 0f;

        if (healthFill != null)
        {
            healthFill.fillAmount =
                Mathf.Clamp01(healthPercentage);
        }

        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }
}
