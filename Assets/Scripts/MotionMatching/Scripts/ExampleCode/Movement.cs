using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour
{
    public Rigidbody rigidbody;

    public float speed;

    [HideInInspector] public float MoveX;
    [HideInInspector] public float MoveZ;

    public bool ForeverRunForwards = false;

    void Update()
    {
        if(ForeverRunForwards == false){
        rigidbody.linearVelocity = new Vector3(MoveX * speed, 0f, MoveZ * speed);
        }
        else
        {
            MoveZ = 1f;
            rigidbody.linearVelocity = new Vector3(0f,0f,speed);
        }
    }

    public void OnMove(InputAction.CallbackContext Move)
    {
        Vector2 MoveVector = Move.ReadValue<Vector2>();
        MoveX = MoveVector.x;
        MoveZ = MoveVector.y;
    }
}
