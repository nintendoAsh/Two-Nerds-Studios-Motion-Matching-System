using UnityEngine;

public class MotionMatchingDebug : MonoBehaviour
{
    public DataSet MMData;
    public PlayerMixedMotionMatching TrejectoryMaker;
    public MotionMatching motionMatching;
    public GameObject Root;
    public float DebugDirectionLineLength = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private int DebugDotLoop= 0;
    public float DebugSphereSize=0.5f;
    void OnDrawGizmos()
    {

        for (int i = 0; i < TrejectoryMaker.PosibleFuturePositions.Count; i++)
        {
            Gizmos.color = Color.red;
            if(i == TrejectoryMaker.PosibleFuturePositions.Count - 1)
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawSphere(TrejectoryMaker.PosibleFuturePositions[i], DebugSphereSize);
            Gizmos.color = Color.purple;
            Gizmos.DrawSphere(Root.transform.position + MMData.Frames[motionMatching.LastMatchedFrame].FutureTrejectoryPositions[i], DebugSphereSize);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Root.transform.position, MMData.Frames[motionMatching.LastMatchedFrame].CurrentDirection * (Vector3.forward * DebugDirectionLineLength + Root.transform.position));
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Root.transform.position, Root.transform.rotation * (Root.transform.rotation * Vector3.forward * DebugDirectionLineLength + Root.transform.position));

            
        }   

        /*DebugDotLoop++;

        if (DebugDotLoop >= motionMatching.PosibleFuturePositions.Count)
        {
            DebugDotLoop = 0;
        }
        */
    }
}
