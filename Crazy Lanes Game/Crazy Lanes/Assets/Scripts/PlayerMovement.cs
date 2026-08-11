using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool touchGround;
    [SerializeField] private Transform groundDetector;
    
    // -------Movement Vars-------

    private bool canMove = true;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decelMultiplier;
    
    private float xVel;
    private float xTime;
    private float zVel;
    private float zTime;
    private float sVel;
    private float sTime;
    [SerializeField] private float stopDelta;
    
    // -------Jumping-------

    [SerializeField] private float initialJumpVel;
    [SerializeField] private float inAirMovementMult;

    private float groundCheckBoxSize = 0.3f;
    private float groundCheckBoxHeight = 0.05f;
    
    // -------Camera Movement-------

    private Vector2 prevMousePos;
    [SerializeField] private float sensitivity;
    
    // -------Init-------
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        groundDetector = GameObject.Find("Player/Ground Check").GetComponent<Transform>();
        touchGround = Physics.CheckBox(groundDetector.position, 
            new Vector3(groundCheckBoxSize, groundCheckBoxHeight, groundCheckBoxSize), 
            Quaternion.Euler(0f, 0f, 0f), LayerMask.GetMask("Ground"));
        
        prevMousePos = Input.mousePosition;
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
        Debug.Log("jump");
    }
    
    // -------Movement Functions-------

    void FixedUpdate()
    {
        touchGround = Physics.CheckBox(groundDetector.position, 
            new Vector3(groundCheckBoxSize, groundCheckBoxHeight, groundCheckBoxSize), 
            Quaternion.Euler(0f, 0f, 0f), LayerMask.GetMask("Ground"));
        
        if (canMove)
        {
            RunMovement(touchGround ? 1f : inAirMovementMult);
            rb.linearVelocity = new Vector3(Mathf.Clamp(xVel,-maxSpeed, maxSpeed) * Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), rb.linearVelocity.y, 
                Mathf.Clamp(zVel,-maxSpeed, maxSpeed) * Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
        }
        
        // Camera

        Vector2 mouseDelta = (Vector2) Input.mousePosition - prevMousePos;
        prevMousePos = Input.mousePosition;

        if (Mathf.Abs(mouseDelta.x) > 0f || Mathf.Abs(mouseDelta.y) > 0f)
            RotateCamera(mouseDelta.x * sensitivity, mouseDelta.y * sensitivity);
    }
    
    private void RunMovement(float accelMult)
    {
        float xMult = 1f - Mathf.Abs(zTime - 0.5f) / 2;
        float zMult = 1f - Mathf.Abs(xTime - 0.5f) / 2;
        float sMult = 1f - Mathf.Abs(sTime - 0.5f) / 2;
        xVel = Mathf.Lerp(-maxSpeed * xMult, maxSpeed * xMult, xTime);
        zVel = Mathf.Lerp(-maxSpeed * zMult, maxSpeed * zMult, zTime);

        if (Input.GetKey(KeyCode.W))
        {
            zTime = Mathf.Clamp01(zTime += Time.deltaTime * acceleration * accelMult);
            xTime = Mathf.Clamp01(xTime += Time.deltaTime * acceleration * accelMult);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            zTime = Mathf.Clamp01(zTime -= Time.deltaTime * acceleration * accelMult);
            xTime = Mathf.Clamp01(xTime -= Time.deltaTime * acceleration * accelMult);
        }
        else if (Mathf.Abs(zTime - 0.5f) > stopDelta)
        {
            zTime = Mathf.Clamp01(zTime += Time.deltaTime * decelMultiplier * accelMult * -Mathf.Sign(zTime - 0.5f));
            xTime = Mathf.Clamp01(xTime += Time.deltaTime * decelMultiplier * accelMult * -Mathf.Sign(xTime - 0.5f));
        }
        else
        {
            zTime = 0.5f;
            xTime = 0.5f;
        }

        if (Input.GetKey(KeyCode.D))
            sTime = Mathf.Clamp01(sTime += Time.deltaTime * acceleration * accelMult);
        else if (Input.GetKey(KeyCode.A))
            sTime = Mathf.Clamp01(sTime -= Time.deltaTime * acceleration * accelMult);
        else if (Mathf.Abs(zTime - 0.5f) > stopDelta)
        {
            sTime = Mathf.Clamp01(sTime += Time.deltaTime * decelMultiplier * accelMult * -Mathf.Sign(sTime - 0.5f));
        }
        else
        {
            sTime = 0.5f;
        }
    }

    private void RotateCamera(float xAmount, float yAmount)
    {
        Transform camera = GameObject.Find("Player/Main Camera").GetComponent<Transform>();
        
        transform.Rotate(Vector3.up, xAmount);
        //camera.RotateAround(transform.position, new Vector3(1 * Mathf.Sign((Mathf.Abs(transform.rotation.eulerAngles.y - 180f) - 90f)), 0, 0), -yAmount);
        camera.Rotate(new Vector3(1, 0, 0), -yAmount);
    }
}
