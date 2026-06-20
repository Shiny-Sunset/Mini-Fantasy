using System.Collections;
using UnityEngine;

public class GaugeControllerGeneral : MonoBehaviour
{
    [SerializeField] private GameObject _gauge;
    [SerializeField] private GameObject _graceGauge;

    [System.NonSerialized] public float maxValue = 100f;
    [System.NonSerialized] public float currentValue;

    private float valueWidth;
    private float _waitingTime = 0.5f;
    private RectTransform _gaugeRect;
    private RectTransform _graceGaugeRect;

    public void calculateWidth()
    {
        _gaugeRect = _gauge.GetComponent<RectTransform>();
        if (_graceGauge != null)
            _graceGaugeRect = _graceGauge.GetComponent<RectTransform>();

        valueWidth = _gaugeRect.sizeDelta.x / maxValue;
        currentValue = maxValue;
        UpdateGaugeImmediate();
    }

    public void Add(float amount)
    {
        currentValue = Mathf.Min(currentValue + amount, maxValue);
        UpdateGaugeImmediate();
    }

    public void Subtract(float amount)
    {
        if (currentValue <= 0) return;

        float delta = valueWidth * amount;
        currentValue = Mathf.Max(currentValue - amount, 0f);
        StartCoroutine(SmoothDecrease(delta));
    }

    public void UpdateGaugeImmediate()
    {
        Vector2 newSize = _gaugeRect.sizeDelta;
        newSize.x = valueWidth * currentValue;
        _gaugeRect.sizeDelta = newSize;

        if (_graceGaugeRect != null)
            _graceGaugeRect.sizeDelta = newSize;
    }

    IEnumerator SmoothDecrease(float delta)
    {
        Vector2 nowSize = _gaugeRect.sizeDelta;
        nowSize.x -= delta;
        _gaugeRect.sizeDelta = nowSize;

        yield return new WaitForSeconds(_waitingTime);
        if (_graceGaugeRect != null)
            _graceGaugeRect.sizeDelta = nowSize;
    }
}
