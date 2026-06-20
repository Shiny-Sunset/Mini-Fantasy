using UnityEngine;

public class GunEnemyAI : EnemyAI
{
    [Header("弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("弾を出す座標")]
    public Transform firePoint;
    [Header("弾の攻撃ダメージ")]
    [Range(0.1f, 9999f)]
    public float attackDamage = 5f;
    [Header("攻撃のクールタイム")]
    [Range(0.1f, 10f)]
    public float fireCooldown = 2f;
    [Header("弾速")]
    [Range(0.1f, 50f)]
    public float bulletSpeed = 10f;
    [Space(10)]
    [Header("逃げる時の速度")]
    [Range(0.1f, 10f)]
    public float retreatSpeed = 3.5f;
    [Header("近づかれたら逃げる距離")]
    [Tooltip("索敵範囲>離れすぎたら追いかける距離>近づかれたら逃げる距離")]
    [Range(0.1f, 100f)]
    public float retreatThreshold = 5f;      // 近づかれたら逃げる距離
    [Header("離れすぎたら追いかける距離")]
    [Tooltip("索敵範囲>離れすぎたら追いかける距離>近づかれたら逃げる距離")]
    [Range(0.1f, 100f)]
    public float farChaseDistance = 20f;     // 離れすぎたら追いかける距離

    private float lastFireTime = -999f;

    protected override void UpdateAI()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange)
        {
            if (distance < retreatThreshold)
            {
                RetreatFromPlayer();
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            else if (distance > farChaseDistance)
            {
                agent.SetDestination(player.transform.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            else
            {
                agent.SetDestination(transform.position);
                animator.SetFloat("Speed", 0f);
                TryShoot();
            }

            lastSeenTime = Time.time;
            if (!isDead)
            {
                FacePlayer();
            }
        }
        else
        {
            if (Time.time - lastSeenTime < lostSightTime)
            {
                agent.SetDestination(player.transform.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            else
            {
                // 巡回モード
                isChasing = false;
                Patrol();
                animator.SetFloat("Speed", agent.velocity.magnitude);

                if (agent.velocity.sqrMagnitude > 0.01f)
                {
                    FaceTarget(agent.destination);
                }
            }
        }

    }



    void RetreatFromPlayer()
    {
        agent.speed = retreatSpeed;
        Vector3 retreatDir = (transform.position - player.transform.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDir * 5f;

        // NavMesh上の有効な位置を探す
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(retreatTarget, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // NavMesh上に見つからなかった場合はそのまま使う
            agent.SetDestination(retreatTarget);
        }
    }

    void TryShoot()
    {
        if (Time.time - lastFireTime >= fireCooldown)
        {
            animator.SetTrigger("Shoot");
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.useGravity = false;

        // プレイヤーの中心座標へ向けて方向を補正
        Vector3 targetPos = player.transform.position + Vector3.up * 0.4f; // プレイヤーの少し上（胸くらい）
        Vector3 direction = (targetPos - firePoint.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
        enemyBullet.damage = attackDamage;
        enemyBullet.dealMP = dealMP;
        enemyBullet.dealSP = dealSP;
    }

}
