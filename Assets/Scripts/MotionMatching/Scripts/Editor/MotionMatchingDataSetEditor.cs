/*
Two Nerds Studios Motion Matching DataSet Maker
Programmed by: Asher Randall

Have fun figureing out how to use it! or how it works becuase even I somedays question it.

I would go on and put a nice quote here becuase like LOOK HOW FAR IVE COME WITH THE YEARS OF WORK but Im not in the mood tonight. lotta shit going down with my firends lives so yeah... anyways I still wanna say this even though I said I wasent.

-------------Find what you are willing to die for and live for it----------------------------

yada yada. sinefeld. good luck have fun don't die. and dont end up with HEE HEE MICHEL JACKSON MOONWALKING on all of your charecters
*/
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement; //another hack becuase unity wont give me the right data... fuck you unity and your undocumented UIToolkit and slightly more documented but still undocumented animation systems
using UnityEngine.SceneManagement;

public class MotionMatchingDataSetEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    public List<AnimationClip> AnimationClips = new List<AnimationClip>(); //Animation Clips
    public List<float> FutureTrejectoryTimes = new List<float>(); //The Future Trejectory Times
     List<BoneObjectsAndSettings> Bones = new List<BoneObjectsAndSettings>(); //The Bones and all of their settings
     List<string> BonePaths = new List<string>(); // a path to the bone from the root bone/first bone
     List<List<EditorCurveBinding>> BoneEditorCurvesPos = new List<List<EditorCurveBinding>>(); // first list contains main/root Bones or curves then the second nested list contains the X,Y,Z and the actaul Curve
     List<List<EditorCurveBinding>> BoneEditorCurvesRotQ = new List<List<EditorCurveBinding>>(); // same as Bone Editor pos but for rotation side note WE ARE HECKING IT UP WITH A LIL HACK!!! storeing both eular and Quaternion curves
     List<List<EditorCurveBinding>> BoneEditorCurvesRotE = new List<List<EditorCurveBinding>>(); // same as BoneEditorCurvesRotQ but for eular angles
    private ListView TrejectoryList; //this is the list you see in the inspector where you set future trejectory times
    private ListView AnimationList; //this is the list you see in the inspector where you add animations
    private ListView BonesList; // same as the trejectory list and animation list but for bones
    private Button BakeButton; // dude... the name is self explaintory why are you reading this
    public List<Vector3> RootPositions = new List<Vector3>(); //Stores ever single fuckin root postion. 
    int frame = 0; //literaly just here to be displayed as total frame count in the inspector

//----------Animation Curves for root motion
    AnimationCurve RootMotionCurveX;
    AnimationCurve RootMotionCurveY;
    AnimationCurve RootMotionCurveZ;
//--------Animation curves for the Quaternion rotation
    AnimationCurve RootRotationCurveQ_X;
    AnimationCurve RootRotationCurveQ_Y;
    AnimationCurve RootRotationCurveQ_Z;
    AnimationCurve RootRotationCurveQ_W;
