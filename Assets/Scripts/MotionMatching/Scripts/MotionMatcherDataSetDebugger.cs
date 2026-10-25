// this code is so F'ed. I made it at 12am in the middle of game jam because I was off task
using UnityEngine;
using System.Collections.Generic;

public class MotionMatcherDataSetDebugger : MonoBehaviour
{
    public DataSet data;
    public int Count; //the frame that will be displayed/is currently being played
    public int MaxFrame; // the highest frame that will get played
    public int MinFrame = 0; // the lowest frame that will get played
    float CurrentFrameRate;
    List<AnimationClip> Clips = new List<AnimationClip>();
    float deltaTimeTracker = 0f; //delta time is added here as a make shift timer 
    int ClipID = 0;
    int LastClipID = 0;
    int AnimationGlobalFrame = 0;
    int LastCount = 0;
    public GameObject Rig;
    public bool firstRun = true; //is true durring the first update cycle
    public GameObject Root;
    public List<bool> ClipsToPlay = new List<bool>(); //List of animations to play. list filled when dataset is added in OnValidate
    bool addmaxframe = true; //if false it wont try to set the max frame again durring the inilization for loop in the Start() fuction
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
            Clips.Add(data.AnimationClips[i].Clip); //pulls the animation clips out of the dataset to a list in RAM
            if(ClipsToPlay[i] != false && addmaxframe != false) //Sets the max frame if you are setting the max frame.
            {
                if(MaxFrame == 0)// dont allow min frame to be larger then max frame. max frame MUST equal zero.
                {
                    for(int j = 0; j < i; j++)//runs through all the clips that have been gone through before this has run. inside the loop every clips frames count gets added to min frame 
                    {
                        MinFrame += data.AnimationClips[j].FrameAmount;//sets the min frame / starting offset.
                        //Debug.Log("CLIP Frames: " + data.AnimationClips[j].FrameAmount + " Clip ID " + (j));
                    }
                }
                MaxFrame += data.AnimationClips[i].FrameAmount; //accumulates all the frames of each clips for each one  
            }
            else if (MaxFrame > 0) //if max frame has been set then make sure it doesnt get set again
            {
                addmaxframe = false;
            }
        }
        MaxFrame = MaxFrame + MinFrame;//sets it to a global frame instead of a clip local postion frame number

        for(int j = 0; j < ClipID; j++) // why does this exist? this doent fuckin do anything.. WHAT! MY BRAIN AT 12 AM WAS WONKY NO WAY!
        {
            AnimationGlobalFrame += data.AnimationClips[j].FrameAmount; 
            //Debug.Log("For Loop Running");
        }

        Count = MinFrame; //set count to the starting offset
        ClipID = data.Frames[Count].AnimationClipNumber;//finds the frame clip ID that will be playing from the current count/frame
        CurrentFrameRate = data.AnimationClips[ClipID].Clip.frameRate; //using that current ClipID we find the clip and store the frame rate so we display the corect frames 
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeTracker += Time.deltaTime; //adds up the current frame times.
        //Debug.Log("Delta: " + deltaTimeTracker);
        if(deltaTimeTracker >= (1/CurrentFrameRate)) //if delta time is larger or equal to the time per frame then play next frame. (frame time is calculated by deviding our current frame rate by 1 to get how long each frame take Ex: 0.016s for a frame rate of 60fps)
        {
            Count++; //advances by 1 frame
            //Debug.Log("Runtime: " + ((FootL.transform.position - Root.transform.position) - BonesLastPosition) / deltaTimeTracker + " Saved: " + data.Frames[Count].BoneData[0].BoneVelocities + " Time: " + deltaTimeTracker);
            //BonesLastPosition = (FootL.transform.position - Root.transform.position);
            deltaTimeTracker = 0; // zeros the DeltaTimeTracker so we can start counting for the next frame
            //Debug.Log("Count: " + Count + " ClipID: " + ClipID);
            //Debug.Log("Trying to add to count");
        }
        
//Debug.Log("Count: " + Count);
        if(Count >= MaxFrame || Count < MinFrame) //if the frame is TOO low or too high it gets reset inside the set Min Max bounderys 
        {
            Count = MinFrame;//sets into the bounderys
            //Debug.Log("Count: " + Count);
        }else if(Count <= data.Frames.Count) //if we are not outside the bounderys keep playing
        {
            ClipID = data.Frames[Count].AnimationClipNumber; //gets the current animation clips

            if(ClipID != LastClipID || firstRun == true) //if the animaltion clip changes or hasents been set yet (firstRun)
            {
                //Debug.Log("If StatmentRunning");
                AnimationGlobalFrame = 0; // resets the global stating point for the current animation clip in the dataset (you see I have every frame in the dataset in one giant list which means if I look at frame 5 globaly frame 5 will be the fith frame of the first clip but if I want to play frame 5 of animation 2 and animation 2 starts in the datset at frame 30 that means I need to set the current global frame to 30 so that the player knows to offset the dataset frame playback by 30)
                for(int j = 0; j < ClipID; j++) // indartes through every animation clip before this one and adds each of their frame amounts to the AnimationGlobalFrame offset 
                {
                    AnimationGlobalFrame += data.AnimationClips[j].FrameAmount;
                    //Debug.Log("For Loop Running");
                }
                LastClipID = ClipID; //sets the last clip to current clip so it doesnt keep running over and over. plus since it was just changed the next time it will change or look for changes this was the LastClipID
                CurrentFrameRate = Clips[ClipID].frameRate; //sets the current frame rate. DUH
                firstRun = false;//then obously if this was the first time that Update ran we need set it to false becuae its first run is done
            }
        }
        

        if(Count != LastCount)//if the frame actaully changed
        {
            //Debug.Log("Time: " + (Count - AnimationGlobalFrame)/CurrentFrameRate + " FrameRate: " + CurrentFrameRate + " LocalFrame: " + (Count - AnimationGlobalFrame) + " ClipID: " + ClipID + " LastClipID: " + LastClipID + " Pervous Frames: " + AnimationGlobalFrame);
            Clips[ClipID].SampleAnimation(Rig, (Count - AnimationGlobalFrame)/CurrentFrameRate); //Sets the frame of the animation. converts the dataset global frame to local by subtracting the offset
            LastCount = Count;//self explaitory
        }
    }

    public float DebugSphereSize=0.5f;
    void OnDrawGizmos()//is run when unity is gonna display gizmos/update gizmos
    {
        //displays all of the data by extracting the data from each frame from the dataset.
        //some data has to be adjusted by a scale offset like bone postions. all of it has to have root postion added in becuase all of the data is local postion to the root.
        
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

    void OnValidate() //when something gets updated in the inspector
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
