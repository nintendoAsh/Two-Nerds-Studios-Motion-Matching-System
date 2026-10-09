using UnityEngine;

public class LockRootMotion : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnAnimatorMove()
    {
        transform.rotation = animator.deltaRotation; //this is a delta so I should make it build on itself over time but havent gotten there yet. 
    }
}