//-------------------Root Motion Curves
    EditorCurveBinding RootMotionX = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.x" }; //since root motion is always in the same place in untiy we inialize it here instead of creating a new path and finding the curve and all tha jazz that we do for Bones
    EditorCurveBinding RootMotionY = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.y" };
    EditorCurveBinding RootMotionZ = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.z" };

    EditorCurveBinding RootRotationX = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.x" }; 
    EditorCurveBinding RootRotationY = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.y" };
    EditorCurveBinding RootRotationZ = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.z" };
    EditorCurveBinding RootRotationW = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.w" }; //Quaternions are weird
    ScriptableObject DataSet; // this is a refrance to the output dataset that is created on bake
    bool IsRecyling = false; //if this is true List View Bone Objects are being reused by unity
    VisualElement RigSourceRoot; 
    ObjectField RigSource; // the source rig object field shown in the inspector
    Scene TempBaking; // the temporary scene the bone data is calcuated in

    [MenuItem("Two Nerds Studios/Motion Matching/Motion Matching DataSet Editor")]
    public static void ShowExample()
    {
        MotionMatchingDataSetEditor wnd = GetWindow<MotionMatchingDataSetEditor>();
        wnd.titleContent = new GUIContent("Motion Matching DataSet Editor");
    }


    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement Title = new Label("TWO NERDS STUDIOS UNITY MOTION MATCHING EDITOR");//Name
        VisualElement Credits = new Label("BY ASHER RANDALL"); //Credits
        VisualElement Version = new Label("Version: 2.0 Alpha - UP TO HERE");// version. Named after tragiclly hip albums

        // Add all Text
        root.Add(Title); 
        root.Add(Credits);
        root.Add(Version);

    

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        AnimationList = root.Q<ListView>("AnimationList"); //finds the list view and inializes it I guess. or stores it? idk it finds it. it puts it in a verable then as you can see 1 line later sets the type of objects stored in the list view
        AnimationList.itemsSource = AnimationClips;

        TrejectoryList = root.Q<ListView>("FutureTrejectorys"); // same thing as the animation list but for future trejectorys
        TrejectoryList.itemsSource = FutureTrejectoryTimes;

        RigSourceRoot = root.Q<VisualElement>("RigSource"); // OwO Dis one diffrent. this finds a blank Visual Element named RigSource then (later) creates a child Object Field becuase unity doesnt allow you to do it in the kinda shitty UXML visual editor. well guess what bro... FUCK YOU I WONT DO WHAT YOU TELL ME. FUCK YOU I WONT DO WHAT YOU TELL ME. FUCK YOU I WONT DO WHAT YOU TELL ME. FUCK YOU I WONT DO WHAT YOU TELL ME...... MOTHER FUCKERS! UGH!... thank you RATM. *bows*
        RigSource = new ObjectField(); 
        RigSource.objectType = typeof(GameObject);
        RigSource.label = "Refrence Rig"; 
        RigSourceRoot.Add(RigSource); //Adds the ObjectField

        //When the rig source is changed it finds all bones and paths before storeing them in a list and displaying them when you need to select a bone
        RigSource.UnregisterValueChangedCallback(evt => {GetBoneNamesAndPaths((GameObject)RigSource.value);});
        RigSource.RegisterValueChangedCallback(evt => {if(BonePaths.Count>0){BonePaths.Clear();}GetBoneNamesAndPaths((GameObject)RigSource.value);});

//--------------------Animation List beginning----------------------------------------------

        AnimationList.makeItem = () =>
        {
        var InsertAnimationClips = new ObjectField(); // creates the field to drag and drop animation into
        InsertAnimationClips.objectType = typeof(AnimationClip); //sets the type of data the field takes
        InsertAnimationClips.label = "Clip to Process:"; // the text beside the entery field
        return InsertAnimationClips; // then it returns the object field ready to be displayed in a list view... Im doing too many code comments arent I? too bad Imma be screwed 6 months later when I dont remember what list views are
        };

        SetTotalText(); // this sets the text of total frames that are about to be processed. technicly this is wont do anything but I guess it will inialize it... maybe? idk why its here but I dont dare touch it

        AnimationList.bindItem = (Field, Index) => // binds (and spawns) the Object field to the list view
        {
            var ObjectField = Field as ObjectField;
          
            ObjectField.value = AnimationClips[Index];
            

            ObjectField.RegisterValueChangedCallback((evt) => {AnimationClips[Index] = evt.newValue as AnimationClip; }); //When the value of the animation field changes create a new Visual List View object as well as a new entree in the AnimationClip list to pair it with
            ObjectField.RegisterValueChangedCallback((evt) => {SetTotalText(); });   //When the value of the animation field changes recalucate the total frame count. (for real this time)
        };

        AnimationList.itemsAdded += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {AnimationList.Rebuild(); SetTotalText();}; //when the value of the list changes add a new animation field and recalculate total frames
        AnimationList.itemsRemoved += (IEnumerable<int> Indices) => EditorApplication.delayCall += () => {AnimationList.Rebuild(); SetTotalText();}; //when the value of the list changes remove a new animation field and recalculate total frames
