using UnityEngine;

public class Telaport : MonoBehaviour
{
    public GameObject RespawnPoint;
    public GameObject Player;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
        Player.transform.position = RespawnPoint.transform.position;
        }
    }
}
