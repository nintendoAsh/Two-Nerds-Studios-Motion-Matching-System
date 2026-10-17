using UnityEngine;

public class ReuseFloor : MonoBehaviour
{
    public GameObject ResetPoint;
   private void OnTriggerEnter(Collider other) 
   {
    other.gameObject.transform.position = ResetPoint.transform.position;
   }
}
