using UnityEngine;
using Unity.Cinemachine;

public class CinemachineLockOnCamera : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float distanceFromPlayer = 6f;
    public float heightOffset = 2f;
    public float followSpeed = 1f;

    private CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void LateUpdate()
    {
        if (target == null || player == null) return;

        // 敵とプレイヤーの方向ベクトル
        Vector3 direction = (target.position - player.position).normalized;

        // カメラの理想的な位置 = プレイヤーの後ろ（敵から見て後ろ側）+ 上にオフセット
        Vector3 desiredPos = player.position - direction * distanceFromPlayer + Vector3.up * heightOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // 敵とプレイヤーの中間点を向く
        Vector3 lookPoint = (player.position + target.position) / 2f;
        transform.LookAt(lookPoint);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }
}
