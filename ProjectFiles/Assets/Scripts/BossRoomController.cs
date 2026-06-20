using UnityEngine;

public class BossRoomController : MonoBehaviour
{
    [Header("鍵となるアイテム")]
    public GameObject keyItem;

    [Header("入口の壁")]
    public GameObject entranceWall;

    [Header("中ボス")]
    public GameObject midBoss;

    private bool bossSpawned = false;

    void Start()
    {
        entranceWall.SetActive(false);
        midBoss.SetActive(false);
    }

    void Update()
    {
        if (!bossSpawned && keyItem == null)
        {
            SpawnBossAndCloseDoor();
        }

        if (bossSpawned && midBoss == null)
        {
            Destroy(entranceWall);
        }
    }

    void SpawnBossAndCloseDoor()
    {
        entranceWall.SetActive(true);
        midBoss.SetActive(true);
        bossSpawned = true;
    }
}
