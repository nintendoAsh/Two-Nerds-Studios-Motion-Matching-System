using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public float MoveX = 0; // the movment input value for the X axis
    [HideInInspector] public float MoveZ = 0; // the Movement input value for the Y axis (it says Z but in the 2D input world of the controller its Y)
    [HideInInspector] public float LastMoveX;// to get the player to slow down you need to multipy something and if we are going based on the normal input if you are not pressing anything MoveX = 0 and any number times zero equles zero so we insted multipy it by this
    [HideInInspector] public float LastMoveZ;
    //bool IsJump = false; // wether jumping is false or true
    [HideInInspector] public bool IsMoveing = false; // wether the player is moving or not or trying to move
    [HideInInspector] public bool IsMoveingForwards = false; // wether the player is moving forwards or not
    [HideInInspector] public bool IsMoveingSide = false; // wether the player is moving side to side or not
    [HideInInspector] public bool IsStopping = false; // are you stopping or not
    bool IsStoppingX = false;
    bool IsStoppingZ = false;
    [HideInInspector] public bool IsSprinting = false; // wether you are currently sprinting
    public enum Form { Neutral, Debug, Human, Wolf2Legs, Wolf4Legs }; //state machines go brr
    [HideInInspector] public Vector3 CameraForward;
    [HideInInspector] public Vector3 CameraRight;
    float PlayerRotation;
    [HideInInspector] public Vector3 ZaxisMovement;
    [HideInInspector] public Vector3 XaxisMovement;
    Vector3 WallOffsetForward = new Vector3(0, 0, 0);
    Vector3 WallOffsetRight = new Vector3(0, 0, 0);
    bool ReloadValues = false;


    //current values
    [HideInInspector] public float RunSpeedForwards = 40f; //your current run speed (will change depending on form)
    [HideInInspector] public float RunSpeedSide = 40f;
    float SprintSpeedForward = 60f;
    float SprintSpeedSide = 60f;
    float AgilityMovementSide = 1; //the current Agilty value for moving left and right 
    float AgilitySlowSide = 1; // The Current Agility value for slowing down when not moving left or right
    float AgilityMovementForwards = 1; //the current Agility value for moving forwards and backwards
    float AgilitySlowForwards = 1; // The Current Agility value for slowing down when not moving forwards or backwards
    float JumpForce; // the current jump speed
    public Form CurrentForm = Form.Neutral;
    public Form LastForm = Form.Neutral;
    [HideInInspector] public float MoveSideVelocity = 0; //your current volicty that you are putting on the player for the side movement
    [HideInInspector] public float MoveForwardVelocity = 0; //your current volicty that you are putting on the player for the forwards movement
    bool XisNegative;
    bool ZisNegative;
    bool XAxisCanMove = true;
    bool ZAxisCanMove = true;
    bool MakingXstop = false;
    bool MakingZstop = false;
    float ChangeDirDelayX = 0;
    float ChangeDirDelayZ = 0;
    Vector3 Movement;

    Vector3 TemporaryPosition;
    public Vector3 TemporaryVelocity;
    Vector3 TemporaryAcceleration;
    Vector3 LastVelocity;


    //Inspector
    [SerializeField] private Rigidbody Rigidbody;//the Rigidbody
    [SerializeField] Transform Camera;
    public GameObject Raycaster;
    PlayerMixedMotionMatching PlayerMixedMoMa;
    PlayerRaycaster PlayerRacaster;
    HealthAndStamina HealthAndStamina;


        //Human Values
        [Header("Human Form")]
        public float HumanJumpHeight = 10;
        public float HumanFormRunSpeedForward = 60f; //the speed of your human form for forwards and backwards
        public float HumanFormRunSpeedSide = 60f;
        public float HumanFormSprintSpeedForward = 50f;
        public float HumanFormSprintSpeedSide = 50f;
        public float AgilityMoveSideHuman = 30; //speed up speed for human for side movements
        public float AgilityMovementForwardsHuman = 1; //speed up speed for human for forwards and backwards movements
        public float AgilitySlowSideHuman = 30; //slow down for left and right for the human form
        public float AgilitySlowForwardsHuman = 1; //slow down for Forwards and backwards for the human form
        public float ChangeDirDelayXHuman = 0;
        public float ChangeDirDelayZHuman = 0;


    //Wolf From 2 Legged values
    [Header("Wolf Form 2 Legs")]
    public float WolfForm2LegsJumpHeight = 20;
    public float WolfForm2LegsRunSpeedForwards = 60f; //everything is the same as the human values exept these controll the wolf form
    public float WolfForm2LegsRunSpeedSide = 60f;
    public float WolfForm2LegsSprintSpeedForwards = 70f;
    public float WolfForm2LegsSprintSpeedSide = 70f;
    public float AgilityMoveSideWolf2Legs = 30;
    public float AgilityMovementForwardsWolf2Legs = 1;
    public float AgilitySlowSideWolf2Legs = 30;
    public float AgilitySlowForwardsWolf2Legs = 1;
    public float ChangeDirDelayXWolf2Legs = 0;
    public float ChangeDirDelayZWolf2Legs = 0;

    //Quadruped Wolf Form values
    [Header("Wolf Form 4 Legs")]
    public float WolfForm4LegsJumpHeight = 5;
    public float WolfForm4LegsRunSpeedForwards = 60f; //everything is the same as the human values and the wolf form biped stuff just this is for when you are on all fours
    public float WolfForm4LegsRunSpeedSide = 60f;
    public float WolfForm4LegsSprintSpeedForwards = 70f;
    public float WolfForm4LegsSprintSpeedSide = 70f;
    public float AgilityMoveSideWolf4Legs = 30;
    public float AgilityMovementForwardsWolf4Legs = 1;
    public float AgilitySlowSideWolf4Legs = 30;
    public float AgilitySlowForwardsWolf4Legs = 1;
    public float ChangeDirDelayXWolf4Legs = 0;
    public float ChangeDirDelayZWolf4Legs = 0;

    public float MoveSpeedFowardDebug;
    public bool ForceMove;

    void Start()
    {
        PlayerRacaster = Raycaster.GetComponent<PlayerRaycaster>();
        HealthAndStamina = GetComponent<HealthAndStamina>();
        PlayerMixedMoMa = gameObject.GetComponent<PlayerMixedMotionMatching>();

        TemporaryAcceleration = Vector3.zero;
        TemporaryVelocity = Vector3.zero;
        TemporaryPosition = transform.position;
    }

    void Update()
    {
        if (CurrentForm != LastForm || ReloadValues == true) //set the players current stats
        {
            switch (CurrentForm)
            {
                case Form.Human:
                    JumpForce = HumanJumpHeight;
                    RunSpeedForwards = HumanFormRunSpeedForward;
                    RunSpeedSide = HumanFormRunSpeedSide;
                    SprintSpeedForward = HumanFormSprintSpeedForward;
                    SprintSpeedSide = HumanFormSprintSpeedSide;
                    AgilityMovementSide = AgilityMoveSideHuman;
                    AgilityMovementForwards = AgilityMovementForwardsHuman;
                    AgilitySlowForwards = AgilitySlowForwardsHuman;
                    AgilitySlowSide = AgilitySlowSideHuman;
                    ChangeDirDelayX = ChangeDirDelayXHuman;
                    ChangeDirDelayZ = ChangeDirDelayZHuman;
                    break;
                case Form.Wolf2Legs:
                    JumpForce = WolfForm2LegsJumpHeight;
                    RunSpeedForwards = WolfForm2LegsRunSpeedForwards;
                    RunSpeedSide = WolfForm2LegsRunSpeedSide;
                    SprintSpeedSide = WolfForm2LegsSprintSpeedSide;
                    SprintSpeedForward = WolfForm2LegsSprintSpeedForwards;
                    AgilityMovementSide = AgilityMoveSideWolf2Legs;
                    AgilityMovementForwards = AgilityMovementForwardsWolf2Legs;
                    AgilitySlowForwards = AgilitySlowForwardsWolf2Legs;
                    AgilitySlowSide = AgilitySlowSideWolf2Legs;
                    ChangeDirDelayX = ChangeDirDelayXWolf2Legs;
                    ChangeDirDelayZ = ChangeDirDelayZWolf2Legs;
                    break;
                case Form.Wolf4Legs:
                    JumpForce = WolfForm4LegsJumpHeight;
                    RunSpeedForwards = WolfForm4LegsRunSpeedForwards;
                    RunSpeedSide = WolfForm2LegsRunSpeedSide;
                    SprintSpeedForward = WolfForm4LegsSprintSpeedForwards;
                    SprintSpeedSide = WolfForm4LegsSprintSpeedSide;
                    AgilityMovementSide = AgilityMoveSideWolf4Legs;
                    AgilityMovementForwards = AgilityMovementForwardsWolf4Legs;
                    AgilitySlowForwards = AgilitySlowForwardsWolf4Legs;
                    AgilitySlowSide = AgilitySlowSideWolf4Legs;
                    ChangeDirDelayX = ChangeDirDelayXWolf4Legs;
                    ChangeDirDelayZ = ChangeDirDelayZWolf4Legs;
                    break;
                case Form.Debug:
                    RunSpeedForwards = 100;
                    RunSpeedSide = 100;
                    AgilityMovementSide = 1;
                    AgilityMovementForwards = 1;
                    break;
                case Form.Neutral:
                    RunSpeedForwards = 40;
                    AgilityMovementSide = 1;
                    AgilityMovementForwards = 1;
                    break;
            }
            LastForm = CurrentForm;
            ReloadValues = false;
        }
        Move();
    }

    
    void FixedUpdate()
    {
        /*
        if (MakingXstop == true)
        {
            IsStopingX();
        }

        if (MakingZstop == true)
        {
            IsStopingZ();
        }
        */
         //runs all the movement code

        if (CurrentForm == Form.Neutral)
        {
            Debug.LogWarning("PLAYER STATE IS SET TO NEUTRAL make sure to set to a state when playing. Thank You! :-)");
        }

        TemporaryPosition = transform.position;
        TemporaryAcceleration = (Rigidbody.linearVelocity - LastVelocity) / Time.fixedDeltaTime;
        LastVelocity = Rigidbody.linearVelocity;
        
    }

    /*

    void Move()
    {
        if (IsMoveingForwards == true || IsMoveingSide == true)
        {

            if (LastMoveZ != MoveZ)
            {
                LastMoveZ = MoveZ;
            }

            if (LastMoveX != MoveX)
            {
                LastMoveX = MoveX;
            }

            if (MoveX == 0 && MoveZ == 0)
            {
                ZeroWallOffset();
            }

           

            if (IsSprinting == true && HealthAndStamina.CurrentEnergy() > 0.5f * Time.fixedDeltaTime)
            {
                HealthAndStamina.UseEnergy(0.5f * Time.fixedDeltaTime);
            }

            if (IsSprinting != true)
            {
                HealthAndStamina.RegainEngergy(0.2f * Time.fixedDeltaTime);
            }



            Vector3 Movement = XaxisMovement + ZaxisMovement;

            Rigidbody.linearVelocity = new Vector3(Movement.x, Rigidbody.linearVelocity.y, Movement.z);

            //Rigidbody.linearVelocity = new Vector3(MoveX * MoveSideVelocity * CameraRight.x, Rigidbody.linearVelocity.y, MoveZ * MoveForwardVelocity * CameraForward.z);
        }

        if(IsMoveingForwards == true)  // if you are currently pressing a button to move you along the Z axis
        {   
            CameraForward = transform.TransformVector(Camera.transform.forward).normalized;
            CameraForward.y = 0;
            IsStoppingZ = false;

            if (MoveForwardVelocity != RunSpeedForwards && MoveZ != 0)
            {
                MoveForwardVelocity = Mathf.MoveTowards(MoveForwardVelocity, RunSpeedForwards, AgilityMovementForwards * Time.fixedDeltaTime);
            }


            if (MoveZ <= -0.1)
            {
                if (PlayerRacaster.IsBackwardsHitting())
                {
                    WallOffsetForward = Vector3.ProjectOnPlane(transform.forward, PlayerRacaster.BackwardsHitting().normal);
                }
                else if (PlayerRacaster.IsBackwardsHitting2())
                {
                    WallOffsetForward = Vector3.ProjectOnPlane(transform.forward, PlayerRacaster.BackwardsHitting2().normal);
                }
                else
                {
                    WallOffsetForward = Vector3.zero;
                }
            }

            if (MoveZ >= 0.1)
            {
                if (PlayerRacaster.IsForwardsHitting())
                {
                    WallOffsetForward = Vector3.ProjectOnPlane(transform.forward, PlayerRacaster.ForwardsHitting().normal);
                }
                else if (PlayerRacaster.IsBackwardsHitting2())
                {
                    WallOffsetForward = Vector3.ProjectOnPlane(transform.forward, PlayerRacaster.ForwardsHitting2().normal);
                }
                else
                {
                    WallOffsetForward = Vector3.zero;
                }
            }

            if (ZAxisCanMove == true)
            {
                
                ZaxisMovement = CameraForward * LastMoveZ * MoveForwardVelocity + WallOffsetForward;
            }
        }


        if (IsMoveingSide == true) //if you are pressing a button that moves you along the X axis
        {
            CameraRight = transform.TransformVector(Camera.transform.right).normalized;
            CameraRight.y = 0;


            if (MoveSideVelocity != RunSpeedSide && MoveX != 0)
            {
                MoveSideVelocity = Mathf.MoveTowards(MoveSideVelocity, RunSpeedSide, AgilityMovementSide * Time.fixedDeltaTime);
            }

            if (MoveX <= -0.1)
            {
                if (PlayerRacaster.IsLeftHitting())
                {
                    WallOffsetRight = Vector3.ProjectOnPlane(transform.right, PlayerRacaster.LeftHitting().normal);
                }
                else if (PlayerRacaster.IsLeftHitting2())
                {
                    WallOffsetRight = Vector3.ProjectOnPlane(transform.right, PlayerRacaster.LeftHitting2().normal);
                }
                else
                {
                    WallOffsetRight = Vector3.zero;
                }
            }

            if (MoveX >= 0.1)
            {
                if (PlayerRacaster.IsRightHitting())
                {
                    WallOffsetRight = Vector3.ProjectOnPlane(transform.right, PlayerRacaster.RightHitting().normal);
                }
                else if (PlayerRacaster.IsRightHitting2())
                {
                    WallOffsetRight = Vector3.ProjectOnPlane(transform.right, PlayerRacaster.RightHitting2().normal);
                }
                else
                {
                    WallOffsetRight = Vector3.zero;
                }
            }

            if (XAxisCanMove == true)
            {
                XaxisMovement = CameraRight * LastMoveX * MoveSideVelocity + WallOffsetRight;
            }
            
        }

        
        else if (IsStopping == true)
        {
            IsStopingX();
            IsStopingZ();

            if (MoveSideVelocity == 0 && MoveForwardVelocity == 0)
            {
                IsStopping = false;
            }

        }
        

        if(IsStoppingX == false && IsStoppingZ == false)
        {
            HealthAndStamina.RegainEngergy(0.5f * Time.fixedDeltaTime);
        }

        if(IsStoppingX == true)
        {
            StopingX();
            if(MoveSideVelocity == 0)
            {
                IsStoppingX = false;
            }
        }

        if(IsStoppingZ == true)
        {
            StopingZ();
            if(MoveForwardVelocity == 0)
            {
                IsStoppingZ = false;
            }
        }
    }
    */

    void Move()
    {
        if (MoveX != 0 && MoveZ != 0){
         if (LastMoveX != MoveX)
            {
                LastMoveX = MoveX;
            }

        if (LastMoveZ != MoveZ)
            {
                LastMoveZ = MoveZ;
            }
        }

        if(ForceMove == true)
        {
            MoveZ = MoveSpeedFowardDebug;
        }

        CameraRight = transform.TransformVector(Camera.transform.right).normalized;
            CameraRight.y = 0;
        CameraForward = transform.TransformVector(Camera.transform.forward).normalized;
            CameraForward.y = 0;

        MoveSideVelocity = Mathf.MoveTowards(MoveSideVelocity, RunSpeedSide * MoveX, AgilityMovementSide * Time.deltaTime);
        MoveForwardVelocity = Mathf.MoveTowards(MoveForwardVelocity, RunSpeedForwards * MoveZ, AgilityMovementForwards * Time.deltaTime);


        if(ForceMove == true)
        {
            ZaxisMovement =  new Vector3(0,0,1) * MoveZ * RunSpeedForwards + WallOffsetForward;
        }else{
        ZaxisMovement = CameraForward * MoveZ * RunSpeedForwards + WallOffsetForward;
        }
        XaxisMovement = CameraRight * MoveX * RunSpeedSide + WallOffsetRight;
               // Debug.Log(ZaxisMovement);

        Movement = XaxisMovement + ZaxisMovement;


        PlayerMixedMoMa.DesiredVelocityAndTrejectory(ref TemporaryPosition, ref TemporaryVelocity, ref TemporaryAcceleration, Movement, PlayerMixedMoMa.TrejectoryHalfLife, Time.deltaTime);

        Rigidbody.linearVelocity = new Vector3(TemporaryVelocity.x, Rigidbody.linearVelocity.y, TemporaryVelocity.z);
    }

    

