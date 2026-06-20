using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class LockOnManager : MonoBehaviour
{
    public float lockOnRadius = 20f;
    public KeyCode lockOnKey = KeyCode.Tab;
    public PlayerMovement playerMovement;
    public Transform player;
    public GameObject targetMarkerPrefab; // 照準マーカーのPrefab
    public GameObject lockOnCamera;
    public GameObject freeLookCamera;


    private List<Transform> lockOnTargets = new List<Transform>();
    private int currentIndex = -1; // -1 = ロックオンなし
    [System.NonSerialized]
    public Transform currentTarget;
    private GameObject currentMarker;
    private Transform currentTargetPoint;
    private Transform lockOnCameraPos;
    private Transform cameraPos;
    private Transform currentCamera;
    private CinemachineLockOnCamera cinemachineLockOnCamera;

    void Start()
    {
        cinemachineLockOnCamera = lockOnCamera.GetComponent<CinemachineLockOnCamera>();
        lockOnCameraPos = lockOnCamera.GetComponent<Transform>();
        cameraPos = freeLookCamera.GetComponent<Transform>();
        ClearLockOn();
    }

    void Update()
    {
        if (Input.GetKeyDown(lockOnKey))
        {
            CycleLockOnTarget();
        }

        // ロックオンしている敵が死んだらロックオン解除
        if (currentTarget != null)
        {
            var enemyHealth = currentTarget.GetComponent<EnemyHealth>();
            if (enemyHealth == null || enemyHealth.IsDead())
            {
                ClearLockOn();
            }
        }

    }

    void CycleLockOnTarget()
    {
        // ロックオン対象候補を取得
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        lockOnTargets = enemies
            .Select(e => e.transform)
            .Where(t =>
            {
                float distance = Vector3.Distance(player.position, t.position);
                if (distance > lockOnRadius) return false;

                var enemyHealth = t.GetComponent<EnemyHealth>();
                return enemyHealth != null && !enemyHealth.IsDead(); // 死んでない敵のみ
            })
            .OrderBy(t => Vector3.Distance(player.position, t.position))
            .ToList();

        if (lockOnTargets.Count == 0)
        {
            ClearLockOn();
            return;
        }

        currentIndex++;

        if (currentIndex >= lockOnTargets.Count)
        {
            currentIndex = -1;
            ClearLockOn();
        }
        else
        {
            LockOn(lockOnTargets[currentIndex]);
        }
    }

    public void LockOn(Transform enemy)
    {
        currentTarget = enemy;
        cinemachineLockOnCamera.SetTarget(enemy);
        freeLookCamera.SetActive(false);
        lockOnCamera.SetActive(true); // LockOnCam を有効化
        playerMovement.cameraTransform = lockOnCameraPos;
        currentCamera = lockOnCameraPos;

        // 照準マーカーの初期化
        if (targetMarkerPrefab != null)
        {
            if (currentMarker != null)
                Destroy(currentMarker);

            // TargetMarkerPoint を探して子としてマーカーを生成
            Transform markerPoint = enemy.Find("EnemyCanvas/TargetMarkerPoint");
            if (markerPoint != null)
            {
                currentMarker = Instantiate(targetMarkerPrefab, markerPoint);
                currentMarker.transform.localPosition = Vector3.zero;
                currentMarker.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("TargetMarkerPoint が見つかりませんでした。");
            }
        }
    }

    public void ClearLockOn()
    {
        currentTarget = null;
        cinemachineLockOnCamera.ClearTarget();
        lockOnCamera.SetActive(false);
        freeLookCamera.SetActive(true);
        playerMovement.cameraTransform = cameraPos;
        currentCamera = cameraPos;

        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null;
        }
    }
}
