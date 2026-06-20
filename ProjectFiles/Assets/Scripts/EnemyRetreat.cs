using UnityEngine;
using UnityEngine.AI;

public class EnemyRetreat : MonoBehaviour
{
    public Transform player;
    public float retreatDistance = 2.0f;
    public float moveBackDistance = 1.5f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = retreatDistance;
    }

    void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= agent.stoppingDistance)
        {
            // 攻撃処理をここに書く
        }
    }
}
