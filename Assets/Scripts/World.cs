using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject world;

    public GameObject Foot;
    public GameObject Shin;
    public GameObject Thigh;
    public GameObject Spine;
    public GameObject Root;
    public GameObject HumanForm;
    public GameObject RootMotion;
    public float SphereSize = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("World Space is: " + world.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
    Debug.Log("WorldPos Foot: " + Foot.transform.position + " Shin: " + Shin.transform.position + " Thigh: " + Thigh.transform.position + " Spine: " + Spine.transform.position + " Root: " + Root.transform.position + " HumanForm: " + HumanForm.transform.position + " RootMotion: " + RootMotion.transform.position);
       Debug.Log("LocalPos Foot: " + Foot.transform.localPosition + " Shin: " + Shin.transform.localPosition + " Thigh: " + Thigh.transform.localPosition + " Spine: " + Spine.transform.localPosition + " Root: " + Root.transform.localPosition + " HumanForm: " + HumanForm.transform.localPosition + " RootMotion: " + RootMotion.transform.localPosition);
       Debug.Log("Rot Foot: " + Foot.transform.rotation + " Shin: " + Shin.transform.rotation + " Thigh: " + Thigh.transform.rotation + " Spine: " + Spine.transform.rotation + " Root: " + Root.transform.rotation + " HumanForm: " + HumanForm.transform.rotation + " RootMotion: " + RootMotion.transform.rotation);

    }

     void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(world.transform.position,SphereSize);
    }
}
