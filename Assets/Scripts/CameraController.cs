using System.Threading;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public PlayerInput input;
    Vector2 mouseDelta = new Vector2(0, 0);
    public GameObject Pivot;
    //public GameObject Follow;
    public bool Inverted = false;

    [Range(-100, 100)] //this is how you get it to pop up as a silder in the inspector
    public float SensitivityX = 100f;
    [Range(-100, 100)]
    public float SensitivityY = 100f;

        [Range(-100, 100)] //this is how you get it to pop up as a silder in the inspector
    public float ControllerStickSensitivityX = 100f;
    [Range(-100, 100)]
    public float ControllerStickSensitivityY = 100f;

    [Range(-360, 360)] public float YClampUp = 0;
    [Range(-360, 360)] public float YClampDown = 0;

    public float MinDeltaMovement = 0; //used to filter out micro moments its set to 3 in editor

    float deltaTimeMultiplier;
    bool IsCurrentDeviceMouse;

    public float mouseX;
    public float mouseY;
    bool IsLocked = true;
    public GameObject DebugMenu;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //sets the cursor to the middle of the screen/game
        //Cursor.visible = false;// hides the mouse cursor
        IsLocked = true;
        DebugMenu.SetActive(false);
    }

    public void ShowCursor(InputAction.CallbackContext context)
    {
        if(context.started && IsLocked == true){
        Cursor.lockState = CursorLockMode.None; //unlocks the mouse pointer
        //Cursor.visible = true;// shows the mouse
        IsLocked = false;
        Debug.Log("Pressed");
        DebugMenu.SetActive(true);
        }else if(context.started && IsLocked == false){
            Cursor.lockState = CursorLockMode.Locked; //sets the cursor to the middle of the screen/game
        //Cursor.visible = false;// hides the mouse cursor
        IsLocked = true;
        DebugMenu.SetActive(false);
        }
    }

    void LateUpdate()
    {
        //transform.position = new Vector3(Follow.transform.position.x, transform.position.y, Follow.transform.position.z);//I needed a way to perant the camera to an object but not have it affected by the perants rotation and this is the workaround/hack


        if (IsCurrentDeviceMouse == false) //choses to multiply the motion by Time.deltaTime because if you hold the mouse to one side it will stop when you stop but a joystick you hold it to the side and it should keep roatating
        {
            deltaTimeMultiplier = Time.deltaTime;//joystick mode
        }
        else
        {
            deltaTimeMultiplier = 1.0f; //mouse/ pointer mode
        }

        if (Inverted == false) // inverts the values if the player wants inverted controls
        {
            Pivot.transform.localRotation = Quaternion.Euler(-mouseY, -mouseX, 0.0f); //somehow I guessed that just adding a negative make itself inverted un-inverting the inverted delta
        }
        else
        {
            Pivot.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0.0f);
        }

    }



    public void OnLook(InputAction.CallbackContext context)//runs when you move the mouse or whatever is affecting the look of the camera
    {
        mouseDelta = context.ReadValue<Vector2>();//this grabs the delta of the mouse and returns it as a vector 2

        if (mouseDelta.magnitude > MinDeltaMovement && IsCurrentDeviceMouse == true) //if the movement is bigger then the minimum delta movemnt (just to clean out noise)
        {


            mouseX += mouseDelta.x * SensitivityX * deltaTimeMultiplier;
            mouseY += mouseDelta.y * SensitivityY * deltaTimeMultiplier;

        }
        else if (IsCurrentDeviceMouse == false)
        {
            mouseX += mouseDelta.x * ControllerStickSensitivityX * deltaTimeMultiplier;
            mouseY += mouseDelta.y * ControllerStickSensitivityY * deltaTimeMultiplier;
        }
        
        mouseY = ClampAngle(mouseY, YClampUp, YClampDown);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax) //does as says it clamps the angle to the given degrees dont know why is so big I could hvae used mathf.Clamp
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    public void OnControlChange() // this runs whenever the input system detects a diffrent control method being used
    {
        Debug.Log("Input Chnaged");
        string InputMethod = input.currentControlScheme;
        if (InputMethod == "Gamepad") //and if it detects and input from a controller it starts doing the whole time.delta time thing above in fixed update
        {
            IsCurrentDeviceMouse = false;
        }
        else
        {
            IsCurrentDeviceMouse = true;
        }
    }
}
