using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class WindBarController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image leftWind;
    [SerializeField] private Image rightWind;

    [Header("Wind Settings")]
    [SerializeField] float maxStrenth = 1f;
    [OnValueChanged(nameof(UpdateWindBar))]
    [Range(0f, 1f)]
    [SerializeField] float windStrength = 1f;

    [OnValueChanged(nameof(UpdateWindBar))]
    [Range(-1, 1)]
    [SerializeField] int windDirection = 0;


    [Button]
    public void RandomWind()
    {
        UpdateWind(Random.Range(0f, maxStrenth), Random.Range(-1, 2));
    }

    public void UpdateWind(float strength, int direction)
    {
        windStrength = Mathf.Clamp01(strength);
        windDirection = Mathf.Clamp(direction, -1, 1);
        UpdateWindBar();
    }

    void UpdateWindBar()
    {
        if (leftWind == null || rightWind == null) return;

        Image windFill;

        switch (windDirection)
        {
            case -1:
                windFill = leftWind;
                rightWind.gameObject.SetActive(false);
                break;
            case 1:
                windFill = rightWind;
                leftWind.gameObject.SetActive(false);
                break;
            default:
                leftWind.gameObject.SetActive(false);
                rightWind.gameObject.SetActive(false);
                return;
        }
        windFill.gameObject.SetActive(true);

        windFill.fillAmount = windStrength;
    }
}