/*
    void StopingX()
    {

        if (Rigidbody.linearVelocity.x == 0)
        {
            MoveSideVelocity = 0;
        }

        if (MoveSideVelocity != 0)
        {
            MoveSideVelocity = Mathf.MoveTowards(MoveSideVelocity, 0, AgilitySlowSide * Time.fixedDeltaTime);
        }

        Vector3 XaxisMovement = CameraRight * LastMoveX * MoveSideVelocity;

        Vector3 Movement = XaxisMovement + ZaxisMovement;

        Rigidbody.linearVelocity = new Vector3(Movement.x, Rigidbody.linearVelocity.y, Movement.z);

    }
    */

    void ZeroWallOffset()
    {
        WallOffsetForward = Vector3.zero;
        WallOffsetRight = Vector3.zero;
    }

/*
    void StopingZ()
    {
        if (Rigidbody.linearVelocity.z == 0)
        {
            MoveForwardVelocity = 0;
        }

        if (MoveForwardVelocity != 0)
        {
            MoveForwardVelocity = Mathf.MoveTowards(MoveForwardVelocity, 0, AgilitySlowForwards * Time.fixedDeltaTime);
        }

        Vector3 ZaxisMovement = CameraForward * LastMoveZ * MoveForwardVelocity;

        Vector3 Movement = XaxisMovement + ZaxisMovement;

        Rigidbody.linearVelocity = new Vector3(Movement.x, Rigidbody.linearVelocity.y, Movement.z);

        PlayerMixedMoMa.SwitchDirectionZ();
    }
*/

    private IEnumerator DirectionChangeX()
    {
        XAxisCanMove = false;
        IsStopping = true;
        yield return new WaitForSeconds(ChangeDirDelayX);
        XAxisCanMove = true;
    }

    private IEnumerator DirectionChangeZ()
    {
        ZAxisCanMove = false;
        IsStopping = true;
        yield return new WaitForSeconds(ChangeDirDelayZ);
        ZAxisCanMove = true;
    }


    public void OnJump(InputAction.CallbackContext JumpPhase)
    {
        if (JumpPhase.started && PlayerRacaster.TouchingGround())
        {
            Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, JumpForce, Rigidbody.linearVelocity.z);
        }
    }

    public void OnMove(InputAction.CallbackContext Move)
    {

        Vector2 MoveVector = Move.ReadValue<Vector2>();
        MoveX = MoveVector.x;
        MoveZ = MoveVector.y;

        if (Move.canceled)
        {
            IsStopping = true;
            IsMoveingForwards = false;
            IsMoveingSide = false;
        }

        if(MoveX == 0)
        {
            IsStoppingX = true;  
        }

        if(MoveZ == 0)
        {
            IsStoppingZ = true;  
        }

        if (Move.performed)
        {

            if(MoveZ != 0)
            {
                IsMoveingForwards = true;
            }

            if(MoveX != 0)
            {
                IsMoveingSide = true;
            }
            
            if (MoveX < -0.1 && XisNegative == false && MoveSideVelocity > 5)
            {
                XisNegative = true;
                StartCoroutine(DirectionChangeX());
            }
            else if (MoveX > 0.1 && XisNegative == true && MoveSideVelocity > 5)
            {
                XisNegative = false;
                StartCoroutine(DirectionChangeX());
            }

            if (MoveZ > -0.1 && ZisNegative == false && MoveForwardVelocity > 5)
            {
                ZisNegative = true;
                StartCoroutine(DirectionChangeZ());
            }
            else if (MoveZ < 0.1 && ZisNegative == true && MoveForwardVelocity > 5)
            {
                ZisNegative = false;
                StartCoroutine(DirectionChangeZ());
            }
        }
    }
    
    public void OnSprint(InputAction.CallbackContext Sprint)
    {
        if (Sprint.started && HealthAndStamina.CurrentEnergy() > 0.5f * Time.fixedDeltaTime)
        {
            RunSpeedForwards = SprintSpeedForward;
            RunSpeedSide = SprintSpeedSide;
            IsSprinting = true;
        }
        else if (Sprint.canceled)
        {
            ReloadValues = true;
            IsSprinting = false;
        }
    }
}