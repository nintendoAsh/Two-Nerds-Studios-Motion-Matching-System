using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerMixedMotionMatching : MonoBehaviour
{
    public Animator HumanFormAni;
    public Animator WolfFormAni;
    public bool OnFours = false; //this determines wether to play the animations for the biped or quadruped wolf form
    PlayerMovement PlayerMovement;
    public GameObject HumanFormModel;
    public GameObject WolfFormModel;
    Vector2 CameraRotation;
    Vector3 PosibleFuturePosition;
    public GameObject Future1;
    public GameObject Future2;
    public GameObject Future3;
    public GameObject Future4;
    //public int[] FutureFramesAhead = 5;
    public List<float> FutureFramesAhead = new List<float>();
    public List<Vector3> PosibleFuturePositions = new List<Vector3>();
     List<Vector3> PosibleFutureRotations = new List<Vector3>();
    int currentIndex = 0;
    float MotifiyedMoveX;
    float MotifiyedMoveZ;
    Rigidbody PlayerRigidbody;
    Vector3 lastvelocity = Vector3.zero;
    Vector3 CurrentVolcity;
    float lastRoatationY = 0;
    float lastRoatationX = 0;
    public GameObject CameraPivot;
    CameraController cameraController;
     [SerializeField] Transform Camera;
     Vector3 CameraRight;


     public Vector3 TeporaryPosition;
     Vector3 TeporaryVelocity;
        Vector3 TeporaryAcceleration;
    public float TrejectoryHalfLife = 0.5f; // the time it will take to be half way to the target position
    public float DebugSphereSize = 0.5f;
     Vector3 LastMovement;
     public float Responsiveness = 0.01f;

    void Awake()
    {
        for (int i = 0; i < FutureFramesAhead.Count; i++)
        {
            PosibleFuturePositions.Add(Vector3.zero);
        }

        for (int j = 0; j < FutureFramesAhead.Count; j++)
        {
            PosibleFutureRotations.Add(Vector3.zero);
            Debug.Log(PosibleFutureRotations[j]);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerMovement = gameObject.GetComponent<PlayerMovement>();
        PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
        cameraController = CameraPivot.GetComponent<CameraController>();
    }

    /*public void SwitchDirectionZ()
    {
        if (PlayerMovement.CurrentForm == PlayerMovement.Form.Human)
        {
            HumanFormAni.ResetTrigger("ChangeDirZ");
            HumanFormAni.SetTrigger("ChangeDirZ");
        }
    }
    
    
    public void CameraRotate(InputAction.CallbackContext context)
    {
        CameraRotation += context.ReadValue<Vector2>();
        HumanFormModel.transform.rotation = Quaternion.Euler(0, CameraRotation.x, 0);
    }
    */

    // Update is called once per frame
    void Update()
    {
        foreach (float value in FutureFramesAhead)
        {
            if (currentIndex >= FutureFramesAhead.Count)
            {
                currentIndex = 0;
                TeporaryPosition = PlayerRigidbody.position;
                TeporaryVelocity = PlayerMovement.TemporaryVelocity;
                TeporaryAcceleration = (PlayerMovement.TemporaryVelocity - lastvelocity) / value;

            }

        //Vector3 ZaxisMovement = PlayerMovement.CameraForward * PlayerMovement.MoveZ * PlayerMovement.RunSpeedForwards;
        Vector3 ZaxisMovement = new Vector3(0,0,1) * PlayerMovement.MoveZ * PlayerMovement.RunSpeedForwards;
        Vector3 XaxisMovement = PlayerMovement.CameraRight * PlayerMovement.MoveX * PlayerMovement.RunSpeedSide;

        Vector3 Movement = XaxisMovement + ZaxisMovement;
        

        //Debug.Log("Movement: " + Movement);

            DesiredVelocityAndTrejectory(ref TeporaryPosition, ref TeporaryVelocity, ref TeporaryAcceleration, new Vector3(Mathf.Lerp(LastMovement.x, Movement.x, Responsiveness), 0f, Mathf.Lerp(LastMovement.z, Movement.z, Responsiveness)),TrejectoryHalfLife, value);

            PosibleFuturePositions[currentIndex] = TeporaryPosition;

             PosibleFutureRotations[currentIndex] = new Vector3(0, CameraPivot.transform.rotation.y, cameraController.mouseX);
             
            currentIndex++;
        }


       lastvelocity = PlayerMovement.TemporaryVelocity;
       lastRoatationY = CameraPivot.transform.rotation.y;
       lastRoatationX = cameraController.mouseX;
       LastMovement = LastMovement;
       /*

        if (PlayerMovement.IsMoveing == true && PlayerMovement.IsSprinting != true)
        {
            if (PlayerMovement.CurrentForm == PlayerMovement.Form.Human) //determines wether to play the human animations or werewolf animations
            {
                HumanFormAni.SetBool("IsWalking", true);
                HumanFormAni.SetBool("IsRunning", false);
            }
        }
        else if (PlayerMovement.IsSprinting == true)
        {
            HumanFormAni.SetBool("IsRunning", true);
            HumanFormAni.SetBool("IsWalking", false);
        }
        else if (PlayerMovement.IsMoveing == false)
        {
            HumanFormAni.SetBool("IsWalking", false);
            HumanFormAni.SetBool("IsRunning", false);
        }
        */
    }

    public void DesiredVelocityAndTrejectory(ref Vector3 Postion, ref Vector3 Velocity, ref Vector3 Acceleration, Vector3 VelocityGoal, float HalfLife, float TimeStep = 1, float epsilon = 1e-5f)
    {
        // https://theorangeduck.com/page/spring-roll-call#controllers thanks Daniel Holden for all of your work and math for spring dampers

        float Damping = 3.356693980033321f / (HalfLife + epsilon); //converts the half life to a damping factor, the epsilon is added to prevent division by zero (using improved half life convertion value from a diffrent article by Daniel Holden https://www.youtube.com/watch?v=7aXq8G4YjvA&t=0s&ab_channel=DanielHolden)
        Vector3 DeltaVelocity = Velocity - VelocityGoal; // the difference between the current velocity and the desired velocity
        Vector3 RequiredAceleration = Acceleration + DeltaVelocity * Damping; // the acceleration required to reach the desired velocity
        float Decay = AproxNegitiveExponent(Damping * TimeStep); // the decay factor of the velocity and trejectory

        Postion = Decay * (((-RequiredAceleration)/(Damping * Damping)) + ((-DeltaVelocity - RequiredAceleration * TimeStep) / Damping)) + (RequiredAceleration/(Damping * Damping)) + DeltaVelocity / Damping + VelocityGoal * TimeStep + Postion; // the new position after applying the desired velocity and trejectory
        Velocity = Decay * (DeltaVelocity + RequiredAceleration * TimeStep) + VelocityGoal; // the new velocity after applying the desired velocity and trejectory
        Acceleration = Decay * (Acceleration - RequiredAceleration * Damping * TimeStep); // the new acceleration after applying the desired velocity and trejectory
    }
    float AproxNegitiveExponent(float x) //a fast approximation of the negative exponent function using one over a simple polynomial (used commonly in spring-damper systems to calculate the decay of the system over time).
    {
        return 1.0f / (1.0f + x + 0.48f*x*x + 0.235f*x*x*x);
    }

    private int DebugDotLoop= 0;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < PosibleFuturePositions.Count; i++)
        {
            if(i == PosibleFuturePositions.Count - 1)
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawSphere(PosibleFuturePositions[i], DebugSphereSize);
        }   

        DebugDotLoop++;

        if (DebugDotLoop >= PosibleFuturePositions.Count)
        {
            DebugDotLoop = 0;
        }
    }

    
}
