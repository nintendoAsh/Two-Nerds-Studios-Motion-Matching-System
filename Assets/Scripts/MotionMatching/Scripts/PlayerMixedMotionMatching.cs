//ignore the name this was going to be the centeral system but times change and my code does too. there are so many rements of unused stuff here. that you may or may not see becuae this is the first thing I write and I havent gone through the code yet
//makes the trejectory for the matcher to match to. could this be intergated into the matcher... yes? I think it should be able to be done. am I going to that that.. nope  ^ω^ Have fun!
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerMixedMotionMatching : MonoBehaviour
{
//and yep just removed plenty of useless verables that you will never see 
 //and that is like 10 now
 //PlayerMovement PlayerMovement;
    public List<float> FutureFramesAhead = new List<float>();
    public List<Vector3> PosibleFuturePositions = new List<Vector3>();
    //List<Vector3> PosibleFutureRotations = new List<Vector3>();
    int currentIndex = 0;
    //Rigidbody PlayerRigidbody;
    Vector3 lastvelocity = Vector3.zero;
    //public GameObject CameraPivot;
    //CameraController cameraController;
    public Vector3 TeporaryPosition;
    Vector3 TeporaryVelocity;
    Vector3 TeporaryAcceleration;
    public float TrejectoryHalfLife = 0.5f; // the time it will take to be half way to the target position
    public float DebugSphereSize = 0.5f; //more gizmo BS. the size of the sphere shown as the debug trejectory.. see. not hard. WHY ARE YOU STILL READING MY CODE COMMENT! YOU ARE JUST WAISTING YOUR TIME! THERE ARE MOTIONS TO BE MATCHED!!!! GET ON IT BRO
    Vector3 LastMovement;
    public float Responsiveness = 0.01f;
    public MovementProvider MovementProvider; 

    void Awake()
    {
        for (int i = 0; i < FutureFramesAhead.Count; i++)
        {
            PosibleFuturePositions.Add(Vector3.zero); //zeros all the vectors so it doesnt throw and error when trying to perdict trejectory
            //PosibleFutureRotations.Add(Vector3.zero); //does the same but for rotations
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //finds everything
        //PlayerMovement = gameObject.GetComponent<PlayerMovement>();
        //PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
        //cameraController = CameraPivot.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (float value in FutureFramesAhead) //for each different times in the future that perdicting.
        {
            if(currentIndex >= FutureFramesAhead.Count) //resets all values once the current index is equal or greater then the amount of total future frames ahead features
            {
                MovementProvider.UpdateValues();
                currentIndex = 0;//obously reset the currentIndex to do it all over again
                TeporaryPosition = MovementProvider.PlayerPostion; //sets the temp position to current position
                TeporaryVelocity = MovementProvider.PlayerVelocity;//set the temp velocity to current velocity
                TeporaryAcceleration = (MovementProvider.PlayerVelocity - lastvelocity) / value; //set the temp acceleration to current acceleration
            }

            //Vector3 ZaxisMovement = PlayerMovement.CameraForward * PlayerMovement.MoveZ * PlayerMovement.RunSpeedForwards;
            //Vector3 ZaxisMovement = new Vector3(0,0,1) * PlayerMovement.MoveZ * PlayerMovement.RunSpeedForwards;//uses the values from player movement to generate a desired velocity for the Z axis
           // Vector3 XaxisMovement = PlayerMovement.CameraRight * PlayerMovement.MoveX * PlayerMovement.RunSpeedSide;//does the same for the X axis

           // Vector3 Movement = XaxisMovement + ZaxisMovement; //adds desired x and z velocity to get a single vector for desired velocity
        

            //Debug.Log("Movement: " + Movement);

            DesiredVelocityAndTrejectory(ref TeporaryPosition, ref TeporaryVelocity, ref TeporaryAcceleration, new Vector3(Mathf.Lerp(LastMovement.x, MovementProvider.DesiredVelocity.x, Responsiveness), 0f, Mathf.Lerp(LastMovement.z, MovementProvider.DesiredVelocity.z, Responsiveness)),TrejectoryHalfLife, value); //figures out what the velocity, postion, and acceleration should be based off of the temp Velocity, position, and acceleration aswell as the time gap/time step (value) then stores each feature in the temp velocity, position, and acceleration veriables

            PosibleFuturePositions[currentIndex] = TeporaryPosition; //sets the future postion for that time step to the perdicted future postion

            //PosibleFutureRotations[currentIndex] = new Vector3(0, CameraPivot.transform.rotation.y, cameraController.mouseX); //doesnt perdict anything right now.
             
            currentIndex++;
        }

// to calcuate velocity you need to save the last postion so when postion changes you can get a postion delta by subtracting current to last then you dived it by delta time and baboom velocity
       lastvelocity = MovementProvider.PlayerVelocity;
       LastMovement = LastMovement;
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

    void OnDrawGizmos()
    {
        //displays the future trejectory as sphere gizmos
        Gizmos.color = Color.red;

        for (int i = 0; i < PosibleFuturePositions.Count; i++)//for each of the future postions
        {
            if(i == PosibleFuturePositions.Count - 1)
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawSphere(PosibleFuturePositions[i], DebugSphereSize);//draw a sphere
        }   
    }
}
