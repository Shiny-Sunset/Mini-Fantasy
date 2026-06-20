using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum PatrolType { Loop, Random }

    [Header("索敵範囲")]
    [Range(0.1f, 100f)]
    public float detectionRange = 10f;

    [Header("通常時の移動速度")]
    [Range(0.1f, 50f)]
    public float normalSpeed = 3.5f;

    [Header("攻撃時に減らすMP")]
    [Range(0f, 50f)]
    public float dealMP = 0f;

    [Header("攻撃時に減らすSP")]
    [Range(0f, 50f)]
    public float dealSP = 0f;

    [Header("巡回ポイント")]
    public PatrolRoute patrolRoute;
    private Transform[] patrolPoints;
    public bool useRandomPatrol = false;

    [Header("追跡を諦めて戻るまでの時間(秒)")]
    public float lostSightTime = 5f;

    [System.NonSerialized]
    public EnemyManager enemyManager;

    protected NavMeshAgent agent;
    protected GameObject player;
    protected Animator animator;
    protected bool isDead = false;

    protected int currentPatrolIndex = 0;
    protected bool isChasing = false;
    protected float lastSeenTime = Mathf.NegativeInfinity;

    protected virtual void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        agent.speed = normalSpeed;
        enemyManager.RegisterEnemy();

        if (patrolRoute != null)
        {
            patrolPoints = patrolRoute.GetPoints();
            if (patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    protected virtual void Update()
    {
        if (!isDead)
            UpdateAI();
    }

    protected virtual void UpdateAI()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.transform.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            lastSeenTime = Time.time;
            FacePlayer();
        }
        else
        {
            if (Time.time - lastSeenTime < lostSightTime)
            {
                agent.SetDestination(player.transform.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
                FacePlayer();
            }
            else
            {
                isChasing = false;
                Patrol();
                animator.SetFloat("Speed", agent.velocity.magnitude);

                if (agent.velocity.sqrMagnitude > 0.01f)
                    FaceTarget(agent.destination);
            }
        }
    }

    protected void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (useRandomPatrol)
            {
                int nextIndex;
                do
                {
                    nextIndex = Random.Range(0, patrolPoints.Length);
                } while (nextIndex == currentPatrolIndex && patrolPoints.Length > 1);
                currentPatrolIndex = nextIndex;
            }
            else
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }

            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    protected void FacePlayer()
    {
        FaceTarget(player.transform.position);
    }

    protected void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        animator.SetTrigger("TakeDamage");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        this.enabled = false;
    }

    public void OnDeathAnimationEnd()
    {
        enemyManager.EnemyDefeated();
        Destroy(gameObject);
    }
}