// ------------------Animation list end-----------------------------------------------------------------------------

// trejectory list beginning -------------------------------------------------------------------------
// look at the animation List stuff because its very simmar to this and I dont need to over over comment more then I already do
        TrejectoryList.makeItem = () =>
        {
            var NewTrejectoryTime = new FloatField();
            //NewTrejectoryTime.objectType = typeof(float); // dont need to define object type since its a float field and will only take a number unlike the animation list where its an Object field and needs to be told what type of object it takes
            NewTrejectoryTime.label = "Future Trjectory in Seconds";
            return NewTrejectoryTime;
        };

        TrejectoryList.bindItem = (Field, Index) =>
        {
            var FloatField = Field as FloatField;
            FloatField.value = FutureTrejectoryTimes[Index];

            FloatField.RegisterValueChangedCallback((evt) => {FutureTrejectoryTimes[Index] = (float)evt.newValue;});
        };

        TrejectoryList.itemsAdded += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {TrejectoryList.Rebuild();}; // the same as animatoin list but doesnt do the recalculate thing for the total frames becuase thats VERY uneeded from somthing that has nothing to do with total frame count
        TrejectoryList.itemsRemoved += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {TrejectoryList.Rebuild();};

// trejectory list end -------------------------------------------------------------------------

//--------------bone and object tracking beginning----------------------------------------------------------------
// yeah this one is a bit diffrent since I kinda got to do it all from scratch since unity is a fucktard and doesnt make it easy to use custom classes in list veiws
BonesList = root.Q<ListView>("BoneList"); // finds the bone list from the UXML... why didnt I just do this at the top with everything else... don't ask me cause I dont know.
        BonesList.itemsSource = Bones; // sets the item source to the Bones list that containes custom class BoneObjectsAndSettings

          BonesList.itemsAdded += (IEnumerable<int> Indices) => { 
            foreach (int index in Indices) // for every item in the List view create a new BoneObjectsAndSettings and add them to the list
            {
                Bones[index] = new BoneObjectsAndSettings();
            }
            EditorApplication.delayCall += () => BonesList.Rebuild(); // rebuilds the UI for the ListView
            };
        BonesList.itemsRemoved += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {BonesList.Rebuild();};

// I clearly lack a syle guide... jesus christ good thing I dont use python or the amount of indentation errors would kill me. well I guess yall wont see that cause its kinda fixed now
BonesList.makeItem = () =>
{
    return new BoneAndTrackableObjectField(); //returns a nice newly created BoneAndTrackableObjectField not to be confused with BoneObjectsAndSettings BoneAndTrackableObjectField is the visual UI version of it
};

