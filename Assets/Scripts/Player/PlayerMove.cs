using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour {
    
    public float forwardSpeed = 6f;
    public float xSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    // ice movement
    public string iceGroundNameContains = "Map_3_1";
    public float iceSideAcceleration = 8f;
    public float iceSideDeceleration = 3f;
    public float surfaceCheckDistance = 2.2f;

    // determine out of bounds
    public float leftBoundX = -10f;
    public float rightBoundX = 10f;
    public GameObject gameOverPanel2;

    private bool outOfBoundsTriggered = false;

    
    private CharacterController controller;
    private Vector3 verticalMove;
    private bool canMove = true;

    private float currentSideVelocity;
    private bool onIce;

    void Start() {
        // get CharacterController component
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        // stop movement if disabled
        if (!canMove) return;

        // check out of bounds
        if (!outOfBoundsTriggered && (transform.position.x < leftBoundX || transform.position.x > rightBoundX)) {
            outOfBoundsTriggered = true;

            StopImmediately();

            if (gameOverPanel2 != null)
                gameOverPanel2.SetActive(true);

            return;
        }

        // detect whether current ground is ice
        UpdateSurfaceType();
        
        // forward movement
        Vector3 forwardMove = transform.forward * forwardSpeed;
        
        // horizontal movement
        float inputX = ReadHorizontalInput();
        float targetVelocity = inputX * xSpeed;

        if (onIce) {
            // Ice: smooth acceleration / deceleration
            float rate = iceSideAcceleration;

            bool noInput = Mathf.Abs(targetVelocity) < 0.01f;
            bool reversing = Mathf.Sign(targetVelocity) != Mathf.Sign(currentSideVelocity) && Mathf.Abs(currentSideVelocity) > 0.01f;

            if (noInput || reversing)
                rate = iceSideDeceleration;

            currentSideVelocity = Mathf.MoveTowards( currentSideVelocity, targetVelocity, rate * Time.deltaTime
            );
        }
        else {
            // normal ground
            currentSideVelocity = targetVelocity;
        }

        Vector3 sideMove = transform.right * currentSideVelocity;

        
        // jump
        if (ReadJumpPressed() && controller.isGrounded) {
            verticalMove.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        // gravity
        if (controller.isGrounded && verticalMove.y < 0f) {
            verticalMove.y = -2f;
        }

        verticalMove.y += gravity * Time.deltaTime;
        
        // apply movement
        Vector3 move = forwardMove + sideMove + verticalMove;
        controller.Move(move * Time.deltaTime);
    }

    // read keyboard + mobile horizontal input
    float ReadHorizontalInput() {
        float x = Input.GetAxis("Horizontal");

        if (Mobile.Instance != null) {
            float mobile = Mobile.Instance.GetHorizontal();

            if (Mathf.Abs(mobile) > 0.001f)
                x = mobile;
        }

        return Mathf.Clamp(x, -1f, 1f);
    }

    // check jump input 
    bool ReadJumpPressed() {
        bool keyboardJump = Input.GetKeyDown(KeyCode.Space);
        bool mobileJump = Mobile.Instance != null && Mobile.Instance.JumpPressed();

        return keyboardJump || mobileJump;
    }

    // detect whether player is on ice surface
    void UpdateSurfaceType() {
        onIce = false;

        if (string.IsNullOrEmpty(iceGroundNameContains))
            return;

        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, surfaceCheckDistance)) {
            onIce = CheckParentName(hit.collider.transform, iceGroundNameContains);
        }
    }

    // check if any parent object name contains keyword for ice ground
    bool CheckParentName(Transform t, string keyword) {
        string key = keyword.ToLower();

        while (t != null) {
            if (t.name.ToLower().Contains(key))
                return true;

            t = t.parent;
        }

        return false;
    }

    // stop movement immediately 
    public void StopImmediately() {
        canMove = false;
        verticalMove = Vector3.zero;
        currentSideVelocity = 0f;
    }

    // resume movement
    public void ResumeMove() {
        canMove = true;
    }
}