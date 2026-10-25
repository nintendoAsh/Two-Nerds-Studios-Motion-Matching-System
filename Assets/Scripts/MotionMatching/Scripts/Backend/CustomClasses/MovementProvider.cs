using UnityEngine;


public abstract class MovementProvider : MonoBehaviour
{
    [HideInInspector] public Vector3 DesiredVelocity = Vector3.zero;
    [HideInInspector] public Vector3 PlayerPostion = Vector3.zero;
    [HideInInspector] public Vector3 PlayerVelocity = Vector3.zero;

    public abstract void UpdateValues();
}
