using UnityEngine;

public class PlayerBullet : AttackHitbox
{
    public float lifeTime = 5f;
    public float speed = 20f;
    public float rotateSpeed = 5f; // 回転スピード（大きいと強く追尾）
    public float heightOffset = 0.2f; // 高さ補正値（胸〜頭くらい）
    private Rigidbody rb;
    private Transform target;
    void Start()
    {

        healMP = 0f;
        healSP = 2.5f;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // 最初の進行方向は forward
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * heightOffset;

            // 向かいたい方向（追尾対象方向）
            Vector3 direction = (targetPos - transform.position).normalized;

            // 現在の前方ベクトル
            Vector3 forward = transform.forward;

            // 現在の前方向と、目標方向との角度（Y軸を基準とした signed angle）
            float angleY = Vector3.SignedAngle(forward, direction, Vector3.up);

            // --- 真後ろを向いているかの判定（例：150度以上）
            bool isBehind = Mathf.Abs(angleY) > 150f;

            // 上回りを強制したいとき：Y方向にある程度持ち上げる補正
            if (isBehind)
            {
                // 強制的に上に持ち上げた方向へ向かうよう補正
                direction += Vector3.up * 0.5f;
                direction.Normalize();
            }

            // 滑らかに進行方向補正（方向ベクトル補間）
            Vector3 newDir = Vector3.Lerp(rb.linearVelocity.normalized, direction, rotateSpeed * Time.fixedDeltaTime).normalized;

            rb.linearVelocity = newDir * speed;

            if (rb.linearVelocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }
        }

    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!hitTargets.Contains(other.gameObject))
            {
                // ダメージ処理（例: EnemyHealth というスクリプトを呼ぶ）
                EnemyHealth eh = other.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.magicHitSound);
                    eh.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // Player 以外のオブジェクトに当たったら破壊（壁など）
            Destroy(gameObject);
        }
    }
}
