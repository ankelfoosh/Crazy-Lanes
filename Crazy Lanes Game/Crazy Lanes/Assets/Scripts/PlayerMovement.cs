using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool touchGround;
    [SerializeField] private Vector3 groundDetector;
    
    // -------Movement Vars-------

    private bool canMove = true;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decelMultiplier;
    
    private float xVel;
    private float xTime;
    private float zVel;
    private float zTime;
    [SerializeField] private float stopDelta;
    
    // -------Jumping-------

    [SerializeField] private float initialJumpVel;
    [SerializeField] private float inAirMovementMult;
    
    // -------Init-------
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        touchGround = Physics.CheckBox(groundDetector, groundDetector, 
            Quaternion.Euler(0f, 0f, 0f), LayerMask.GetMask("Ground"));
    }
    
    // -------Jump Functions-------

    void Update()
    {
        if (touchGround && Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(rb.linearVelocity.x, initialJumpVel, rb.linearVelocity.z), ForceMode.Impulse);
    }
    
    // -------Movement Functions-------

    void FixedUpdate()
    {
        touchGround = Physics.CheckBox(groundDetector, groundDetector, 
            Quaternion.Euler(0f, 0f, 0f), LayerMask.GetMask("Ground"));
        
        if (canMove)
        {
            RunMovement(touchGround ? 1f : inAirMovementMult);
            rb.linearVelocity = new Vector3(Mathf.Clamp(xVel,-maxSpeed, maxSpeed), rb.linearVelocity.y, 
                Mathf.Clamp(zVel,-maxSpeed, maxSpeed));
        }
    }
    
    private void RunMovement(float speedMult)
    {
        float xMult = 1f - Mathf.Abs(zTime - 0.5f) / 2;
        float zMult = 1f - Mathf.Abs(xTime - 0.5f) / 2;
        xVel = Mathf.Lerp(-maxSpeed * xMult, maxSpeed * xMult, xTime);
        zVel = Mathf.Lerp(-maxSpeed * zMult, maxSpeed * zMult, zTime);
        
        if (Input.GetKey(KeyCode.W))
            zTime = Mathf.Clamp01(zTime += Time.deltaTime * acceleration);
        else if (Input.GetKey(KeyCode.S))
            zTime = Mathf.Clamp01(zTime -= Time.deltaTime * acceleration);
        else if (Mathf.Abs(zTime - 0.5f) > stopDelta)
            zTime = Mathf.Clamp01(zTime += Time.deltaTime * decelMultiplier * -Mathf.Sign(zTime - 0.5f));
        else
            zTime = 0.5f;

        if (Input.GetKey(KeyCode.D))
            xTime = Mathf.Clamp01(xTime += Time.deltaTime * acceleration);
        else if (Input.GetKey(KeyCode.A))
            xTime = Mathf.Clamp01(xTime -= Time.deltaTime * acceleration);
        else if (Mathf.Abs(xTime - 0.5f) > stopDelta)
            xTime = Mathf.Clamp01(xTime += Time.deltaTime * decelMultiplier * -Mathf.Sign(xTime - 0.5f));
        else
            xTime = 0.5f;
    }
}
