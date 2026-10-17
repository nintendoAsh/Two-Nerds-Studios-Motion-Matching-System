using UnityEngine;

public class MovingGround : MonoBehaviour
{
    public PlayerMovement playerMovement;
    [SerializeField] private Rigidbody Rigidbody;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 Move = transform.position + new Vector3(0f, 0f, -playerMovement.TemporaryVelocity.z) * Time.fixedDeltaTime;
        Rigidbody.MovePosition(Move); 
    }
}
