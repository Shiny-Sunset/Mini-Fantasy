using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("歩きの速度")]
    [Range(0.1f, 50f)] public float walkSpeed = 2f;
    [Header("走りの速度")]
    [Range(0.1f, 50f)] public float runSpeed = 4f;
    [Header("加速度")]
    [Range(0.1f, 50f)] public float acceleration = 5f;
    [Header("カメラの座標")]
    public Transform cameraTransform;

    [Header("重力設定")]
    public float gravity = -9.81f;
    public float groundCheckOffset = -0.1f;

    [System.NonSerialized]
    public bool isDead = false;
    [System.NonSerialized]
    public bool stopMove = false;

    private CharacterController controller;
    private Animator animator;
    private float currentSpeed = 0f;
    private HandController handController;

    private Vector3 moveDirection;
    private Vector3 verticalVelocity = Vector3.zero; // Y方向の速度（重力処理用）

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        handController = GetComponent<HandController>();
        handController.Grab();
    }

    void Update()
    {
        // ----- 重力の処理 -----
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
        verticalVelocity.y += gravity * Time.deltaTime;

        // 最終的な移動量（水平＋重力）
        Vector3 totalMovement = moveDirection * currentSpeed + verticalVelocity;
        controller.Move(totalMovement * Time.deltaTime);

        // ----- 移動制限チェック -----
        if (isDead || stopMove)
        {
            moveDirection = Vector3.zero;
            currentSpeed = 0f;
            animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
            return;
        }

        // ----- 入力処理 -----
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(h, 0, v).normalized;

        // カメラ方向に基づく移動ベクトル計算
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = Vector3.zero;
        if (inputDirection.magnitude >= 0.1f)
        {
            moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;

            // 回転
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                Time.deltaTime * 10f
            );
        }

        // スピード設定
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float maxSpeed = isRunning ? runSpeed : walkSpeed;
        float targetMagnitude = moveDirection.magnitude * maxSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetMagnitude, acceleration * Time.deltaTime);

        // アニメーション設定
        float animSpeed = (moveDirection.magnitude > 0.1f) ? (isRunning ? 2f : 1f) : 0f;
        animator.SetFloat("Speed", animSpeed, 0.2f, Time.deltaTime);
    }
}
