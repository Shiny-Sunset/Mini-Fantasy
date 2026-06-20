using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 5f;
    public float dealMP = 0f;
    public float dealSP = 0f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(damage);
                playerStatus.LoseMP(dealMP);
                playerStatus.LoseSP(dealSP);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
