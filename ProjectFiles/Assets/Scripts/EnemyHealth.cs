using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("体力(HP)")]
    [Range(0.1f, 9999f)]
    public float maxHP = 100f;
    private float currentHP;
    private EnemyAI enemyAI;
    [Header("体力バー(UI)")]
    public GaugeController gaugeController;
    [Header("ダメージ表示用テキストプレハブ(UI)")]
    public GameObject damagePopupPrefab;
    private bool isDead = false;
    private Transform canvasTransform;

    void Awake()
    {
        gaugeController.maxHP = maxHP;
        gaugeController.currentHP = maxHP;
    }

    void Start()
    {
        currentHP = maxHP;
        enemyAI = GetComponent<EnemyAI>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
            canvasTransform = canvas.transform;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (!isDead)
        {
            ShowDamage(amount);
            gaugeController.BeInjured(amount);
        }

        if (currentHP <= 0)
        {
            isDead = true;
            enemyAI.Die();
        }
        else
        {
            enemyAI.TakeDamage();
        }
    }

    void ShowDamage(float amount)
    {
        if (canvasTransform == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.25f);
        GameObject popup = Instantiate(damagePopupPrefab, screenPos, Quaternion.identity, canvasTransform);
        popup.GetComponent<DamagePopup>().Setup(amount);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
