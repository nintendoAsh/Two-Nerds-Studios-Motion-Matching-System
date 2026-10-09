using UnityEngine;

public class PlayerRaycaster : MonoBehaviour
{
    float alpha = 0.75f;
    Vector3 RaycastDirectionForwards;
    Vector3 RaycastDirectionRight;
    Vector3 RaycastDirectionUp = Vector3.up;
    public GameObject FollowRotation;

    public float TotalRaycastDistanceOffset = 1;
    public Vector3 TotalRaycastPosOffset = new Vector3(0f, 0f, 0f);
    public LayerMask RaycastXYZLayerMask;
    [Space]
    [Space]

    [Header("Z AXIS")]
    [Header("Forwards")]
    public GameObject RaycastForwardsEmpty;
    public Vector3 RaycastOffsetForwards;
    public float RayDistaceForwards = 100;
    [Space]
    public GameObject RaycastForwardsEmpty2;
    public Vector3 RaycastOffsetForwards2;
    public float RayDistaceForwards2 = 100;
    [Header("Backwards")]
    public GameObject RaycastBackwardsEmpty;
    public Vector3 RaycastOffsetBackwards;
    public float RayDistaceBackwards = 100;
    [Space]
    public GameObject RaycastBackwardsEmpty2;
    public Vector3 RaycastOffsetBackwards2;
    public float RayDistaceBackwards2 = 100;

    [Header("X AXIS")]
    [Header("Right")]
    public GameObject RaycastRightEmpty;
    public Vector3 RaycastOffsetRight;
    public float RayDistaceRight = 100;
    [Space]
    public GameObject RaycastRightEmpty2;
    public Vector3 RaycastOffsetRight2;
    public float RayDistaceRight2 = 100;
    [Header("Left")]
    public GameObject RaycastLeftEmpty;
    public Vector3 RaycastOffsetLeft;
    public float RayDistaceLeft = 100;
    [Space]
    public GameObject RaycastLeftEmpty2;
    public Vector3 RaycastOffsetLeft2;
    public float RayDistaceLeft2 = 100;

    [Header("Y AXIS")]
    [Header("MISCELLANEOUS")]
    public GameObject GroundCheckEmpty;
    public Vector3 GroundCheckOffset;
    public float RayDistaceDown = 100;
    public LayerMask GroundLayer;
    [Space]
    public GameObject HeadCheckEmpty;
    public Vector3 HeadCheckOffset;
    public float RayDistaceUp = 100;


    Ray ForwardDirRay;
    Ray ForwardDirRay2;

    Ray BackwardDirRay;
    Ray BackwardDirRay2;

    Ray RightDirRay;
    Ray RightDirRay2;

    Ray LeftDirRay;
    Ray LeftDirRay2;

    Ray GroundCheck;
    Ray HeadCheck;

    void LateUpdate()
    {
        transform.right = FollowRotation.transform.right; //maches the rotation to the object spesifyed in this case the camera so when checking for wether you can walk forward or back it is forward and back ralative to the camera
    }

