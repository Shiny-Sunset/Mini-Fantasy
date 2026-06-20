using System.Collections;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    [Header("魔法の弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("魔法の弾を出す座標")]
    public Transform firePoint;
    [Header("魔法の弾の攻撃ダメージ倍率")]
    [Range(0f, 1000f)]
    public float attackRate = 0.5f;
    [Header("魔法の使用MP")]
    [Range(0.1f, 100f)]
    [UnityEngine.Serialization.FormerlySerializedAs("useMagicPpont")]
    public float useMagicPoint = 20f;
    [Header("魔法のクールタイム")]
    [Range(0.1f, 10f)]
    public float fireCooldown = 2f;
    [Header("弾速")]
    [Range(0.1f, 50f)]
    public float bulletSpeed = 10f;
    [Header("HP回復量")]
    [Range(0.1f, 9999f)]
    public float healHP = 30f;
    public float healMP = 30f;
    [Header("必殺の最後の突きの移動距離")]
    public float thrustDistance = 2f;
    [Header("必殺の最後の突きの移動速度")]
    public float thrustSpeed = 10f;
    public float rotationSpeed = 10f;

    [Header("必殺の攻撃倍率")]
    [Range(0f, 1000f)]
    public float ultRate = 15f;

    [Header("雷関係")]
    public GameObject lightningVFXPrefab;
    public float lightningInterval = 0.2f;
    public Vector2 areaSize = new Vector2(8f, 5f);

    [Header("必殺のヒットボックス")]
    public GameObject hitboxObject;

    private Coroutine lightningRoutine;
    private bool isThrusting = false;
    private float thrustRemaining;

    private CharacterController controller;

    public KeyCode magicKey = KeyCode.Alpha1;
    public KeyCode healKey = KeyCode.Alpha2;
    public KeyCode blockKey = KeyCode.Alpha3;
    public KeyCode ultimateKey = KeyCode.Alpha4;
    public LockOnManager lockOnManager;
    [System.NonSerialized]
    public bool isBlocking = false;

    private Transform lockOnTarget;
    private Animator animator;
    private PlayerStatus status;
    private float lastFireTime = -999f;
    private int upperBodyLayerIndex = 1;
    private bool isUltimateActive = false;
    private PlayerMovement playerMovement;
    private AttackHitbox ultHitBox;
    private PlayerAttack playerAttack;

    void Start()
    {
        animator = GetComponent<Animator>();
        status = GetComponent<PlayerStatus>();
        lockOnTarget = lockOnManager.currentTarget;
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        ultHitBox = hitboxObject.GetComponent<AttackHitbox>();
        ultHitBox.SetDamage(playerAttack.attackDamage * ultRate);
        ultHitBox.playerStatus = status;
        hitboxObject.SetActive(false);
    }

    void Update()
    {
        lockOnTarget = lockOnManager.currentTarget;

        if (Input.GetKeyDown(magicKey) && !isBlocking)
            UseMagic();

        if (Input.GetKeyDown(healKey) && !isBlocking)
            UseHeal();

        if (Input.GetKey(blockKey))
            StartBlock();
        else
            EndBlock();

        if (Input.GetKeyDown(ultimateKey) && !isBlocking)
            UseUltimate();

        if (isThrusting)
        {
            Vector3 move = transform.forward * thrustSpeed * Time.deltaTime;
            controller.Move(move);
            thrustRemaining -= move.magnitude;

            if (thrustRemaining <= 0f)
                isThrusting = false;
        }

        if (isUltimateActive && lockOnTarget != null)
        {
            Vector3 direction = lockOnTarget.position - transform.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void UseMagic()
    {
        if (!IsUpperBodyAnimating("root_Magic") && Time.time - lastFireTime >= fireCooldown)
        {
            if (status.CanUseMP(useMagicPoint))
            {
                animator.SetTrigger("Magic");
                lastFireTime = Time.time;
            }
        }
    }

    void Shoot()
    {
        lastFireTime = Time.time;
        status.UseMP(useMagicPoint);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.SetTarget(lockOnTarget);
            playerBullet.damage = playerAttack.attackDamage * attackRate;
            playerBullet.speed = bulletSpeed;
        }
    }

    void UseHeal()
    {
        if (!IsUpperBodyAnimating("root_Heal") && !status.FullHP())
        {
            if (status.CanUseMP(healMP))
                animator.SetTrigger("Heal");
        }
    }

    void Heal()
    {
        status.UseMP(healMP);
        status.HealHP(healHP);
        AudioManager.Instance.PlaySE(AudioManager.Instance.healSound);
    }

    void StartBlock()
    {
        if (isBlocking) return;
        isBlocking = true;
        animator.SetBool("IsBlocking", true);
        status.blockRate = 0.5f;
    }

    void EndBlock()
    {
        if (!isBlocking) return;
        isBlocking = false;
        animator.SetBool("IsBlocking", false);
        status.blockRate = 1f;
    }

    bool IsUpperBodyAnimating(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayerIndex);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1f;
    }

    void UseUltimate()
    {
        if (!IsUpperBodyAnimating("root_Ultimate"))
        {
            if (status.UseSP())
                animator.SetTrigger("Ultimate");
        }
    }

    public void StartUltimate()
    {
        isUltimateActive = true;
        status.blockRate = 0f;
        playerMovement.stopMove = true;
    }

    public void EndUltimate()
    {
        isUltimateActive = false;
        status.blockRate = 1f;
        playerMovement.stopMove = false;
    }

    public void TriggerThrust()
    {
        isThrusting = true;
        thrustRemaining = thrustDistance;
    }

    public void StartLightning()
    {
        if (lightningRoutine == null)
            lightningRoutine = StartCoroutine(LightningLoop());
    }

    public void StopLightning()
    {
        if (lightningRoutine != null)
        {
            StopCoroutine(lightningRoutine);
            lightningRoutine = null;
        }
    }

    public void EnableUltHitbox()
    {
        if (hitboxObject != null)
            hitboxObject.SetActive(true);
    }

    public void DisableUltHitbox()
    {
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
            ultHitBox.ResetHitTargets();
        }
    }

    private IEnumerator LightningLoop()
    {
        while (true)
        {
            Vector3 pos = GetRandomPositionInFront();
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Instantiate(lightningVFXPrefab, pos, randomRotation);
            yield return new WaitForSeconds(lightningInterval);
        }
    }

    private Vector3 GetRandomPositionInFront()
    {
        Vector3 center = transform.position + transform.forward * (areaSize.y / 2f);
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        Vector3 offset = transform.right * randomX + transform.forward * randomZ;
        return new Vector3(center.x + offset.x, transform.position.y, center.z + offset.z);
    }
}
