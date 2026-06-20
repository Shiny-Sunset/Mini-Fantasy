using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("ボス部屋のドア（壁）")]
    public GameObject bossDoor;
    private int enemyCount = 0;

    void Start()
    {
        if (bossDoor != null)
            bossDoor.SetActive(true);
    }

    public void RegisterEnemy()
    {
        enemyCount++;
    }

    public void BossEnemy()
    {
        enemyCount--;
    }

    public void EnemyDefeated()
    {
        enemyCount--;
        if (enemyCount <= 0)
            OpenBossDoor();
    }

    void OpenBossDoor()
    {
        if (bossDoor != null)
        {
            //bossDoor.SetActive(false);
            Debug.Log("全ての敵を倒したのでボスドアが開いた！");
        }
    }
}
