using UnityEngine;

public class BossDoorOpener : MonoBehaviour
{
    [Header("鍵1と鍵2")]
    public GameObject key1;
    public GameObject key2;

    [Header("ボス部屋の扉")]
    public GameObject bossDoor;

    private bool doorOpened = false;

    void Update()
    {
        if (doorOpened) return;

        // 鍵1と鍵2を両方取得（null）されたら扉を開く
        if (key1 == null && key2 == null)
        {
            bossDoor.SetActive(false);
            doorOpened = true;
        }
    }
}