BonesList.bindItem = (Element, Index) =>
        {
            IsRecyling = true; //when the inspector trys to rebind the BoneAndTrackableObjectField to a new Bone on the list to save ram this is set to true 
            if(Index < 0 || Index >= Bones.Count) return;
            var VisElement = Element as BoneAndTrackableObjectField;
            if (VisElement == null) return; //keeps everything from exeploding becuase unity is stupid with UI and can go kill itself

//----------Manualy assinging everything from the BoneObjectsAndSettings custom class to the UI----------
            var RootFoldOut = VisElement.Q <Foldout> (name: "root");
            var BoneName = VisElement.Q <TextField> (name: "BoneNameField");
            var BoneSelect = VisElement.Q<DropdownField> (name: "BoneSelect");
            BoneSelect.choices = BonePaths;
            var IncludeRotation = VisElement.Q <Toggle> (name: "RotationToggle");
            var IncludePosition = VisElement.Q <Toggle> (name: "PositionToggle");
            var IncludeVelocity = VisElement.Q <Toggle> (name: "VelocityToggle");

            BoneName.UnregisterValueChangedCallback(OnBoneChanged);
            BoneSelect.UnregisterValueChangedCallback(OnBoneSelected);
            IncludeRotation.UnregisterValueChangedCallback(OnRotationInclutionChanged);
            IncludePosition.UnregisterValueChangedCallback(OnPositionInclutionChanged);
            IncludeVelocity.UnregisterValueChangedCallback(OnVelocityInclutionChanged);
            RootFoldOut.UnregisterValueChangedCallback(OnFold);

            //Bones[Index].BoneName = "Bone Object Name";
            //Debug.Log(BonePaths.Count);
            BoneObjectsAndSettings BoneSettingsData = Bones[Index];
            BoneSelect.value = BoneSettingsData.BoneObject;
            BoneName.value = BoneSettingsData.BoneName;
            RootFoldOut.text = BoneSettingsData.BoneName;
            RootFoldOut.value = BoneSettingsData.FoldOutValue;
            IncludeRotation.value = BoneSettingsData.IncludeRotation;
            IncludePosition.value = BoneSettingsData.IncludePosition;
            IncludeVelocity.value = BoneSettingsData.IncludeVelocity;
            
            BoneSelect.userData = Index;
            BoneName.userData = Index;
            RootFoldOut.userData = Index;
            IncludeRotation.userData = Index;
            IncludePosition.userData = Index;
            IncludeVelocity.userData = Index;

            BoneSelect.RegisterValueChangedCallback(OnBoneSelected);
            RootFoldOut.RegisterValueChangedCallback(OnFold);
            BoneName.RegisterValueChangedCallback(OnBoneChanged);
            IncludeRotation.RegisterValueChangedCallback(OnRotationInclutionChanged);
            IncludePosition.RegisterValueChangedCallback(OnPositionInclutionChanged);
            IncludeVelocity.RegisterValueChangedCallback(OnVelocityInclutionChanged);
            IsRecyling = false;
        };

