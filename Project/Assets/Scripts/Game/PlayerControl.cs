using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;      // 移动速度
    //[SerializeField] private float turnSpeed = 8f;      // 转向速度
    [SerializeField] private float turnSmoothTime = 0.1f; // 转向平间
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject modelRotation;

    private AudioSource audioSource;
    private Vector3 moveDirection;
    private float currentTurnVelocity;
    private float targetAngle;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // 获取原始输入 (W=前, S=后, A=左, D=右)
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        // 创建移动方向 (世界坐标系)
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 应用移动 (仅当有输入时)
        if (moveDirection.magnitude > 0.1f)
        {
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            animator.SetFloat("Walk", 1f);
            audioSource.mute = false;
        }
        else 
        {
            animator.SetFloat("Walk", 0f);
            audioSource.mute = true;
        }
    }

    void HandleRotation()
    {
        // 仅在有移动输入时转向
        if (moveDirection.magnitude > 0.1f)
        {
            // 计算目标角度 (面向移动方向)
            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // 平滑旋转到目标角度
            float smoothedAngle = Mathf.SmoothDampAngle(
                modelRotation.transform.eulerAngles.y,
                targetAngle,
                ref currentTurnVelocity,
                turnSmoothTime
            );

            modelRotation.transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }
    }
}
