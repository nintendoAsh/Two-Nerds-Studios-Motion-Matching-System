using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

public class MotionMatching : MonoBehaviour
{
    public float Cost = 0.5f;
    public float TimeBetweenSerches = 0.1f;
    public DataSet MMData;
    public float FutureTrejectoryWeight = 1f;
    public float RotationWeight = 1f;
    public PlayerMixedMotionMatching TrejectoryMaker;
    float deltaTimeAdd = 0f;
    float TrejectoryCost;
    float BoneRotationCost;
    public Animator Ani;
    List<AnimationClip> Clips = new List<AnimationClip>();
    PlayableGraph graph;
    List<AnimationClipPlayable> AnimationPlayables = new List<AnimationClipPlayable>();
    public int LastMatchedFrame = 0;
    public GameObject Root;
    public GameObject Scale; //to normalize scale of everything to the corect scale. will multiply postions by this game objects scale value
    float BonePositionCost;
    float BoneVelocityCost;
    public List<AssignableBone> Bones = new List<AssignableBone>();
    int frame=0;
    Vector3 ScaleOffset = new Vector3(1,1,1);
    AnimationMixerPlayable Mixer;
    int runAmount;

    public float Clip1Wight = 1;
    public float Clip2Weight = 1;
    int LastMatchedAnimationClip = 0;
    int count = 0;
    List<Vector3> BonesLastPosition = new List<Vector3>();
    //public float VelocityWeight = 0.5f;

    int BestFrame = 0;
    float BestCost = 999999999f;
    public int FarFrame = 15;

      float TrejectoryCostTemp =0f;
                float RotCostTemp =0f;
                float BonePosCostTemp =0f;
                float BoneVolCostTemp =0f;
                float BoneRotCostTemp = 0f;

