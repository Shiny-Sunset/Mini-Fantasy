using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float damage = 10f;
    protected HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public PlayerStatus playerStatus;
    public float healMP = 5f;
    public float healSP = 2.5f;

    public void SetDamage(float value)
    {
        damage = value;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!hitTargets.Contains(other.gameObject))
            {
                hitTargets.Add(other.gameObject);

                EnemyHealth eh = other.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.hitSound);
                    eh.TakeDamage(damage);
                    playerStatus.RecoverMP(healMP);
                    playerStatus.AddSP(healSP);
                }
            }
        }
    }

    public void ResetHitTargets()
    {
        hitTargets.Clear();
    }
}
