using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [Header("体力(HP)")]
    [Range(0.1f, 9999f)]
    public float maxHP = 100f;
    [System.NonSerialized]
    public float currentHP;
    [Header("体力バー(UI)")]
    public GaugeControllerGeneral hpGauge;
    public GameManager gameManager;
    private Animator animator;

    [Header("MP（魔法ポイント）")]
    public float maxMP = 100f;
    [Header("MPバー(UI)")]
    public GaugeControllerGeneral mpGauge;
    [System.NonSerialized]
    public float currentMP = 0f;

    [Header("SP（スペシャルポイント）")]
    public float maxSP = 100f;
    [Header("SPバー(UI)")]
    public GaugeControllerGeneral spGauge;
    [System.NonSerialized]
    public float currentSP = 0f;

    [Header("ステータス回復速度")]
    public float mpRegenRate = 5f;
    public float spRegenRate = 1f;

    [Header("防御率")]
    [Range(0f, 1f)]
    public float blockRate = 1f;

    private PlayerSkills playerSkills;

    public Slider hpSlider;
    private float previousHpSliderValue;

    private PlayerMovement playerMovement;

    void Awake()
    {
        if (hpSlider != null)
        {
            hpSlider.value = maxHP;
            previousHpSliderValue = maxHP;
        }

        hpGauge.maxValue = maxHP;
        mpGauge.maxValue = maxMP;
        spGauge.maxValue = maxSP;
        hpGauge.calculateWidth();
        mpGauge.calculateWidth();
        spGauge.calculateWidth();
    }

    void Start()
    {
        currentHP = maxHP;
        hpGauge.currentValue = currentHP;
        mpGauge.currentValue = currentMP;
        spGauge.currentValue = currentSP;

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerSkills = GetComponent<PlayerSkills>();
    }

    void Update()
    {
        if (hpSlider != null && hpSlider.value != previousHpSliderValue)
        {
            SetMaxHp();
            previousHpSliderValue = hpSlider.value;
        }

        RecoverMP(mpRegenRate * Time.deltaTime);
        AddSP(spRegenRate * Time.deltaTime);

        currentMP = mpGauge.currentValue;
        currentSP = spGauge.currentValue;
        currentHP = hpGauge.currentValue;
    }

    public bool CanUseMP(float amount) => mpGauge.currentValue >= amount;

    public void UseMP(float amount)  => mpGauge.Subtract(amount);
    public void LoseMP(float amount) => UseMP(amount);

    public void RecoverMP(float amount) => mpGauge.Add(amount);

    public void AddSP(float amount)  => spGauge.Add(amount);
    public void LoseSP(float amount) => spGauge.Subtract(amount);

    public void TakeDamage(float amount)
    {
        float actualDamage = amount * blockRate;
        hpGauge.Subtract(actualDamage);

        if (playerSkills.isBlocking)
            AudioManager.Instance.PlaySE(AudioManager.Instance.guardHitSound);
        else
            AudioManager.Instance.PlaySE(AudioManager.Instance.playerHitSound);

        if (hpGauge.currentValue <= 0)
            Die();
    }

    public void HealHP(float amount) => hpGauge.Add(amount);

    public bool UseSP()
    {
        if (spGauge.currentValue >= spGauge.maxValue)
        {
            spGauge.Subtract(spGauge.maxValue);
            return true;
        }
        return false;
    }

    public bool FullHP() => currentHP >= maxHP;

    void Die()
    {
        animator.SetTrigger("Down");
        playerMovement.isDead = true;
        animator.SetFloat("Speed", 0f);
    }

    // アニメーションイベントから呼ぶ（Down アニメーション終了時）
    public void OnDeathAnimationEnd()
    {
        gameManager.ShowGameOverUI();
    }

    private void SetMaxHp()
    {
        maxHP = hpSlider.value;
        hpGauge.maxValue = maxHP;
        hpGauge.calculateWidth();
        currentHP = maxHP;
        hpGauge.currentValue = currentHP;
    }
}
