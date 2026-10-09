using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;//the Rigidbody
    public float GravityForce = 1f;
    public float GravitySpeed = 1f;
    float GravityOnStart;
    public float TerminalVelocity = 2f; // the game object will not move faster then this
    float CurrentFallForce = 0;
    public float FallMultiplyer = 1f;
    public float LowFallMultiplyer = 1f;
    bool JumpP;
    public enum ForceType { Acceleration, Impulse, Force, VelocityChange };// pritty much a table of values of the diffrent types of force 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GravityOnStart = GravityForce;
    }

    public void ResetGravity()
    {
        GravityForce = GravityOnStart;
    }

    public void SetGravity(float SetGravityTo)
    {
        GravityForce = SetGravityTo;
    }

    public void ApplyForce(Vector3 Force, ForceType ApplyForceOfType)
    {
        Rigidbody.AddForce(Force, (ForceMode)ApplyForceOfType); //the stuff I have put in the Erum is the same as the ones for ForceMode so doing the (ForceMode)ApplyForceOfType is just pumping the values directly into ForceMode
    }

    public void SetDrag(float Drag)
    {
        Rigidbody.linearDamping = Drag;
    }

    void FixedUpdate()
    {
        /*if (HumanGroundCheckScript.OnGround == false)
        {
            CurrentFallForce += GravitySpeed * GravityForce * Time.fixedDeltaTime;
            CurrentFallForce = Mathf.Clamp(CurrentFallForce, 0, TerminalVelocity);
            Rigidbody.AddForce(Vector3.down * CurrentFallForce, ForceMode.Acceleration);
        }
        else
        {
            CurrentFallForce = 0;
        }
        */

        if (Rigidbody.linearVelocity.y < 0f)
        {
            Rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (FallMultiplyer - 1) * Time.fixedDeltaTime;
        }
        else if (Rigidbody.linearVelocity.y > 0f && JumpP == false)
        {
            Rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (LowFallMultiplyer - 1) * Time.fixedDeltaTime;
        }
    }

    public void JumpPressed(InputAction.CallbackContext JumpPhase)
    {
        if (!JumpPhase.canceled)
        {
            JumpP = true;
        }
        else
        {
            JumpP = false;
        }
    }

    
}
