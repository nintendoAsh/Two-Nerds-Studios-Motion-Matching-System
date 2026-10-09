using UnityEngine;
using System.Collections.Generic;

public class MotionMatcherDataSetDebugger : MonoBehaviour
{
    public DataSet data;
    //public Animator Ani;
    public int Count;
    public int MaxFrame;
    public int MinFrame = 0;
    float CurrentFrameRate;
    List<AnimationClip> Clips = new List<AnimationClip>();
    float deltaTimeTracker = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int ClipID = 0;
    int LastClipID = 0;
    int AnimationGlobalFrame = 0;
    int LastCount = 0;
    public GameObject Rig;
    public bool firstRun = true;
    public GameObject Root;
    public List<bool> ClipsToPlay = new List<bool>();
    bool addmaxframe = true;
    public float ScaleOffset = 1f;
    public float FutureVelocityOffset1 = 0.5f;
    public float FutureVelocityOffset2 = 1f;
    public bool ShowTrejectory = true;
    public bool ShowRoot = true;
    public bool ShowRotation = true;
    public bool ShowFeetPos = true;
    public bool ShowFeetVol = true;

    public GameObject FootL;
    public GameObject FootR;
    public Vector3 BonesLastPosition = Vector3.zero;
    void Start()
    {
        for(int i = 0; i < data.AnimationClips.Count; i++)
        {
            Debug.Log(i);
            Clips.Add(data.AnimationClips[i].Clip);
            if(ClipsToPlay[i] != false && addmaxframe != false)
            {
                if(MaxFrame == 0)
                {
                    for(int j = 0; j < i; j++)//subtracts one by starting with one so only looks at frames before current animation to isolate current animation/frame 
                    {
                        MinFrame += data.AnimationClips[j].FrameAmount;
                        //Debug.Log("CLIP Frames: " + data.AnimationClips[j].FrameAmount + " Clip ID " + (j));

                    }
                }
                MaxFrame += data.AnimationClips[i].FrameAmount;
            }
            else if (MaxFrame > 0)
            {
                addmaxframe = false;
            }
        }
        MaxFrame = MaxFrame + MinFrame;

        for(int j = 0; j < ClipID; j++)//subtracts one by starting with one so only looks at frames before current animation to isolate current animation/frame 
        {
            AnimationGlobalFrame += data.AnimationClips[j].FrameAmount;
            //Debug.Log("For Loop Running");
        }

        Count = MinFrame;
        ClipID = data.Frames[Count].AnimationClipNumber;
        CurrentFrameRate = data.AnimationClips[ClipID].Clip.frameRate;

        
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeTracker += Time.deltaTime;
        //Debug.Log("Delta: " + deltaTimeTracker);
        if(deltaTimeTracker >= (1/CurrentFrameRate))
        {
            Count++;
            Debug.Log("Runtime: " + ((FootL.transform.position - Root.transform.position) - BonesLastPosition) / deltaTimeTracker + " Saved: " + data.Frames[Count].BoneData[0].BoneVelocities + " Time: " + deltaTimeTracker);
            BonesLastPosition = (FootL.transform.position - Root.transform.position);
            deltaTimeTracker = 0;
            //Debug.Log("Count: " + Count + " ClipID: " + ClipID);
            //Debug.Log("Trying to add to count");
        }
        
//Debug.Log("Count: " + Count);
        if(Count >= MaxFrame || Count < MinFrame)
        {
            Count = MinFrame;
            //Debug.Log("Count: " + Count);
        }else if(Count <= data.Frames.Count)
        {
            
            
            ClipID = data.Frames[Count].AnimationClipNumber;

            if(ClipID != LastClipID || firstRun == true)
            {
                //Debug.Log("If StatmentRunning");
                AnimationGlobalFrame = 0;
                for(int j = 0; j < ClipID; j++)//subtracts one by starting with one so only looks at frames before current animation to isolate current animation/frame 
                {
                    AnimationGlobalFrame += data.AnimationClips[j].FrameAmount;
                    //Debug.Log("For Loop Running");
                }
                LastClipID = ClipID;
                CurrentFrameRate = Clips[ClipID].frameRate;
                firstRun = false;
            }

        }
        

        if(Count != LastCount)
        {
            //Debug.Log("Time: " + (Count - AnimationGlobalFrame)/CurrentFrameRate + " FrameRate: " + CurrentFrameRate + " LocalFrame: " + (Count - AnimationGlobalFrame) + " ClipID: " + ClipID + " LastClipID: " + LastClipID + " Pervous Frames: " + AnimationGlobalFrame);
            Clips[ClipID].SampleAnimation(Rig, (Count - AnimationGlobalFrame)/CurrentFrameRate);
            LastCount = Count;
        }
    }

