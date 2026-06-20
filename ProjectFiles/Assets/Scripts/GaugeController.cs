using System.Collections;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    [SerializeField] private GameObject _gauge;
    [SerializeField] private GameObject _graceGauge;

    [System.NonSerialized] public float maxHP;
    [System.NonSerialized] public float currentHP;

    private float hpWidth;
    private float _waitingTime = 0.5f;
    private RectTransform _gaugeRect;
    private RectTransform _graceGaugeRect;

    void Start()
    {
        _gaugeRect = _gauge.GetComponent<RectTransform>();
        _graceGaugeRect = _graceGauge.GetComponent<RectTransform>();
        hpWidth = _gaugeRect.sizeDelta.x / maxHP;
        currentHP = maxHP;
    }

    public void BeInjured(float attack)
    {
        if (currentHP >= 0)
        {
            float damage = hpWidth * attack;
            StartCoroutine(DamageDecrease(damage));
        }
        currentHP = Mathf.Max(currentHP - attack, 0f);
    }

    IEnumerator DamageDecrease(float damage)
    {
        Vector2 newSize = _gaugeRect.sizeDelta;
        newSize.x -= damage;
        _gaugeRect.sizeDelta = newSize;

        yield return new WaitForSeconds(_waitingTime);
        _graceGaugeRect.sizeDelta = newSize;
    }
}
