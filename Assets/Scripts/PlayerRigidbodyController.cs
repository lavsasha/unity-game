using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRigidbodyController : MonoBehaviour
{
    [Header("Forward (fallback)")]
    public float forwardSpeed = 12f;

    [Header("Lanes")]
    public int lanes = 3;
    public float laneDistance = 2f;
    public float sideStiffness = 10f;
    public float sideAccel = 60f;
    public float maxSideSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public int maxJumps = 2;
    public float groundCheckDistance = 0.6f;

    Rigidbody rb;
    int currentLane = 1;
    float targetX;
    int jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        UpdateTargetX();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.linearVelocity;
        float curForward = (GameSpeedManager.Instance != null) ? GameSpeedManager.Instance.CurrentSpeed : forwardSpeed;
        vel.z = curForward;

        float posX = rb.position.x;
        float error = targetX - posX;
        float desiredSideVel = Mathf.Clamp(error * sideStiffness, -maxSideSpeed, maxSideSpeed);
        float maxDeltaV = sideAccel * Time.fixedDeltaTime;
        float newSideVel = Mathf.MoveTowards(vel.x, desiredSideVel, maxDeltaV);
        vel.x = newSideVel;

        rb.linearVelocity = vel;
    }

    void ChangeLane(int dir)
    {
        currentLane = Mathf.Clamp(currentLane + dir, 0, Mathf.Max(0, lanes - 1));
        UpdateTargetX();
    }

    void UpdateTargetX()
    {
        float centerIndex = (lanes - 1) / 2f;
        float x = (currentLane - centerIndex) * laneDistance;
        targetX = x;
    }

    void TryJump()
    {
        if (IsGrounded() || jumpCount < maxJumps)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint c in collision.contacts)
        {
            if (Vector3.Dot(c.normal, Vector3.up) > 0.5f)
            {
                jumpCount = 0;
                break;
            }
        }
    }
}
