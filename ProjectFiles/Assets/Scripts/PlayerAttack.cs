using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻撃のクールタイム")]
    [Range(0.1f, 10f)]
    public float attackCooldown = 0.5f;
    [Header("攻撃ダメージ")]
    [Range(0.1f, 9999f)]
    public float attackDamage = 10f;

    public Slider attackDamageSlider;
    private float previousValue;
    [Header("通常攻撃で回復するMPの量")]
    [Range(0.1f, 100f)]
    public float healMP = 5f;
    [Header("攻撃のヒットボックス")]
    public GameObject hitboxObject;
    private AttackHitbox attackHitbox;
    private PlayerStatus playerStatus;
    private Animator animator;
    private bool canAttack = true;
    private PlayerSkills playerSkills;

    void Start()
    {
        if (attackDamageSlider != null)
        {
            attackDamageSlider.value = attackDamage;
            previousValue = attackDamage;
        }
        hitboxObject.SetActive(false);
        playerStatus = GetComponent<PlayerStatus>();
        attackHitbox = hitboxObject.GetComponent<AttackHitbox>();
        attackHitbox.SetDamage(attackDamage);
        attackHitbox.playerStatus = playerStatus;
        attackHitbox.healMP = healMP;
        animator = GetComponent<Animator>();
        playerSkills = GetComponent<PlayerSkills>();
    }

    void Update()
    {
        if (attackDamageSlider != null && attackDamageSlider.value != previousValue)
        {
            attackDamage = attackDamageSlider.value;
            attackHitbox.SetDamage(attackDamage);
            previousValue = attackDamageSlider.value;
        }
        if (Input.GetMouseButtonDown(0) && canAttack && !playerSkills.isBlocking)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    // アニメーションイベントから呼ばれる
    public void EnableHitbox(float damageRate)
    {
        attackHitbox.SetDamage(attackDamage * damageRate);
        hitboxObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        attackHitbox.ResetHitTargets();
        hitboxObject.SetActive(false);
    }
}
