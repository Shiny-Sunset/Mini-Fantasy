using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    public GameObject enemy;
    public void OnDeathAnimationEnd()
    {
        Destroy(enemy);
    }
}