//--------------bone and object tracking end----------------------------------------------------------------

        Label FrameAmountLabel = root.Q<Label>("FrameAmount"); //creates the Frames To Process text and dysplays it
    
        FrameAmountLabel = new Label("Frames To Process:" + frame);

        BakeButton = root.Q<Button>("BakeButton");
        BakeButton.clicked += BakeButtonClicked; // when the bake button is clicked well...

        void BakeButtonClicked()// Come down here and....
        {
            Debug.Log("Bake Button Clicked");
            Bake(); // then we bake... DUH!
        }
    }

    void Bake()//takes all of the input Animation Clips, Future Trejectory Values, and Bone Settings and compiles them into a dataset ScriptableObject that is saved to the assets folder in unity
    {
        EditorUtility.DisplayProgressBar("Baking DataSet", "Starting up the Tragiclly Hip", 0f);// creates and displays a loading bar

        if(AnimationClips != null | FutureTrejectoryTimes != null)//checks if the dataset should even be created so some fuck nuts doesnt break it (me)
        {
            int TotalFrames = ExetractTotalFrameAmount(); // the total amount of frames we have created by ExetractFrameAmount
            List<Vector3[]> RootPositions = new List<Vector3[]>(); //yes this is a array inside of a list. is it smart hell the fuck no. does it work... I guess so. each list entree is a animation clip then each array entree is a frame

            DataSet = ScriptableObject.CreateInstance(typeof(DataSet));//creates a ScriptableObject
            AssetDatabase.CreateAsset(DataSet, "Assets/MotionMatchingDataSet.asset"); //Names and Saves it

            DataSet DataSetFile = DataSet as DataSet; //a cast
            int CurrentFrame = 0;//global frame position for Trejectory samples

            for (int i = 0; i < AnimationClips.Count; i++)// for each animation clip we..
            {
                float FrameRate = AnimationClips[i].frameRate; //find that animation clips frame rate
                int TemporaryFrameCount = ExetractFrameAmount(i);// extracts the frame amount of this animation clip AND ONLY THIS CLIP
                DataSetFile.AnimationClips.Add(new AnimationClipInfo{Clip = AnimationClips[i], FrameAmount = TemporaryFrameCount}); // adds the animation clip to the animations list on the dataset
                RootPositions.Add(new Vector3[TemporaryFrameCount]);//creates the field add the corect amount of frames to the array for each clip if you dont do this when you get to adding the data you will get a out of range error since what you are looking for doesnt exist yet
                if(AnimationClips[i].hasRootCurves)//if the animation clip contains root motion (its kinda required)
                {
                    EditorUtility.DisplayProgressBar("Baking DataSet", "Getting Animation Curves. Animation " + i + " of " + AnimationClips.Count, i); // updates the loading bar
                    //Debug.Log("Getting Animation Curves. Animation " + i + " of " + AnimationClips.Count + " Clip Frames: " + TemporaryFrameCount);

//---------------Gets the full animation curves for the root postion and rotation so they can later be evaluated-------
                    RootMotionCurveX = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionX);
                    RootMotionCurveY = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionY);
                    RootMotionCurveZ = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionZ);

                    RootRotationCurveQ_X = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationX);
                    RootRotationCurveQ_Y = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationY);
                    RootRotationCurveQ_Z = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationZ);
                    RootRotationCurveQ_W = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationW);
                    //Debug.Log("Clip " + i + " has motion curves.");
                    for (int j = 0; j < TemporaryFrameCount; j++)
                    {
                        EditorUtility.DisplayProgressBar("Baking DataSet", "Feeding the hungery frame array. Frame" + j + " of " + TemporaryFrameCount, j); // updates the loading bar once again 
                        RootPositions[i][j] = new Vector3(RootMotionCurveX.Evaluate((float)j/AnimationClips[i].frameRate), RootMotionCurveY.Evaluate((float)j/AnimationClips[i].frameRate), RootMotionCurveZ.Evaluate((float)j/AnimationClips[i].frameRate)); // evaluates X, Y, and Z animation curves at the current frame (j) and save it to RootPosition aka the list array aka RootPosition[clip][frame]
                        DataSetFile.Frames.Add(new Frame {RootPosition = RootPositions[i][j]});//creates and adds a frame to the dataset containing the root positional data
                        Quaternion RootRotation = new Quaternion(RootRotationCurveQ_X.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_Y.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_Z.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_W.Evaluate((float)j/AnimationClips[i].frameRate)); //does the same but for root rotation and since that is auto set to a Quaternion (I think) I dont need to do any trickery like the bones
                        DataSetFile.Frames[j].CurrentDirection = RootRotation; // saves root rotation to current/newly created frame
                    }


                    for(int k = 0; k < TemporaryFrameCount; k++)//K is local frame pos
                    {
                        for(int l = 0; l < FutureTrejectoryTimes.Count; l++)//for each future trejectory sample
                        {
                            //Debug.Log("Running Clip: " + i + " LocalFrame: " + k + " GlobalFrame: " + CurrentFrame + " TrejectoryNumb: " + l);
                            DataSetFile.Frames[CurrentFrame].FutureTrejectoryPositions.Add(RootPositions[i][Mathf.Clamp((int)Mathf.Round(FutureTrejectoryTimes[l] * FrameRate) + k, 0, TemporaryFrameCount - 1)] - RootPositions[i][k]); //adds future the root postion (trejectory) sample subtracted by current root postion (to put it into local space) 
                        }
                        DataSetFile.Frames[CurrentFrame].AnimationClipNumber = i; //sets the animation clip number in the animation frame so the matcher will know what animation clip to play when it plays that frame
                        CurrentFrame++;
                    }
                }
                else
                {
                    Debug.LogWarning("Clip " + AnimationClips[i] + " does not have motion curves."); //warns you if you dont have an animation with root motion
                }
            
            }

            if(Bones.Count > 0 && RigSource.value != null)//if bones exist and we have a source rig that we can find the bones on / spawn in later when evaluateing bones
            {
                TempBaking = EditorSceneManager.NewPreviewScene(); //BACK STAGE CREW DRAMA!!! KILLANA WOULD KILL ME! in all serosusness this makes a temporary scene is frame to get bone world postions without dealing with unitys shitty animation system
                GameObject Rig = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)RigSource.value, TempBaking);//spawns the source rig into the temporary scene
                Rig.transform.position = Vector3.zero; //make sure its zeroed postionaly
                Rig.transform.rotation = Quaternion.identity;// aswell as rotationaly 
                
                //Debug.Log("Test" + Rig.transform.position);
                List<string> Paths = new List<string>();

                int TemporaryFrameCount; //again does like what it does in the root motion side of things. saves the total frames for the current clip
                int PreviouslyEvaluatedFrames = 0;//basicly self explaintory every frame that is run through the bone system adds 1 to this int

                for(int i = 0; i < Bones.Count; i++)
                {
                    DataSetFile.Bones.Add(new BoneObjectInfo{BoneName = Bones[i].BoneName, BoneID = i}); //adds all the bones you sleected in the inspector to the bone list in the dataset so that the matcher knows what bone you are talking about becuase everything is done with IDs becusae it makes my life a bit easyer sometimes
                }

                foreach (BoneObjectsAndSettings boneObjects in Bones)
                {
                    Paths.Add(boneObjects.BoneObject);//extracts the paths from the Bone Settings field
                }

                for(int k = 0; k < Paths.Count; k++) //for each bone (that we have a path for)
                {
                    PreviouslyEvaluatedFrames = 0;
                    for (int i = 0; i < AnimationClips.Count; i++)// for each clip
                    {
                        TemporaryFrameCount = ExetractFrameAmount(i);
                        Vector3 LastBonePos = Vector3.zero;// zero it so that last animation clip doesnt interfeer with the next clips first frame or whatever
                        for(int j = 0; j < TemporaryFrameCount; j++)//for each frame
                        {
                            AnimationClips[i].SampleAnimation(Rig,j/AnimationClips[i].frameRate); //samples the animation frame by frame
                            Transform BoneTransform = Rig.transform.Find(Paths[k]); //finds the transfrom for the bone we are looking for
                            //Debug.Log(Paths[k]);
                            Vector3 CurrentVelovity  = Vector3.zero;//zeros the current because you must set it to somthing or math/code wise unity freaks out!
                            if(Bones[k].IncludeVelocity == true)//if the user wants to match Velocity
                            {
                                CurrentVelovity = (((BoneTransform.position - RootPositions[i][j])) - LastBonePos) / (1 / AnimationClips[i].frameRate);// calcuates Velocity based on Delta local postion devided by Delta Time
                            }
                            DataSetFile.Frames[PreviouslyEvaluatedFrames].BoneData.Add(new Bone{BoneID = k, BonePositions = BoneTransform.position - RootPositions[i][j], BoneRotations = BoneTransform.localRotation, BoneVelocities = CurrentVelovity}); // adds everything to the dataset. it also calcuates the local postion of the bone (I should have stated sooner when I say local postion I mean local not to the parent bone but intead the root bone)
                            //Debug.Log("Frame: "+ j +" root Pos from curve: " + RootPositions[i][j] +" Root Pos From Root: " + Root.transform.position + " BoneWorldPos: " + BoneTransform.position + " BoneLocalPos manual: " + (BoneTransform.position - RootPositions[i][j]) + " Unity Built in: " + Root.transform.InverseTransformPoint(BoneTransform.position));
                            LastBonePos = BoneTransform.position - RootPositions[i][j]; // sets last bone postion to bone local postion (local to root position)
                            PreviouslyEvaluatedFrames++;
                        }
                    }
                }
                EditorSceneManager.ClosePreviewScene(TempBaking);//deletes the temporary scene so we dont get more memory leaks then we already have
                Debug.Log("Bone Curves Compleated");
            }
            EditorUtility.SetDirty(DataSetFile); //sets the dataset to dirty so that unity will save it properly (when we save it) to disk instead of letting it die in the RAM
            AssetDatabase.SaveAssets();// saves the dataset
        }
        else
        {
            Debug.LogError("wolves are confused. must define Animation Clips and Future Trejectorys");
        }
        EditorUtility.ClearProgressBar();// deletes the progress bar
    }

    int ExetractFrameAmount(int ClipIndex) //extracts the total frames for animation clip ClipIndex in list AnimationClips
    {
        if(AnimationClips[ClipIndex] == null)//so that untiy doesnt crash
        {
            Debug.Log("No Animation Clip Assigned To Index: " + ClipIndex); //bruh you aint got no animation clips
            return 0;
        }else
        {
            float FrameRate = AnimationClips[ClipIndex].frameRate;//gets the clip frame rate
            float ClipLength = AnimationClips[ClipIndex].length;//gets the clip length

            //Debug.Log("Frame Rate: " + FrameRate);
            //Debug.Log("Clip Length: " + ClipLength);
            //Debug.Log("Total Frames: " + (FrameRate * ClipLength));
            return (int)(FrameRate * ClipLength);// frame rate multiplyed by ClipLength will return the total frames becuase math is math
        }
    }

    int ExetractTotalFrameAmount() //extracts the TOTAL frame amount. by using the ExetractFrameAmount and a for loop to find and add up everything into one big total frame count
    {
        int TotalFrames = 0;
        for (int i = 0; i < AnimationClips.Count; i++) //loops through all animation clips calcuating their total frames indivualy then adding them up togthor
        {
            TotalFrames += ExetractFrameAmount(i);
        }
        //Debug.Log("Total Frames To Process: " + TotalFrames);
        return TotalFrames;
    }

    void SetTotalText() //self explaintory
    {
        Label FrameAmountLabel = rootVisualElement.Q<Label>("FrameAmount");
        FrameAmountLabel.text = "Frames To Process: " + ExetractTotalFrameAmount(); //we are feeding funtions to funtions to funtions
    }

    private void OnBoneChanged(ChangeEvent<string> evt) //Update the bone UI 
    {
        var BoneTextField = evt.target as TextField; //stores the bone name text
        var root = BoneTextField.parent as Foldout; //stores the fold out that contains said bone / text field / whatever whatever / BoneObjectsAndSettings
        if(evt.target is TextField field &&  field.userData is int index && index < Bones.Count) // checks if the index exists
        {
            Bones[index].BoneName = evt.newValue; //Updates the Bone data if you happened to change the name
            if(IsRecyling == false)
            {
                root.text = BoneTextField.value; // if we are not recyling the fold out (becuae unity recyles things to save ram) update the text 
            }
        }
    }

    private void GetBoneNamesAndPaths(GameObject BoneSourceObject) //SELF EXPLAINTORY 
    {
        //string SourcePath = AssetDatabase.GetAssetPath(BoneSourceObject);
        //Debug.Log(SourcePath);

        //object jim = AssetDatabase.LoadAssetAtPath(SourcePath, typeof(object));
        SkinnedMeshRenderer skinnedMeshRenderer_m = BoneSourceObject.GetComponentInChildren<SkinnedMeshRenderer>();//it gets the source rig and find the the skinnedMeshRenderer and finds all the bones that are all defined in it
        Transform RootAnimation = BoneSourceObject.transform; // the root of the animation (not nessisarly the ataul root bone)


        foreach (Transform bone in skinnedMeshRenderer_m.bones)//this find every bone in the SkinnedMeshRenderer / Refrence Rig 
        {
            //Debug.Log(bone);
            Transform CurrentBone = bone.parent;
            string path = ""; //Temp bone path
            while(CurrentBone != RootAnimation && CurrentBone != null) //while the current bone doesnt equal the root bone. it starts at the source aka the bone and walks up the bone path to the root
            {

                path = path.Insert(0, "/" + CurrentBone.name);//as it goes up the bone chains to the root each bone gets added to the total path
                if(CurrentBone.parent == RootAnimation || CurrentBone.parent == null)
                {
                    path = path.Remove(0,1);//then removes the first slash so it actally works
                }
                
                CurrentBone = CurrentBone.parent;
            }
            //Debug.Log(path);
            BonePaths.Add(path);//adds the bone path to the bone path list

        }
    }

    private void GetCurvesFromPaths() //from the bone paths found in GetBoneNamesAndPaths it finds (and stores) the bone curves
    {
        Debug.Log("getting Curves From Paths");
        if(BonePaths.Count < 1)
        {
            Debug.LogWarning("No Bone Paths Found");
            return;
        }else
        {
            foreach (BoneObjectsAndSettings boneObjects in Bones) // for each bone that you selected in the inspector
            {
                string paths = boneObjects.BoneObject;//extracts the paths from the Bone Settings field
                //paths = paths.Remove(0, 11);
                //Debug.Log(paths);
                BoneEditorCurvesPos.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.z" }});// finds and store the X,Y,Z Local Postions curves 
                BoneEditorCurvesRotQ.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.z" }, new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.w" }}); // same but for roation stores the X,Y,Z,W Quaternion value. and yes I also try to store the eular values too so that if one doesnt exist another will take over
                BoneEditorCurvesRotE.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.z" }});//owo what dis. just a lil cute fwickin heck 
                //Debug.Log(new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "jimmy" });
            }
        }
    }

    private void OnRotationInclutionChanged(ChangeEvent<bool> evt) //runs when rotation includeion is changed and sets the new value look at the OnBoneChanged it has more comments
    {
        if(evt.target is Toggle field &&  field.userData is int index && index < Bones.Count)//checks if index exists
        {
            Bones[index].IncludeRotation = evt.newValue;//sets new value from UI
        }
    }
