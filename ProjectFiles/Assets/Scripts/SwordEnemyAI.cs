using UnityEngine;

public class SwordEnemyAI : EnemyAI
{
    [Header("攻撃のヒットボックス")]
    public GameObject hitboxObject;
    [Header("攻撃を発動する距離")]
    [Range(0.1f, 10f)]
    public float attackRange = 1.8f;
    [Header("攻撃のクールタイム")]
    [Range(0.1f, 10f)]
    public float attackCooldown = 2f;
    [Header("攻撃ダメージ")]
    [Range(0.1f, 9999f)]
    public float attackDamage = 10f;

    private float lastAttackTime = -999f;
    private EnemyAttackHitbox enemyAttackHitbox;

    protected override void Start()
    {
        base.Start();
        hitboxObject.SetActive(false);
        enemyAttackHitbox = hitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackHitbox.SetDamage(attackDamage);
        enemyAttackHitbox.dealMP = dealMP;
        enemyAttackHitbox.dealSP = dealSP;
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    public void EnableHitbox()
    {
        hitboxObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        enemyAttackHitbox.ResetHitTargets();
        hitboxObject.SetActive(false);
    }
}
