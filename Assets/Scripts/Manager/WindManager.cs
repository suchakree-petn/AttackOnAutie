using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class WindManager : Singleton<WindManager>
{
    [FoldoutGroup("UI References")]
    [SerializeField] private Image leftWindGauge,rightWindGauge,leftWindArrow,rightWindArrow;

    [Header("Wind Settings")]
    [SerializeField] float maxStrenth = 1f;

    [OnValueChanged(nameof(UpdateWindBar))]
    [Range(0f, 1f)]
    [SerializeField] float windStrength = 1f;
    public float WindStrength => windStrength;

    [OnValueChanged(nameof(UpdateWindBar))]
    [Range(-1, 1)]
    [SerializeField] int windDirection = 0;
    public int WindDirection => windDirection;


    protected override void InitAfterAwake()
    {
    }

    public float GetWindMultiplier()
    {
        return WindDirection * WindStrength;
    }
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
        if (leftWindGauge == null || rightWindGauge == null) return;

        Image windFill,windArrow;

        switch (windDirection)
        {
            case -1:
                windFill = leftWindGauge;
                windArrow = leftWindArrow;
                rightWindGauge.gameObject.SetActive(false);
                rightWindArrow.gameObject.SetActive(false);
                break;
            case 1:
                windFill = rightWindGauge;
                windArrow = rightWindArrow;
                leftWindGauge.gameObject.SetActive(false);
                leftWindArrow.gameObject.SetActive(false);

                break;
            default:
                leftWindGauge.gameObject.SetActive(false);
                rightWindGauge.gameObject.SetActive(false);
                rightWindArrow.gameObject.SetActive(false);
                leftWindArrow.gameObject.SetActive(false);
                return;
        }
        windFill.gameObject.SetActive(true);
        windArrow.gameObject.SetActive(true);
        windFill.fillAmount = windStrength;
    }
}
