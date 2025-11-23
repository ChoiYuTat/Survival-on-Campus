using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("跳躍參數")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f; // 射線檢測距離
    public float slopeLimit = 45f;           // 允許的斜坡角度
    public float fastFallSpeed = 20f;        // 空中快速落地速度

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool collisionGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 跳躍輸入
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // 強制清除狀態，避免二段誤判
            isGrounded = false;
            collisionGrounded = false;
        }
        // 空中快速落地
        else if (!isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -fastFallSpeed, rb.linearVelocity.z);
        }
    }

    void FixedUpdate()
    {
        // Raycast 檢查腳下是否有地面，並且角色速度向下時才算落地
        bool raycastGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance)
                               && rb.linearVelocity.y <= 0.01f;

        // 混合判斷：Collision + Raycast
        isGrounded = collisionGrounded || raycastGrounded;
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 檢查接觸點法線是否朝上（小於 slopeLimit 視為地面）
            if (Vector3.Angle(contact.normal, Vector3.up) < slopeLimit)
            {
                collisionGrounded = true;
                return;
            }
        }
        collisionGrounded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        // 離開碰撞時，不直接設 false，交由 OnCollisionStay + Raycast 判斷
        collisionGrounded = false;
    }

    // 可視化 Debug：在 Scene 視窗畫出 Raycast 射線
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
