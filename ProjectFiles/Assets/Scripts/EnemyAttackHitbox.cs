using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private float damage = 10;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public float dealMP = 2f;
    public float dealSP = 0f;

    public void SetDamage(float value)
    {
        damage = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hitTargets.Contains(other.gameObject))
            {
                hitTargets.Add(other.gameObject);

                PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    playerStatus.TakeDamage(damage);
                    playerStatus.LoseMP(dealMP);
                    playerStatus.LoseSP(dealSP);
                }
            }
        }
    }

    // アニメーションイベントから呼ぶ（攻撃終了時）
    public void ResetHitTargets()
    {
        hitTargets.Clear();
    }
}