                public float VelocityMultiplyer = 1f;
                bool IsSearching = false;
                public float ChangeThreshold = 1f;
                public float ClipChangePenalty = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < Bones.Count; i++)
        {
            BonesLastPosition.Add(Vector3.zero);
        }
        graph = PlayableGraph.Create("MotionMatching"); //creates a playable graph
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var output = AnimationPlayableOutput.Create(graph, "Animation", Ani);//animation output (this is where the playable graph output goes)
        Mixer = AnimationMixerPlayable.Create(graph, Clips.Count);
        for(int i = 0; i < Clips.Count; i++)
        {
            AnimationPlayables.Add(AnimationClipPlayable.Create(graph, Clips[i])); // turn animation clip into a unity playable 
            graph.Connect(AnimationPlayables[i], 0, Mixer, i);
            Mixer.SetInputWeight(i, 0);
        }

        output.SetSourcePlayable(Mixer); //sets playable graph output to output

        if(Scale != null)
        {
            ScaleOffset = Scale.transform.localScale;
            //Debug.Log("Scale Offset: " + ScaleOffset);
        }
        graph.Play(); 
    }

    float CalculateCost(int i)
    {
        TrejectoryCost = 0;
        BonePositionCost = 0;
        BoneVelocityCost = 0;
        BoneRotationCost = 0;
        float ClipPenalty = 0;

        for(int j = 0; j < TrejectoryMaker.FutureFramesAhead.Count; j++)
        {
            TrejectoryCost += (FutureTrejectoryWeight / TrejectoryMaker.FutureFramesAhead.Count) * (Root.transform.InverseTransformPoint(TrejectoryMaker.PosibleFuturePositions[j]) - new Vector3(MMData.Frames[i].FutureTrejectoryPositions[j].x * ScaleOffset.x, MMData.Frames[i].FutureTrejectoryPositions[j].y * ScaleOffset.y, MMData.Frames[i].FutureTrejectoryPositions[j].z * ScaleOffset.z)).sqrMagnitude;
        }
            
        float RotationCost = RotationWeight * new Vector4(Root.transform.rotation.x - MMData.Frames[i].CurrentDirection.x, Root.transform.rotation.y - MMData.Frames[i].CurrentDirection.y, Root.transform.rotation.z - MMData.Frames[i].CurrentDirection.z, Root.transform.rotation.w - MMData.Frames[i].CurrentDirection.w).sqrMagnitude;

        for(int j = 0; j < Bones.Count; j++)
        {
            BonePositionCost += Bones[j].BonePositionWeight * (Root.transform.InverseTransformPoint(Bones[j].Bone.transform.position) - new Vector3(MMData.Frames[i].BoneData[j].BonePositions.x * ScaleOffset.x, MMData.Frames[i].BoneData[j].BonePositions.y * ScaleOffset.y, MMData.Frames[i].BoneData[j].BonePositions.z * ScaleOffset.z)).sqrMagnitude;
            BoneRotationCost += Bones[j].BoneRotationWeight * new Vector4(Bones[j].Bone.transform.localRotation.x - MMData.Frames[i].BoneData[j].BoneRotations.x, Bones[j].Bone.transform.localRotation.y - MMData.Frames[i].BoneData[j].BoneRotations.y, Bones[j].Bone.transform.localRotation.z - MMData.Frames[i].BoneData[j].BoneRotations.z, Bones[j].Bone.transform.localRotation.w - MMData.Frames[i].BoneData[j].BoneRotations.w).sqrMagnitude;
            BoneVelocityCost += Bones[j].BoneVelocityWeight * (((Root.transform.InverseTransformPoint(Bones[j].Bone.transform.position) - BonesLastPosition[j]) / deltaTimeAdd) * VelocityMultiplyer - new Vector3(MMData.Frames[i].BoneData[j].BoneVelocities.x * ScaleOffset.x, MMData.Frames[i].BoneData[j].BoneVelocities.y * ScaleOffset.y, MMData.Frames[i].BoneData[j].BoneVelocities.z * ScaleOffset.z)).sqrMagnitude;
        }

        if(LastMatchedAnimationClip != MMData.Frames[i].AnimationClipNumber)
        {
            ClipPenalty = ClipChangePenalty;
        }

        float TotalCost = TrejectoryCost + RotationCost + BonePositionCost + BoneVelocityCost + ClipPenalty;

        if(IsSearching == true)
        {
            if(TotalCost < BestCost && Mathf.Abs(i - LastMatchedFrame) > FarFrame)
            {
                BestCost = TotalCost;
                BestFrame = i;
                //Debug.Log("BEST FRAME CHANGED: " + BestFrame + "Trejectory: " + TrejectoryCost + " RotationCost: " + RotationCost + " BonePositionCost: " + BonePositionCost + " Bone Velocity Cost: " + BoneVelocityCost + " Total: " + BestCost);

                TrejectoryCostTemp = TrejectoryCost;
                RotCostTemp = RotationCost;
                BonePosCostTemp = BonePositionCost;
                BoneVolCostTemp = BoneVelocityCost;
                BoneRotCostTemp = BoneRotationCost;
            }
        }

        return TotalCost;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeAdd += Time.deltaTime;

        if(deltaTimeAdd >= TimeBetweenSerches)
        {
            BestCost = float.PositiveInfinity;
            BestFrame = -1;
            IsSearching = true;
          
            for(int i = 0; i < MMData.Frames.Count; i++)
            {
                float TotalCost = CalculateCost(i);

                if(i == MMData.Frames.Count-1)
                {
                    IsSearching = false;
                    Debug.Log("BEST FRAME Set: " + BestFrame + "Trejectory: " + TrejectoryCostTemp + " RotationCost: " + RotCostTemp + " BonePositionCost: " + BonePosCostTemp + " Bone Rotation Cost: " + BoneRotCostTemp + " Bone Velocity Cost: " + BoneVolCostTemp + " Total: " + BestCost);
                }
                
            }

            Debug.Log("Current Cost" + CalculateCost((int)Mathf.Round((float)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate)) + " Frame: " + (int)Mathf.Round((int)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate) + " FrameRate: " + Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate + " Time: " + AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime());

            if(BestFrame != LastMatchedFrame && BestFrame != -1 && CalculateCost((int)Mathf.Round((float)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate)) > ChangeThreshold)
            {
                LastMatchedFrame = BestFrame;
                frame = BestFrame;
                int AnimationID = MMData.Frames[BestFrame].AnimationClipNumber;
                if(LastMatchedAnimationClip != AnimationID)
                {
                    Mixer.SetInputWeight(LastMatchedAnimationClip, 0);
                    Mixer.SetInputWeight(AnimationID, 1);
                    //Debug.Log("last animation clip was  not the same");
                }
                LastMatchedAnimationClip = AnimationID;
                int AnimationGlobalFrame=0;
                for(int j = 1; j < AnimationID; j++)//subtracts one by starting with one so only looks at frames before current animation to isolate current animation/frame 
                {
                    AnimationGlobalFrame += MMData.AnimationClips[j].FrameAmount;
                    //Debug.Log("For Loop Running");
                }
                //Debug.Log("LocalFrame: " + (i - AnimationGlobalFrame));
                Debug.Log("MATCH FOUND! global Frame #: " + BestFrame + " Local Frame#: " + (BestFrame - AnimationGlobalFrame) + " Animation ID: " + AnimationID  + " Frame Cost: " + BestCost);
                Debug.Log("Real: " + ((Root.transform.InverseTransformPoint(Bones[0].Bone.transform.position) - BonesLastPosition[0]) / deltaTimeAdd) + " Saved: " + new Vector3(-MMData.Frames[frame].BoneData[0].BoneVelocities.x * ScaleOffset.x, -MMData.Frames[frame].BoneData[0].BoneVelocities.y * ScaleOffset.y, -MMData.Frames[frame].BoneData[0].BoneVelocities.z * ScaleOffset.z) + " Last Pos: " + BonesLastPosition[0] + " Current Pos: " + Root.transform.InverseTransformPoint(Bones[0].Bone.transform.position));
                AnimationPlayables[AnimationID].SetTime((BestFrame - AnimationGlobalFrame)/Clips[AnimationID].frameRate);//set the corect frame to be played
                graph.Evaluate();
                //graph.Play();//plays the animation starting from that frame
            }
            else
            {
                graph.Play();
                Debug.LogWarning("Last Selection was Last selected Frame or current frame was good enough");
            }

            for(int i = 0; i < Bones.Count; i++)
            {
                BonesLastPosition[i] = Root.transform.InverseTransformPoint(Bones[i].Bone.transform.position);
            }
            Debug.Log("Time Since Last Search" + deltaTimeAdd);
            deltaTimeAdd = 0f;
        }

        //Debug.Log(AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate);
    }

    void OnValidate()
    {
        if(MMData != null && Bones.Count < 1)
        {
            for(int i = 0; i < MMData.Bones.Count; i++)
            {
                Bones.Add(new AssignableBone{Name = MMData.Bones[i].BoneName, BoneID = MMData.Bones[i].BoneID});
            }
        }

        if(MMData != null && Clips.Count < 1)
        {
            for(int i = 0; i < MMData.AnimationClips.Count; i++)
            {
                Clips.Add(MMData.AnimationClips[i].Clip);
            }
        }
    }

     void OnDisable()
    {
        // Destroys all Playables and PlayableOutputs created by the graph.
        graph.Destroy();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        for(int i = 0; i < MMData.Bones.Count; i++)
        {
        Gizmos.DrawSphere(new Vector3(MMData.Frames[frame].BoneData[i].BonePositions.x * ScaleOffset.x, MMData.Frames[frame].BoneData[i].BonePositions.y * ScaleOffset.y, MMData.Frames[frame].BoneData[i].BonePositions.z * ScaleOffset.z) + Root.transform.position, 0.5f);
        }

        for(int i = 0; i < MMData.Frames[frame].FutureTrejectoryPositions.Count; i++)
        {
            if(i == 0)
            {
                Gizmos.color = Color.yellow;
            }

            if(i == 1)
            {
                Gizmos.color = Color.black;
            }

            if(i == 2)
            {
                Gizmos.color = Color.white;
            }

            if(i == 3)
            {
                Gizmos.color = Color.orange;
            }

            if(i == 4)
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawSphere(new Vector3(MMData.Frames[frame].FutureTrejectoryPositions[i].x * ScaleOffset.x, MMData.Frames[frame].FutureTrejectoryPositions[i].y * ScaleOffset.y, MMData.Frames[frame].FutureTrejectoryPositions[i].z * ScaleOffset.z) + Root.transform.position, 0.5f);
        }
    }


}

[System.Serializable]
public class AssignableBone
{
    public string Name = "bone";
    public int BoneID;
    public GameObject Bone;
    public float BonePositionWeight = 1f;
    public float BoneRotationWeight = 1f;
    public float BoneVelocityWeight = 1f;
}


