//the runtime heart. this scipt controlls the playback and selection of animation frames
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

public class MotionMatching : MonoBehaviour
{
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
    int LastMatchedAnimationClip = -1;
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
            Mixer.SetInputWeight(i, 0);//sets all mixer weights to zero so no clip has control over the charecter 
        }

        output.SetSourcePlayable(Mixer); //sets playable graph output to output

        if(Scale != null)
        {
            ScaleOffset = Scale.transform.localScale;
            //Debug.Log("Scale Offset: " + ScaleOffset);
        }
        graph.Play(); 
    }

    float CalculateCost(int i) //calculates and returns the cost value by comparing the current frame to the spesifyed frame (i)
    {
        TrejectoryCost = 0;
        BonePositionCost = 0;
        BoneVelocityCost = 0;
        BoneRotationCost = 0;
        float ClipPenalty = 0;

        for(int j = 0; j < TrejectoryMaker.FutureFramesAhead.Count; j++) //accumulates a cost for each trejectory feature into a single trejectory cost
        {
            TrejectoryCost += (FutureTrejectoryWeight / TrejectoryMaker.FutureFramesAhead.Count) * (Root.transform.InverseTransformPoint(TrejectoryMaker.PosibleFuturePositions[j]) - new Vector3(MMData.Frames[i].FutureTrejectoryPositions[j].x * ScaleOffset.x, MMData.Frames[i].FutureTrejectoryPositions[j].y * ScaleOffset.y, MMData.Frames[i].FutureTrejectoryPositions[j].z * ScaleOffset.z)).sqrMagnitude; //uses weighted squared distance to calculate cost
            //Debug.Log("FutureFramesAhead Numb: " + j + " Total accumulated cost: " + TrejectoryCost + " TrejectoryMaker Point: " + Root.transform.InverseTransformPoint(TrejectoryMaker.PosibleFuturePositions[j]) + " Dataset: " + new Vector3(MMData.Frames[i].FutureTrejectoryPositions[j].x * ScaleOffset.x, MMData.Frames[i].FutureTrejectoryPositions[j].y * ScaleOffset.y, MMData.Frames[i].FutureTrejectoryPositions[j].z * ScaleOffset.z));
        }
            
        float RotationCost = RotationWeight * new Vector4(Root.transform.rotation.x - MMData.Frames[i].CurrentDirection.x, Root.transform.rotation.y - MMData.Frames[i].CurrentDirection.y, Root.transform.rotation.z - MMData.Frames[i].CurrentDirection.z, Root.transform.rotation.w - MMData.Frames[i].CurrentDirection.w).sqrMagnitude; //converts a Quaternion into a vector4 so it can be used with .sqrMagnitude also while it does that it manualy does the difrence by for each valued added to the vector it does Dataset.x - RealRotation.x

        for(int j = 0; j < Bones.Count; j++) //it goes through every bone in the bone list. checking each bones postion roation and velocity and making a cost for each of the features
        {
            BonePositionCost += Bones[j].BonePositionWeight * (Root.transform.InverseTransformPoint(Bones[j].Bone.transform.position) - new Vector3(MMData.Frames[i].BoneData[j].BonePositions.x * ScaleOffset.x, MMData.Frames[i].BoneData[j].BonePositions.y * ScaleOffset.y, MMData.Frames[i].BoneData[j].BonePositions.z * ScaleOffset.z)).sqrMagnitude;
            BoneRotationCost += Bones[j].BoneRotationWeight * new Vector4(Bones[j].Bone.transform.localRotation.x - MMData.Frames[i].BoneData[j].BoneRotations.x, Bones[j].Bone.transform.localRotation.y - MMData.Frames[i].BoneData[j].BoneRotations.y, Bones[j].Bone.transform.localRotation.z - MMData.Frames[i].BoneData[j].BoneRotations.z, Bones[j].Bone.transform.localRotation.w - MMData.Frames[i].BoneData[j].BoneRotations.w).sqrMagnitude;
            BoneVelocityCost += Bones[j].BoneVelocityWeight * (((Root.transform.InverseTransformPoint(Bones[j].Bone.transform.position) - BonesLastPosition[j]) / deltaTimeAdd) * VelocityMultiplyer - new Vector3(MMData.Frames[i].BoneData[j].BoneVelocities.x * ScaleOffset.x, MMData.Frames[i].BoneData[j].BoneVelocities.y * ScaleOffset.y, MMData.Frames[i].BoneData[j].BoneVelocities.z * ScaleOffset.z)).sqrMagnitude;
        }

        if(LastMatchedAnimationClip != MMData.Frames[i].AnimationClipNumber) //if the clip that you are looking at is diffrent from the frame that is currently playing
        {
            ClipPenalty = ClipChangePenalty; //add a small penitly to be added into the total cost
        }

        float TotalCost = TrejectoryCost + RotationCost + BonePositionCost + BoneVelocityCost + ClipPenalty; //adds up all the cost values into a total cost for that frame.

        if(IsSearching == true) // if you are searching through the dataset and not just taking a quick glane at a few frames for the cost threathold 
        {
            if(TotalCost < BestCost && Mathf.Abs(i - LastMatchedFrame) > FarFrame)//if the current frames cost is better then the last best frames cost then change the current frame into the new best frame
            {
                BestCost = TotalCost; 
                BestFrame = i;
                //Debug.Log("BEST FRAME CHANGED: " + BestFrame + "Trejectory: " + TrejectoryCost + " RotationCost: " + RotationCost + " BonePositionCost: " + BonePositionCost + " Bone Velocity Cost: " + BoneVelocityCost + " Total: " + BestCost);
//Debug Stuff-----------------=-------------------------
                TrejectoryCostTemp = TrejectoryCost;
                RotCostTemp = RotationCost;
                BonePosCostTemp = BonePositionCost;
                BoneVolCostTemp = BoneVelocityCost;
                BoneRotCostTemp = BoneRotationCost;
//-----------------------------------------------------
            }
        }

        return TotalCost;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeAdd += Time.deltaTime; //stores delta Time which will turn deltaTimeAdd into a make shift timer

        if(deltaTimeAdd >= TimeBetweenSerches) //if the Timer (deltaTimeAdd) reaches the set time between searches then start dataset/frame searches
        {
            //resets values so data from last search doesnt carry over
            BestCost = float.PositiveInfinity;
            BestFrame = -1;
            IsSearching = true;//sets it to searching so that it will store the best frame it sees
          
            for(int i = 0; i < MMData.Frames.Count; i++)//searches through every frame
            {
                CalculateCost(i);

                if(i == MMData.Frames.Count-1) //if you hit the last frame stop searching. and if the debug.log is uncomented then show what it contributing to the total cost
                {
                    IsSearching = false;
                    Debug.Log("BEST FRAME Set: " + BestFrame + "Trejectory: " + TrejectoryCostTemp + " RotationCost: " + RotCostTemp + " BonePositionCost: " + BonePosCostTemp + " Bone Rotation Cost: " + BoneRotCostTemp + " Bone Velocity Cost: " + BoneVolCostTemp + " Total: " + BestCost);
                }
                
            }

            //Debug.Log("Current Cost" + CalculateCost((int)Mathf.Round((float)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate)) + " Frame: " + (int)Mathf.Round((int)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate) + " FrameRate: " + Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate + " Time: " + AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime());

            if(BestFrame != LastMatchedFrame && BestFrame != -1 /*&& CalculateCost((int)Mathf.Round((float)AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate)) > ChangeThreshold*/) //if current matched frame isnt the same as last matched frame and best frame has been assinged and... the current frames cost COMPARED TO ITSELF isnt bigger then the cost threathold play the frame
            {
                LastMatchedFrame = BestFrame;
                frame = BestFrame;//why dont I just use LastMatchedFrame or best frame... fuck my code is ass
                int AnimationID = MMData.Frames[BestFrame].AnimationClipNumber; //finds the animation ID in the frames data so that the corect frame of the corect clip is played and not like frame 30 of animation 1 instead of frame 30 of animation 0 like its suposed to be
                if(LastMatchedAnimationClip != AnimationID) //if the new animation frame uses a diffrent animation clip (ID) then change the weights so that the new animation clip takes control of the charecter
                {
                    if(LastMatchedAnimationClip < 0)
                    {
                        Mixer.SetInputWeight(AnimationID, 1);//sets the new animation clip to 1 aka full effect
                    }
                    else
                    {
                        Mixer.SetInputWeight(LastMatchedAnimationClip, 0); //sets the last animation clips weight to zero aka no effect
                        Mixer.SetInputWeight(AnimationID, 1);//sets the new animation clip to 1 aka full effect
                        Debug.Log("last animation clip was  not the same");
                    }
                }
                LastMatchedAnimationClip = AnimationID;
                int AnimationGlobalFrame=0;//sets the playback offset to zero. since the dataset has every frame in one big list not divided by animation clip you need to subtract all the frame that came before that clips first frame so the playback works and doesnt play the wrong or just jump to the last frame
                for(int j = 1; j < AnimationID; j++)//subtracts one by starting with one so only looks at frames before current animation to isolate everything before current animation/frame. it adds up all the frame counts of these animation clips before the clip we want to play so then it can subtract it from the global frame index and get the local frame value so it can properly playback 
                {
                    AnimationGlobalFrame += MMData.AnimationClips[j].FrameAmount;
                    //Debug.Log("For Loop Running");
                }
                //Debug.Log("LocalFrame: " + (i - AnimationGlobalFrame));
                Debug.Log("MATCH FOUND! global Frame #: " + BestFrame + " Local Frame#: " + (BestFrame - AnimationGlobalFrame) + " Animation ID: " + AnimationID  + " Frame Cost: " + BestCost);
                //Debug.Log("Real: " + ((Root.transform.InverseTransformPoint(Bones[0].Bone.transform.position) - BonesLastPosition[0]) / deltaTimeAdd) + " Saved: " + new Vector3(-MMData.Frames[frame].BoneData[0].BoneVelocities.x * ScaleOffset.x, -MMData.Frames[frame].BoneData[0].BoneVelocities.y * ScaleOffset.y, -MMData.Frames[frame].BoneData[0].BoneVelocities.z * ScaleOffset.z) + " Last Pos: " + BonesLastPosition[0] + " Current Pos: " + Root.transform.InverseTransformPoint(Bones[0].Bone.transform.position));
                AnimationPlayables[AnimationID].SetTime((BestFrame - AnimationGlobalFrame)/Clips[AnimationID].frameRate);//set the corect frame to be played
                graph.Evaluate(); //moves the play head
                //graph.Play();//plays the animation starting from that frame
            }
            else
            {
                graph.Play();//keep playing the animation clip till next search/match
                Debug.LogWarning("Last Selection was Last selected Frame or current frame was good enough");
            }

            for(int i = 0; i < Bones.Count; i++) //goes through each bone and stores its current postion so that when it generates a velocity feature it can get the delta postion
            {
                BonesLastPosition[i] = Root.transform.InverseTransformPoint(Bones[i].Bone.transform.position);
            }
            //Debug.Log("Time Since Last Search" + deltaTimeAdd);
            deltaTimeAdd = 0f;//resets the timer
        }

        //Debug.Log(AnimationPlayables[MMData.Frames[LastMatchedFrame].AnimationClipNumber].GetTime() * Clips[MMData.Frames[LastMatchedFrame].AnimationClipNumber].frameRate);
    }

    void OnValidate() //when items in the inspector are changed
    {
        if(MMData != null && Bones.Count < 1) //checks if the bone list is already populated and the Motion Matching dataset has been assinged
        {
            for(int i = 0; i < MMData.Bones.Count; i++)
            {
                Bones.Add(new AssignableBone{Name = MMData.Bones[i].BoneName, BoneID = MMData.Bones[i].BoneID});//adds and names assingeableBones to the bones list in the inspector
            }
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        if(MMData != null && Clips.Count < 1)
        {
            for(int i = 0; i < MMData.AnimationClips.Count; i++)
            {
                Clips.Add(MMData.AnimationClips[i].Clip);//adds the animation clips to the clips list for playback
            }
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        
    }

     void OnDisable()
    {
        // Destroys all Playables and PlayableOutputs created by the graph.
        graph.Destroy();
    }

    void OnDrawGizmos() //shows debug gizmos. I dont really want to explain it. its not that complicated. if you are looking at a motion matching script you are smart enough that you can proboly figure it out. I hope. or just pull a YO ʃæt dʒiː piː tiː
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
public class AssignableBone //the assingeableBones costom class that you see in the inspector
{
    public string Name = "bone"; //the name of the bone so you know which is which
    public int BoneID;//yeah this is kinda pointless since I just use the list ID
    public GameObject Bone;//the gameobeject is coraponds to
    
    //the settings for the amount the bone contributes to the total cost 
    public float BonePositionWeight = 1f;
    public float BoneRotationWeight = 1f;
    public float BoneVelocityWeight = 1f;
}


