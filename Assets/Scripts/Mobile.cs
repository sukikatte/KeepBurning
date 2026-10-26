using UnityEngine;

public class Mobile : MonoBehaviour {
    
    public static Mobile Instance;

    public bool leftHeld;
    public bool rightHeld;
    public bool jumpPressed;

    void Awake() {
        // assign singleton instance
        Instance = this;
    }

    // called button: press left
    public void OnLeftDown() {
        leftHeld = true;
    }

    // called button: release left
    public void OnLeftUp() {
        leftHeld = false;
    }

    // called button: press right
    public void OnRightDown() {
        rightHeld = true;
    }

    // called button: release right
    public void OnRightUp() {
        rightHeld = false;
    }

    // called button: jump press
    public void OnJump() {
        jumpPressed = true;
    }


    public float GetHorizontal() {
        if (leftHeld)
            return -1f;

        if (rightHeld)
            return 1f;

        return 0f;
    }
    
    // make sure each press only jump once
    public bool JumpPressed() {
        bool pressed = jumpPressed;
        jumpPressed = false;
        return pressed;
    }
}