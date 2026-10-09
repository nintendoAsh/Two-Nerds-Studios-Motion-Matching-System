using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RootTester : MonoBehaviour
{
    public DataSet data;
    public Animator Ani;
    //public GameObject Debug;
    public GameObject Root;
    int count = 0;
    float frametime = 0;
    GameObject Rig;
    public GameObject RigSorce;
    public AnimationClip clip;
    public AnimationClip clip2;
                    List<string> Paths = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        frametime += Time.deltaTime;
        if(count <= (clip.frameRate * clip.length))
        {       
        clip.SampleAnimation(Rig,count/clip.frameRate);
        }
        else
        {
            clip2.SampleAnimation(Rig,(count - (clip.frameRate * clip.length))/clip2.frameRate);
            Debug.Log("Clip 2 Frame: " + (count - (clip.frameRate * clip.length)) + " Frame Time: " + (count - (clip.frameRate * clip.length))/clip2.frameRate + " Count: " + count);
        }
/*
                        for(int k = 0; k < Paths.Count; k++)
                        {
                            Transform BoneTransform = Rig.transform.Find(Paths[k]);
                            Debug.Log(Paths[k]);
                            Debug.Log(" Rig Pos: " + Rig.transform.position + " BoneWorldPos: " + BoneTransform.position + " BoneLocalPos: " + data.Frames[count].BoneData[0].BonePositions + " Pos: "+  new Vector3(data.Frames[count].BoneData[0].BonePositions.x + Rig.transform.position.x,data.Frames[count].BoneData[0].BonePositions.y + Rig.transform.position.y,data.Frames[count].BoneData[0].BonePositions.z + Rig.transform.position.z));
                        }
                        */
                if(frametime >= 0.01667){
                        count++;
                        frametime=0;
                }
        
    }
    private int DebugDotLoop= 0;
    public float DebugSphereSize=0.5f;
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.purple;
        //Gizmos.DrawSphere(data.Frames[count].BoneData[0].BonePositions, 0.5f);

        for(int i = 0; i < data.Bones.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(data.Frames[count].BoneData[i].BonePositions.x + Rig.transform.position.x,data.Frames[count].BoneData[i].BonePositions.y + Rig.transform.position.y,data.Frames[count].BoneData[i].BonePositions.z + Rig.transform.position.z), 0.5f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Root.transform.position, DebugSphereSize);

Gizmos.color = Color.green;
        for(int i = 0; i< data.Frames[count].FutureTrejectoryPositions.Count; i++)
        {
            Gizmos.DrawSphere(data.Frames[count].FutureTrejectoryPositions[i] + Rig.transform.position, 0.5f);
        }

        
    }


    void Start()
    {
         Rig = Instantiate(RigSorce);
                Rig.transform.position = Vector3.zero;
                Debug.Log("Test" + Rig.transform.position);
                Paths.Add("Human Form/Root/spine/thigh.L/shin.L/foot.L");
                 
    }


}