    public RaycastHit ForwardsHitting()
    {
        RaycastDirectionForwards = transform.forward;
        ForwardDirRay = new Ray(RaycastForwardsEmpty.transform.position + RaycastOffsetForwards + TotalRaycastPosOffset, RaycastDirectionForwards);

        Physics.Raycast(ForwardDirRay, out RaycastHit For1Hit, RayDistaceForwards * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return For1Hit;
    }

    public RaycastHit ForwardsHitting2()
    {
        RaycastDirectionForwards = transform.forward;
        ForwardDirRay2 = new Ray(RaycastForwardsEmpty2.transform.position + RaycastOffsetForwards2 + TotalRaycastPosOffset, RaycastDirectionForwards);
        Physics.Raycast(ForwardDirRay2, out RaycastHit For2Hit, RayDistaceForwards2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return For2Hit;
    }

    public RaycastHit BackwardsHitting()
    {
        RaycastDirectionForwards = transform.forward;
        BackwardDirRay = new Ray(RaycastBackwardsEmpty.transform.position + RaycastOffsetBackwards2, -RaycastDirectionForwards);

        Physics.Raycast(BackwardDirRay, out RaycastHit Back1Hit, RayDistaceBackwards * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Back1Hit;
    }

    public RaycastHit BackwardsHitting2()
    {
        RaycastDirectionForwards = transform.forward;
        BackwardDirRay2 = new Ray(RaycastBackwardsEmpty2.transform.position + RaycastOffsetBackwards2, -RaycastDirectionForwards);

        Physics.Raycast(BackwardDirRay2, out RaycastHit Back2Hit, RayDistaceBackwards2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Back2Hit;
    }

    public RaycastHit RightHitting()
    {
        RaycastDirectionRight = transform.right;
        RightDirRay = new Ray(RaycastRightEmpty.transform.position + RaycastOffsetRight + TotalRaycastPosOffset, RaycastDirectionRight);

        Physics.Raycast(RightDirRay, out RaycastHit Right1Hit, RayDistaceRight * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Right1Hit;
    }

    public RaycastHit RightHitting2()
    {
        RaycastDirectionRight = transform.right;
        RightDirRay2 = new Ray(RaycastRightEmpty2.transform.position + RaycastOffsetRight2 + TotalRaycastPosOffset, RaycastDirectionRight);

        Physics.Raycast(RightDirRay2, out RaycastHit Right2Hit, RayDistaceRight2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Right2Hit;
    }

    public RaycastHit LeftHitting()
    {
        RaycastDirectionRight = transform.right;
        LeftDirRay = new Ray(RaycastLeftEmpty.transform.position + RaycastOffsetLeft + TotalRaycastPosOffset, -RaycastDirectionRight);

        Physics.Raycast(LeftDirRay, out RaycastHit Left1Hit, RayDistaceLeft * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Left1Hit;
    }

    public RaycastHit LeftHitting2()
    {
        RaycastDirectionRight = transform.right;
        LeftDirRay2 = new Ray(RaycastLeftEmpty2.transform.position + RaycastOffsetLeft2 + TotalRaycastPosOffset, -RaycastDirectionRight);

        Physics.Raycast(LeftDirRay2, out RaycastHit Left2Hit, RayDistaceLeft2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);

        return Left2Hit;
    }










    // Is touching bools for return

    public bool TouchingGround()
    {
        GroundCheck = new Ray(GroundCheckEmpty.transform.position + GroundCheckOffset + TotalRaycastPosOffset, -RaycastDirectionUp);

        return Physics.Raycast(GroundCheck, RayDistaceDown * TotalRaycastDistanceOffset, GroundLayer);
    }

    public bool HeadTouch()
    {
        HeadCheck = new Ray(HeadCheckEmpty.transform.position + HeadCheckOffset + TotalRaycastPosOffset, RaycastDirectionUp);

        return Physics.Raycast(HeadCheck, RayDistaceUp * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }



    public bool IsForwardsHitting()
    {
        RaycastDirectionForwards = transform.forward;
        ForwardDirRay = new Ray(RaycastForwardsEmpty.transform.position + RaycastOffsetForwards + TotalRaycastPosOffset, RaycastDirectionForwards);
        return Physics.Raycast(ForwardDirRay, RayDistaceForwards * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsForwardsHitting2()
    {
        RaycastDirectionForwards = transform.forward;
        ForwardDirRay2 = new Ray(RaycastForwardsEmpty2.transform.position + RaycastOffsetForwards2 + TotalRaycastPosOffset, RaycastDirectionForwards);
        return Physics.Raycast(ForwardDirRay2, RayDistaceForwards2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsBackwardsHitting()
    {
        RaycastDirectionForwards = transform.forward;
        BackwardDirRay = new Ray(RaycastBackwardsEmpty.transform.position + RaycastOffsetBackwards, -RaycastDirectionForwards);
        return Physics.Raycast(BackwardDirRay, RayDistaceBackwards * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsBackwardsHitting2()
    {
        RaycastDirectionForwards = transform.forward;
        BackwardDirRay2 = new Ray(RaycastBackwardsEmpty2.transform.position + RaycastOffsetBackwards2, -RaycastDirectionForwards);
        return Physics.Raycast(BackwardDirRay2, RayDistaceBackwards2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsRightHitting()
    {
        RaycastDirectionRight = transform.right;
        RightDirRay = new Ray(RaycastRightEmpty.transform.position + RaycastOffsetRight + TotalRaycastPosOffset, RaycastDirectionRight);
        return Physics.Raycast(RightDirRay, RayDistaceRight * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsRightHitting2()
    {
        RaycastDirectionRight = transform.right;
        RightDirRay2 = new Ray(RaycastRightEmpty2.transform.position + RaycastOffsetRight2 + TotalRaycastPosOffset, RaycastDirectionRight);
        return Physics.Raycast(RightDirRay2, RayDistaceRight2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsLeftHitting()
    {
        RaycastDirectionRight = transform.right;
        LeftDirRay = new Ray(RaycastLeftEmpty.transform.position + RaycastOffsetLeft + TotalRaycastPosOffset, -RaycastDirectionRight);
        return Physics.Raycast(LeftDirRay, RayDistaceLeft * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    public bool IsLeftHitting2()
    {
        RaycastDirectionRight = transform.right;
        LeftDirRay2 = new Ray(RaycastLeftEmpty2.transform.position + RaycastOffsetLeft2 + TotalRaycastPosOffset, -RaycastDirectionRight);
        return Physics.Raycast(LeftDirRay2, RayDistaceLeft2 * TotalRaycastDistanceOffset, RaycastXYZLayerMask);
    }

    
    


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, alpha);
        Gizmos.DrawRay(RaycastForwardsEmpty.transform.position + RaycastOffsetForwards + TotalRaycastPosOffset, transform.forward * RayDistaceForwards * TotalRaycastDistanceOffset);
        Gizmos.DrawRay(RaycastForwardsEmpty2.transform.position + RaycastOffsetForwards2, transform.forward * RayDistaceForwards * TotalRaycastDistanceOffset);

        Gizmos.DrawRay(RaycastBackwardsEmpty.transform.position + RaycastOffsetBackwards + TotalRaycastPosOffset, -transform.forward * RayDistaceBackwards * TotalRaycastDistanceOffset);
        Gizmos.DrawRay(RaycastBackwardsEmpty2.transform.position + RaycastOffsetBackwards2 + TotalRaycastPosOffset, -transform.forward * RayDistaceBackwards2 * TotalRaycastDistanceOffset);

        Gizmos.DrawRay(RaycastRightEmpty.transform.position + RaycastOffsetRight + TotalRaycastPosOffset, transform.right * RayDistaceRight * TotalRaycastDistanceOffset);
        Gizmos.DrawRay(RaycastRightEmpty2.transform.position + RaycastOffsetRight2 + TotalRaycastPosOffset, transform.right * RayDistaceRight2 * TotalRaycastDistanceOffset);

        Gizmos.DrawRay(RaycastLeftEmpty.transform.position + RaycastOffsetLeft + TotalRaycastPosOffset, -transform.right * RayDistaceLeft * TotalRaycastDistanceOffset);
        Gizmos.DrawRay(RaycastLeftEmpty2.transform.position + RaycastOffsetLeft2 + TotalRaycastPosOffset, -transform.right * RayDistaceLeft2 * TotalRaycastDistanceOffset);

        Gizmos.DrawRay(GroundCheckEmpty.transform.position + GroundCheckOffset + TotalRaycastPosOffset, -RaycastDirectionUp * RayDistaceDown * TotalRaycastDistanceOffset);
        Gizmos.DrawRay(HeadCheckEmpty.transform.position + HeadCheckOffset + TotalRaycastPosOffset, RaycastDirectionUp * RayDistaceUp * TotalRaycastDistanceOffset);

        //Debug.Log(ForwardsHitting());
    }

}
