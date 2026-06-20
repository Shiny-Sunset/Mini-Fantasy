using UnityEngine;

public class BossEnemyAI : EnemyAI
{
    [Header("攻撃のヒットボックス")]
    public GameObject rightHitboxObject;
    public GameObject leftHitboxObject;
    [Header("近距離攻撃のクールタイム")]
    [Range(0.1f, 10f)]
    public float attackCooldown = 5f;
    [Header("攻撃可能な距離")]
    [Range(0.1f, 10f)]
    public float meleeAttackRange = 3f;
    [Range(0.1f, 50f)]
    public float rangedAttackRange = 10f;
    [Header("攻撃ダメージ")]
    [Range(0.1f, 9999f)]
    public float attackDamage = 10f;

    [Header("弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("弾を出す座標")]
    public Transform firePoint1;
    public Transform firePoint2;
    public Transform firePoint3;
    public Transform firePoint4;
    [Header("リングのヒットボックス")]
    public GameObject ring1HitboxObject;
    public GameObject ring2HitboxObject;
    public GameObject ring3HitboxObject;
    public GameObject ring4HitboxObject;
    [Header("弾の攻撃ダメージ")]
    [Range(0.1f, 9999f)]
    public float bulletAttackDamage = 5f;
    [Header("遠距離攻撃のクールタイム")]
    [Range(0.1f, 10f)]
    public float fireCooldown = 2f;
    [Header("弾速")]
    [Range(0.1f, 50f)]
    public float bulletSpeed = 10f;

    public GameManager gameManager;

    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private EnemyAttackHitbox enemyAttackRightHitbox;
    private EnemyAttackHitbox enemyAttackLeftHitbox;
    private EnemyAttackHitbox enemyAttackRing1Hitbox;
    private EnemyAttackHitbox enemyAttackRing2Hitbox;
    private EnemyAttackHitbox enemyAttackRing3Hitbox;
    private EnemyAttackHitbox enemyAttackRing4Hitbox;

    protected override void Start()
    {
        base.Start();

        rightHitboxObject.SetActive(false);
        leftHitboxObject.SetActive(false);
        ring1HitboxObject.SetActive(false);
        ring2HitboxObject.SetActive(false);
        ring3HitboxObject.SetActive(false);
        ring4HitboxObject.SetActive(false);

        enemyAttackRightHitbox = rightHitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackRightHitbox.SetDamage(attackDamage);
        enemyAttackRightHitbox.dealMP = dealMP;
        enemyAttackRightHitbox.dealSP = dealSP;

        enemyAttackLeftHitbox = leftHitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackLeftHitbox.SetDamage(attackDamage);
        enemyAttackLeftHitbox.dealMP = dealMP;
        enemyAttackLeftHitbox.dealSP = dealSP;

        float ringDamage = attackDamage / 2f;
        float ringDealMP = dealMP / 2f;
        float ringDealSP = dealSP / 2f;

        enemyAttackRing1Hitbox = ring1HitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackRing1Hitbox.SetDamage(ringDamage);
        enemyAttackRing1Hitbox.dealMP = ringDealMP;
        enemyAttackRing1Hitbox.dealSP = ringDealSP;

        enemyAttackRing2Hitbox = ring2HitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackRing2Hitbox.SetDamage(ringDamage);
        enemyAttackRing2Hitbox.dealMP = ringDealMP;
        enemyAttackRing2Hitbox.dealSP = ringDealSP;

        enemyAttackRing3Hitbox = ring3HitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackRing3Hitbox.SetDamage(ringDamage);
        enemyAttackRing3Hitbox.dealMP = ringDealMP;
        enemyAttackRing3Hitbox.dealSP = ringDealSP;

        enemyAttackRing4Hitbox = ring4HitboxObject.GetComponent<EnemyAttackHitbox>();
        enemyAttackRing4Hitbox.SetDamage(ringDamage);
        enemyAttackRing4Hitbox.dealMP = ringDealMP;
        enemyAttackRing4Hitbox.dealSP = ringDealSP;

        enemyManager.BossEnemy();
    }

    protected override void Update()
    {
        if (!player || isDead) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttack4Playing = stateInfo.IsName("BossEnemy_Attack4");

        if (distance <= detectionRange && (!isAttacking || isAttack4Playing))
        {
            if (!isAttack4Playing && Time.time - lastAttackTime >= attackCooldown)
            {
                TryStartAttack(distance);
            }
            else if (!isAttack4Playing)
            {
                agent.SetDestination(player.transform.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }

            FacePlayer();
        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetFloat("Speed", 0f);
        }
    }

    void TryStartAttack(float distance)
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        agent.isStopped = true;

        int attackIndex;

        if (distance <= meleeAttackRange)
        {
            attackIndex = Random.Range(1, 3); // Attack1 or Attack2（近距離）
        }
        else if (distance <= rangedAttackRange)
        {
            attackIndex = Random.Range(3, 5); // Attack3 or Attack4（遠距離）
        }
        else
        {
            isAttacking = false;
            agent.isStopped = false;
            return;
        }

        animator.SetTrigger($"Attack{attackIndex}");
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (agent.isOnNavMesh)
            agent.isStopped = false;
    }

    public void EnableRightHitbox()  { rightHitboxObject.SetActive(true); }
    public void DisableRightHitbox() { enemyAttackRightHitbox.ResetHitTargets(); rightHitboxObject.SetActive(false); }

    public void EnableLeftHitbox()   { leftHitboxObject.SetActive(true); }
    public void DisableLeftHitbox()  { enemyAttackLeftHitbox.ResetHitTargets(); leftHitboxObject.SetActive(false); }

    public void EnableAllRingHitbox()
    {
        ring1HitboxObject.SetActive(true);
        ring2HitboxObject.SetActive(true);
        ring3HitboxObject.SetActive(true);
        ring4HitboxObject.SetActive(true);
    }

    public void DisableAllRingHitbox()
    {
        enemyAttackRing1Hitbox.ResetHitTargets(); ring1HitboxObject.SetActive(false);
        enemyAttackRing2Hitbox.ResetHitTargets(); ring2HitboxObject.SetActive(false);
        enemyAttackRing3Hitbox.ResetHitTargets(); ring3HitboxObject.SetActive(false);
        enemyAttackRing4Hitbox.ResetHitTargets(); ring4HitboxObject.SetActive(false);
    }

    public void EnableRing1Hitbox()  { ring1HitboxObject.SetActive(true); }
    public void DisableRing1Hitbox() { enemyAttackRing1Hitbox.ResetHitTargets(); ring1HitboxObject.SetActive(false); }

    public void EnableRing2Hitbox()  { ring2HitboxObject.SetActive(true); }
    public void DisableRing2Hitbox() { enemyAttackRing2Hitbox.ResetHitTargets(); ring2HitboxObject.SetActive(false); }

    public void EnableRing3Hitbox()  { ring3HitboxObject.SetActive(true); }
    public void DisableRing3Hitbox() { enemyAttackRing3Hitbox.ResetHitTargets(); ring3HitboxObject.SetActive(false); }

    public void EnableRing4Hitbox()  { ring4HitboxObject.SetActive(true); }
    public void DisableRing4Hitbox() { enemyAttackRing4Hitbox.ResetHitTargets(); ring4HitboxObject.SetActive(false); }

    void Shoot(int point)
    {
        Transform firePoint = point switch
        {
            1 => firePoint1,
            2 => firePoint2,
            3 => firePoint3,
            4 => firePoint4,
            _ => null
        };
        if (firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = firePoint.up * bulletSpeed;

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
        enemyBullet.damage = bulletAttackDamage;
        enemyBullet.dealMP = dealMP;
        enemyBullet.dealSP = dealSP;
    }

    void AllShoot()
    {
        for (int i = 1; i <= 4; i++)
            Shoot(i);
    }

    void Clear()
    {
        gameManager.ShowGameClearUI();
        OnDeathAnimationEnd();
    }
}