    private int DebugDotLoop= 0;
    public float DebugSphereSize=0.5f;
    void OnDrawGizmos()
    {
        
        //Gizmos.DrawSphere(data.Frames[count].BoneData[0].BonePositions, 0.5f);

        for(int i = 0; i < data.Bones.Count; i++)
        {
            if(ShowFeetPos == true){
            Gizmos.color = Color.purple;
            Gizmos.DrawSphere(new Vector3(data.Frames[Count].BoneData[i].BonePositions.x * ScaleOffset + Rig.transform.position.x, data.Frames[Count].BoneData[i].BonePositions.y * ScaleOffset + Rig.transform.position.y, data.Frames[Count].BoneData[i].BonePositions.z * ScaleOffset + Rig.transform.position.z), DebugSphereSize);
            }

            if(ShowFeetVol == true){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3((data.Frames[Count].BoneData[i].BoneVelocities.x * ScaleOffset) * FutureVelocityOffset1 + Rig.transform.position.x + (data.Frames[Count].BoneData[i].BonePositions.x * ScaleOffset), (data.Frames[Count].BoneData[i].BoneVelocities.y * ScaleOffset) * FutureVelocityOffset1 + Rig.transform.position.y + (data.Frames[Count].BoneData[i].BonePositions.y * ScaleOffset), (data.Frames[Count].BoneData[i].BoneVelocities.z * ScaleOffset) * FutureVelocityOffset1 + Rig.transform.position.z + (data.Frames[Count].BoneData[i].BonePositions.z * ScaleOffset)), DebugSphereSize);
            Gizmos.DrawSphere(new Vector3((data.Frames[Count].BoneData[i].BoneVelocities.x * ScaleOffset) * FutureVelocityOffset2 + Rig.transform.position.x + (data.Frames[Count].BoneData[i].BonePositions.x * ScaleOffset), (data.Frames[Count].BoneData[i].BoneVelocities.y * ScaleOffset) * FutureVelocityOffset2 + Rig.transform.position.y + (data.Frames[Count].BoneData[i].BonePositions.y * ScaleOffset), (data.Frames[Count].BoneData[i].BoneVelocities.z * ScaleOffset) * FutureVelocityOffset2 + Rig.transform.position.z + (data.Frames[Count].BoneData[i].BonePositions.z * ScaleOffset)), DebugSphereSize);
            }
        }

        if(ShowRoot == true){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Root.transform.position, DebugSphereSize);
        }

        if(ShowRotation == true){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Root.transform.position + (data.Frames[Count].CurrentDirection * new Vector3(0,0,1)), DebugSphereSize);
        }

        if(ShowTrejectory == true)
        {
            Gizmos.color = Color.green;
            for(int i = 0; i< data.Frames[Count].FutureTrejectoryPositions.Count; i++)
            {
                Gizmos.DrawSphere(data.Frames[Count].FutureTrejectoryPositions[i] * ScaleOffset + Rig.transform.position, DebugSphereSize);
            }
        }


        
    }

    void OnValidate()
    {
        if(ClipsToPlay.Count == 0)
        {
            for(int i = 0; i < data.AnimationClips.Count; i++)
            {
                ClipsToPlay.Add(true);
            }
        }
    }
}