//yada yada yada so on so forth its all the same. you get the jist of it (I hope) if not idk why you are looking at me for motion matching advice becuase only me and god know how this code works and well... it seems only god knows now becuase I have forgotton... oops
    private void OnPositionInclutionChanged(ChangeEvent<bool> evt)
    {
        if(evt.target is Toggle field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].IncludePosition = evt.newValue;
        }
    }

    private void OnVelocityInclutionChanged(ChangeEvent<bool> evt)
    {
        if(evt.target is Toggle field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].IncludeVelocity = evt.newValue;
        }
    }

    private void OnFold(ChangeEvent<bool> evt)
    {
        if(evt.target is Foldout field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].FoldOutValue = field.value;
        }
    }

    private void OnBoneSelected(ChangeEvent<string> evt)
    {
        if(evt.target is DropdownField field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].BoneObject = field.value;
        }
    }
}


[System.Serializable]
public class BoneObjectsAndSettings // the custom class for the bone settings as seen in the Bone Selector list in the inspector
{
    public string BoneName = "Bone Object Name";
    public string BoneObject;
    public bool IncludeRotation = true;
    public bool IncludePosition = true;
    public bool IncludeVelocity = true;
    public bool FoldOutValue = false; //needed so I can set the value of the fold out to see if its open or closed and to have it remeber
}

public class BoneAndTrackableObjectField: VisualElement // the visual Element for BoneObjectsAndSettings
{
    public BoneAndTrackableObjectField()
    {
        var root = new Foldout{text = "name"};//creates the fold out/dropdown field
        root.name = "root";

//creates all of the visual elements plus names it and adds lable texts too
        var BoneTextField = new TextField("Bone Object Name"); 
        BoneTextField.label = "Bone Name";
        BoneTextField.name = "BoneNameField";

        var BoneSelect = new DropdownField();
        BoneSelect.name = "BoneSelect";
        BoneSelect.label = "Bone Object to track:";

        var PositionToggle = new Toggle("Track Position?");
        PositionToggle.name = "PositionToggle";

        var RotationToggle = new Toggle("Track Rotation?");
        RotationToggle.name = "RotationToggle";

        var VelocityToggle = new Toggle("Track Velocity?");
        VelocityToggle.name = "VelocityToggle";

//everything gets added to the inspector dropdown
        Add(root);
        root.Add(BoneTextField);
        root.Add(BoneSelect);
        root.Add(PositionToggle);
        root.Add(RotationToggle);
        root.Add(VelocityToggle);
    }
}
