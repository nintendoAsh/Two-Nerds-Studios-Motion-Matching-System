using UnityEngine;

public class LockRootMotion : MonoBehaviour //locks root motion so that the charecter doesnt walk all over the place (though you could also do somthing like this but it uses a velocity and the chaercter actally move and follow. the orange duck did it that way)
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>(); 
    }
    void OnAnimatorMove()//takes control of root motion
    {
        transform.rotation = animator.deltaRotation; //this is a delta so I should make it build on itself over time but havent gotten there yet. (basiclly just allows roation so that the charecter can still change facing direction )
    }
}
