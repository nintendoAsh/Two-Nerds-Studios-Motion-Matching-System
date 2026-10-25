using UnityEngine;

public class RigidbodyMovementProvider : MovementProvider
{
    public Rigidbody rigidbody;
    public Movement movement;
    void Update()
    {
        PlayerPostion = rigidbody.position;
        PlayerVelocity = rigidbody.linearVelocity;
        DesiredVelocity = new Vector3(movement.speed * movement.MoveX, 0f, movement.speed * movement.MoveZ);
    }

    public override void UpdateValues()
    {
        PlayerPostion = rigidbody.position;
        PlayerVelocity = rigidbody.linearVelocity;
    }
}
