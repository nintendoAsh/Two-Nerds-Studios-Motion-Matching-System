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
    public List<AnimationClip> AnimationClips = new List<AnimationClip>();
    public List<float> FutureTrejectoryTimes = new List<float>();
     List<BoneObjectsAndSettings> Bones = new List<BoneObjectsAndSettings>();
     List<string> BonePaths = new List<string>();
     //List<string> BoneNames = new List<string>(); 
     //List<EditorCurveBinding> BoneEditorCurvesPos = new List<EditorCurveBinding>();
     List<List<EditorCurveBinding>> BoneEditorCurvesPos = new List<List<EditorCurveBinding>>(); // first list contains main/root Bones or curves then the second nested list contains the X,Y,Z and the actaul Curve
     List<List<EditorCurveBinding>> BoneEditorCurvesRotQ = new List<List<EditorCurveBinding>>(); // same as Bone Editor pos but for rotation side note WE ARE HECKING IT UP WITH A LIL HACK!!! storeing both eular and Quaternion curves
     List<List<EditorCurveBinding>> BoneEditorCurvesRotE = new List<List<EditorCurveBinding>>(); // same as BoneEditorCurvesRotQ but for eular angles
    private ListView TrejectoryList;
    private ListView AnimationList;
    private ListView BonesList;
    private Button BakeButton;
    public List<Vector3> RootPositions = new List<Vector3>();
    int frame = 0;
    AnimationCurve RootMotionCurveX;
    AnimationCurve RootMotionCurveY;
    AnimationCurve RootMotionCurveZ;
//--------Animation curves for the Quaternion rotation
    AnimationCurve RootRotationCurveQ_X;
    AnimationCurve RootRotationCurveQ_Y;
    AnimationCurve RootRotationCurveQ_Z;
    AnimationCurve RootRotationCurveQ_W;

///-------animation curves for the fuckin eular angles 
    AnimationCurve RootRotationCurveE_X;
    AnimationCurve RootRotationCurveE_Y;
    AnimationCurve RootRotationCurveE_Z;


    EditorCurveBinding RootMotionX = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.x" };
    EditorCurveBinding RootMotionY = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.y" };
    EditorCurveBinding RootMotionZ = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootT.z" };

    EditorCurveBinding RootRotationX = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.x" }; 
    EditorCurveBinding RootRotationY = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.y" };
    EditorCurveBinding RootRotationZ = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.z" };
    EditorCurveBinding RootRotationW = new EditorCurveBinding {path = "", type = typeof(Animator), propertyName = "RootQ.w" }; //Quaternions are weird
    ScriptableObject DataSet;
    private ListView listView;
    bool IsRecyling = false;
    VisualElement RigSourceRoot;
    ObjectField RigSource;
    Scene TempBaking;
    DropdownField RootSelect;

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
        VisualElement Title = new Label("TWO NERDS STUDIOS UNITY MOTION MATCHING EDITOR");
        VisualElement Credits = new Label("BY ASHER RANDALL");
        VisualElement Version = new Label("Version: 2.0 Alpha - UP TO HERE");
        root.Add(Title);
        root.Add(Credits);
        root.Add(Version);

    

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        AnimationList = root.Q<ListView>("AnimationList");
        AnimationList.itemsSource = AnimationClips;

        TrejectoryList = root.Q<ListView>("FutureTrejectorys");
        TrejectoryList.itemsSource = FutureTrejectoryTimes;

        RigSourceRoot = root.Q<VisualElement>("RigSource");
        RigSource = new ObjectField();
        RigSource.objectType = typeof(GameObject);
        RigSource.label = "Refrence Rig";
        RigSourceRoot.Add(RigSource);

        //RootSelect = root.Q<DropdownField>("RootSelect");
        //RootSelect.label = "Rig root bone";
        //RootSelect.choices = new List<string>{"Please Input Rig Object"};
        //RootSelect.SetEnabled(false);
        //RootSelect.Add(RigSource);

        //bone list would go here but you cant use a function before its declared
        RigSource.UnregisterValueChangedCallback(evt => {GetBoneNamesAndPaths((GameObject)RigSource.value);});
        RigSource.RegisterValueChangedCallback(evt => {if(BonePaths.Count>0){BonePaths.Clear();}GetBoneNamesAndPaths(
            (GameObject)RigSource.value);
            });

//--------------------Animation List beginning----------------------------------------------

        AnimationList.makeItem = () =>
        {
        var InsertAnimationClips = new ObjectField();
        InsertAnimationClips.objectType = typeof(AnimationClip);
        InsertAnimationClips.label = "Clip to Process:";
        return InsertAnimationClips;
        };

        SetTotalText();

        AnimationList.bindItem = (Field, Index) =>
        {
            var ObjectField = Field as ObjectField;
          
            ObjectField.value = AnimationClips[Index];
            
            
            Debug.Log(AnimationClips.Count);
            ObjectField.RegisterValueChangedCallback((evt) => {AnimationClips[Index] = evt.newValue as AnimationClip; });
            ObjectField.RegisterValueChangedCallback((evt) => {SetTotalText(); });   
        };

        AnimationList.itemsAdded += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {AnimationList.Rebuild(); SetTotalText();};
        AnimationList.itemsRemoved += (IEnumerable<int> Indices) => EditorApplication.delayCall += () => {AnimationList.Rebuild(); SetTotalText();};
// ------------------Animation list end-----------------------------------------------------------------------------

// trejectory list beginning -------------------------------------------------------------------------

        TrejectoryList.makeItem = () =>
        {
            var NewTrejectoryTime = new FloatField();
            //NewTrejectoryTime.objectType = typeof(float);
            NewTrejectoryTime.label = "Future Trjectory in Seconds";
            return NewTrejectoryTime;
        };

        TrejectoryList.bindItem = (Field, Index) =>
        {
            var FloatField = Field as FloatField;
            FloatField.value = FutureTrejectoryTimes[Index];

            FloatField.RegisterValueChangedCallback((evt) => {FutureTrejectoryTimes[Index] = (float)evt.newValue;});
        };

        TrejectoryList.itemsAdded += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {TrejectoryList.Rebuild();};
        TrejectoryList.itemsRemoved += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {TrejectoryList.Rebuild();};

// trejectory list end -------------------------------------------------------------------------

//--------------bone and object tracking beginning----------------------------------------------------------------
BonesList = root.Q<ListView>("BoneList");
        BonesList.itemsSource = Bones;

          BonesList.itemsAdded += (IEnumerable<int> Indices) => { 
            foreach (int index in Indices)
              {
                  Bones[index] = new BoneObjectsAndSettings();
              }
              EditorApplication.delayCall += () => BonesList.Rebuild();
              };
        BonesList.itemsRemoved += (IEnumerable<int> Indices) =>  EditorApplication.delayCall += () => {BonesList.Rebuild();};

BonesList.makeItem = () =>
{
    return new BoneAndTrackableObjectField();
};

BonesList.bindItem = (Element, Index) =>
        {
            IsRecyling = true;
            if(Index < 0 || Index >= Bones.Count) return;
            var VisElement = Element as BoneAndTrackableObjectField;
            if (VisElement == null) return; //keeps everything from exeploding becuase unity is stupid with UI and can go kill itself

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



        Label FrameAmountLabel = root.Q<Label>("FrameAmount");
    
        FrameAmountLabel = new Label("Frames To Process:" + frame);

        BakeButton = root.Q<Button>("BakeButton");
        BakeButton.clicked += BakeButtonClicked;

        void BakeButtonClicked()
        {
            Debug.Log("Bake Button Clicked");
            Bake();
        }
    }

    void Bake()
    {
        EditorUtility.DisplayProgressBar("Baking DataSet", "Starting up the Tragiclly Hip", 0f);

        if(AnimationClips != null | FutureTrejectoryTimes != null)
        {
            int TotalFrames = ExetractTotalFrameAmount();
            List<Vector3[]> RootPositions = new List<Vector3[]>();

            DataSet = ScriptableObject.CreateInstance(typeof(DataSet));
            AssetDatabase.CreateAsset(DataSet, "Assets/MotionMatchingDataSet.asset");

            DataSet DataSetFile = DataSet as DataSet;
            int CurrentFrame = 0;//global frame position for Trejectory samples

            for (int i = 0; i < AnimationClips.Count; i++)
            {
                float FrameRate = AnimationClips[i].frameRate;
                int TemporaryFrameCount = ExetractFrameAmount(i);
                DataSetFile.AnimationClips.Add(new AnimationClipInfo{Clip = AnimationClips[i], FrameAmount = TemporaryFrameCount});
                RootPositions.Add(new Vector3[TemporaryFrameCount]);//creats the feild add the corect amount of frames to the array for each clip if you dont do this when you get to adding the data you will get a out of range error since what you are looking for doesnt exist yet
                if(AnimationClips[i].hasRootCurves)
                {
                    EditorUtility.DisplayProgressBar("Baking DataSet", "Getting Animation Curves. Animation " + i + " of " + AnimationClips.Count, i);
                    Debug.Log("Getting Animation Curves. Animation " + i + " of " + AnimationClips.Count + " Clip Frames: " + TemporaryFrameCount);
                    RootMotionCurveX = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionX);
                    RootMotionCurveY = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionY);
                    RootMotionCurveZ = AnimationUtility.GetEditorCurve(AnimationClips[i], RootMotionZ);

                    RootRotationCurveQ_X = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationX);
                    RootRotationCurveQ_Y = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationY);
                    RootRotationCurveQ_Z = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationZ);
                    RootRotationCurveQ_W = AnimationUtility.GetEditorCurve(AnimationClips[i], RootRotationW);
                    Debug.Log("Clip " + i + " has motion curves.");
                    for (int j = 0; j < TemporaryFrameCount; j++)
                    {
                        EditorUtility.DisplayProgressBar("Baking DataSet", "Feeding the hungery frame array. Frame" + j + " of " + TemporaryFrameCount, j);
                        RootPositions[i][j] = new Vector3(RootMotionCurveX.Evaluate((float)j/AnimationClips[i].frameRate), RootMotionCurveY.Evaluate((float)j/AnimationClips[i].frameRate), RootMotionCurveZ.Evaluate((float)j/AnimationClips[i].frameRate));
                        DataSetFile.Frames.Add(new Frame {RootPosition = RootPositions[i][j]});
                        Quaternion RootRotation = new Quaternion(RootRotationCurveQ_X.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_Y.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_Z.Evaluate((float)j/AnimationClips[i].frameRate), RootRotationCurveQ_W.Evaluate((float)j/AnimationClips[i].frameRate));
                        DataSetFile.Frames[j].CurrentDirection = RootRotation;
                    }


                    for(int k = 0; k < TemporaryFrameCount; k++)//K is local frame pos
                    {
                        for(int l = 0; l < FutureTrejectoryTimes.Count; l++)
                        {
                            Debug.Log("Running Clip: " + i + " LocalFrame: " + k + " GlobalFrame: " + CurrentFrame + " TrejectoryNumb: " + l);
                            DataSetFile.Frames[CurrentFrame].FutureTrejectoryPositions.Add(RootPositions[i][Mathf.Clamp((int)Mathf.Round(FutureTrejectoryTimes[l] * FrameRate) + k, 0, TemporaryFrameCount - 1)] - RootPositions[i][k]); //adds future root postion (trejectory) sample subtracted by current root postion (to put it into local space) 
                        }
                        DataSetFile.Frames[CurrentFrame].AnimationClipNumber = i;
                        CurrentFrame++;
                    }
                }
                else
                {
                    Debug.LogWarning("Clip " + AnimationClips[i] + " does not have motion curves.");
                }
            
            }

            if(Bones.Count > 0 && RigSource.value != null)
            {
                TempBaking = EditorSceneManager.NewPreviewScene(); //BACK STAGE CREW DRAMA!!! KILLANA WOULD KILL ME! in all serosusness this makes a temporary scene is frame to get bone world postions without dealing with unitys shitty animation system
                GameObject Rig = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)RigSource.value, TempBaking);
                Rig.transform.position = Vector3.zero;
                Rig.transform.rotation = Quaternion.identity;
                
                Debug.Log("Test" + Rig.transform.position);
                List<string> Paths = new List<string>();

                int TemporaryFrameCount;
                int PreviouslyEvaluatedFrames = 0;

                for(int i = 0; i < Bones.Count; i++)
                {
                    DataSetFile.Bones.Add(new BoneObjectInfo{BoneName = Bones[i].BoneName, BoneID = i});
                }

                foreach (BoneObjectsAndSettings boneObjects in Bones)
                {
                    Paths.Add(boneObjects.BoneObject);//extracts the paths from the Bone Settings field
                }

                for(int k = 0; k < Paths.Count; k++)
                {
                    PreviouslyEvaluatedFrames = 0;
                    for (int i = 0; i < AnimationClips.Count; i++)
                    {
                        TemporaryFrameCount = ExetractFrameAmount(i);
                        Vector3 LastBonePos = Vector3.zero;
                        for(int j = 0; j < TemporaryFrameCount; j++)
                        {
                            AnimationClips[i].SampleAnimation(Rig,j/AnimationClips[i].frameRate);
                            Transform BoneTransform = Rig.transform.Find(Paths[k]);
                            //Debug.Log(Paths[k]);
                            Vector3 CurrentVelovity  = Vector3.zero;
                            if(Bones[k].IncludeVelocity == true)
                            {
                                CurrentVelovity = (((BoneTransform.position - RootPositions[i][j])) - LastBonePos) / (1 / AnimationClips[i].frameRate);
                            }
                            DataSetFile.Frames[PreviouslyEvaluatedFrames].BoneData.Add(new Bone{BoneID = k, BonePositions = BoneTransform.position - RootPositions[i][j], BoneRotations = BoneTransform.localRotation, BoneVelocities = CurrentVelovity});
                            //Debug.Log("Frame: "+ j +" root Pos from curve: " + RootPositions[i][j] +" Root Pos From Root: " + Root.transform.position + " BoneWorldPos: " + BoneTransform.position + " BoneLocalPos manual: " + (BoneTransform.position - RootPositions[i][j]) + " Unity Built in: " + Root.transform.InverseTransformPoint(BoneTransform.position));
                            LastBonePos = BoneTransform.position - RootPositions[i][j];
                            PreviouslyEvaluatedFrames++;
                        }
                    }
                }
                EditorSceneManager.ClosePreviewScene(TempBaking);

                /*
                Debug.Log("Running Bone Matching");
                GetCurvesFromPaths();
                int TemporaryFrameCount;

                List<List<AnimationCurve>> BonePosAnimationCurves = new List<List<AnimationCurve>>(); // just like the bone editor curves first list for bone second for individal X,Y,Z
                List<List<AnimationCurve>> BoneRotAnimationCurves_Q = new List<List<AnimationCurve>>();// HEY GUESS WHAT!!! MY LIL HACK AFFECTS EVERYTHING ROTATION WISE DONT YOU LOVE IT!
                List<List<AnimationCurve>> BoneRotAnimationCurves_E = new List<List<AnimationCurve>>();
                List<Vector3[]> BonePositions = new List<Vector3[]>(); //for those of you wondering. this is for storeing the evaluated curves before wipeing the list and then re-evlauating new curves... anyways basicly it stores it so later we can put it in the dataset
                List<Quaternion[]> BoneRotations = new List<Quaternion[]>();


                for (int i = 0; i < AnimationClips.Count; i++)
                {
                    TemporaryFrameCount = ExetractFrameAmount(i);
                    BonePositions.Add(new Vector3[TemporaryFrameCount]);//adds a new bone Position clip and then adds the corect ammount of frames to that clip
                    BoneRotations.Add(new Quaternion[TemporaryFrameCount]);
                    int PreviouslyEvaluatedFrames = 0; // wat dis... just a lil hack to get around my sutpidity. TODO: rewrite entire Bake fuction so its NOT A FUCKIN MESS... kill me now. tbh though it would be better if it was intigrated and not like here is root motion and here is bone stuff becuase I prob woulent need my lil bitty cute hack
                    for(int j = 0; j < BoneEditorCurvesPos.Count; j++)
                    {
                        BonePosAnimationCurves.Add(new List<AnimationCurve>{AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesPos[j][0]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesPos[j][1]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesPos[j][2])});
                        BoneRotAnimationCurves_Q.Add(new List<AnimationCurve>{AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotQ[j][0]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotQ[j][1]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotQ[j][2]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotQ[j][3])});
                        BoneRotAnimationCurves_E.Add(new List<AnimationCurve>{AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotE[j][0]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotE[j][1]),AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesRotE[j][2])});
                        //Debug.Log(AnimationUtility.GetEditorCurve(AnimationClips[i], BoneEditorCurvesPos[j][0]));
                        if(BoneRotAnimationCurves_Q[j][0] == null)//if not a Quaternion convert to Quaternion and add
                        {
                            for(int k = 0; k < TemporaryFrameCount; k++)//runs the rotation checks first
                            {
                                BoneRotations[i][k] = Quaternion.Euler(new Vector3(BoneRotAnimationCurves_E[j][0].Evaluate(k/AnimationClips[i].frameRate),BoneRotAnimationCurves_E[j][1].Evaluate(k/AnimationClips[i].frameRate),BoneRotAnimationCurves_E[j][2].Evaluate(k/AnimationClips[i].frameRate)));
                            }
                            Debug.Log("Is a eluar angle. converting");
                        }
                        else 
                        {
                            for(int k = 0; k < TemporaryFrameCount; k++)
                            {
                                BoneRotations[i][k] = new Quaternion(BoneRotAnimationCurves_Q[j][0].Evaluate(k/AnimationClips[i].frameRate),BoneRotAnimationCurves_Q[j][1].Evaluate(k/AnimationClips[i].frameRate),BoneRotAnimationCurves_Q[j][2].Evaluate(k/AnimationClips[i].frameRate),BoneRotAnimationCurves_Q[j][3].Evaluate(k/AnimationClips[i].frameRate));
                            }
                            Debug.Log("is a fuck-ass Quaternion. WHY DOES YOUR ANIMATION NATIVLY HAVE THEM? NEEEEEEEEEEEEEEEEERRRRRRRRRDDDDDDDDDDDD!");
                        }

                        string[] ParentCurves = Bones[j].BoneObject.Split("/");//splits every parent in the path to a string in the array
                        List<List<AnimationCurve>> ParentAnimationCurvesPos = new List<List<AnimationCurve>>();
                        List<List<AnimationCurve>> ParentAnimationCurvesRot = new List<List<AnimationCurve>>();
                        //Debug.Log(ParentAnimationCurvesRot.Count);
                        int index = 0;
                        string TempPath = "";
                        for(int m = 0; m < ParentCurves.Length; m++)
                        {
                             if(TempPath == "")
                            {
                                TempPath += ParentCurves[m];
                            }
                            ParentAnimationCurvesPos.Add(new List<AnimationCurve>{AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalPosition.x" })});
                            ParentAnimationCurvesPos[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalPosition.y" }));
                            ParentAnimationCurvesPos[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalPosition.z" }));
                            //Debug.Log(ParentAnimationCurvesPos[index][1]);

                            ParentAnimationCurvesRot.Add(new List<AnimationCurve>{AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalPosition.x" })}); // just add it as a placeholder
                            if(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalRotation.x" }) == null)
                            {
                                //Debug.Log("Index: " + index + "ParentAnimationCurvesRot: " + ParentAnimationCurvesRot.Count);
                                ParentAnimationCurvesRot[index][0] = AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "localEulerAnglesRaw.x" });//replaces the placeholder
                                ParentAnimationCurvesRot[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "localEulerAnglesRaw.y" }));
                                ParentAnimationCurvesRot[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "localEulerAnglesRaw.z" }));
                            }
                            else
                            {
                                ParentAnimationCurvesRot[index][0] = AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalRotation.x" });//replaces the placeholder
                                ParentAnimationCurvesRot[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalRotation.y" }));
                                ParentAnimationCurvesRot[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalRotation.z" }));
                                ParentAnimationCurvesRot[index].Add(AnimationUtility.GetEditorCurve(AnimationClips[i], new EditorCurveBinding {path = TempPath, type = typeof(Transform), propertyName = "m_LocalRotation.w" }));
                            }
                            index++;
                            if(m+1 != ParentCurves.Length && m !=0)
                            {
                                TempPath += "/" + ParentCurves[m];
                            }
                            //Debug.Log("TempPath: "+ TempPath + "Pos: " + ParentAnimationCurvesPos[index-1][0]);
                        }


                        for(int k = 0; k < TemporaryFrameCount; k++)
                        {
                            BonePositions[i][k] = new Vector3(BonePosAnimationCurves[j][0].Evaluate((float)k/AnimationClips[i].frameRate),BonePosAnimationCurves[j][1].Evaluate((float)k/AnimationClips[i].frameRate),BonePosAnimationCurves[j][2].Evaluate((float)k/AnimationClips[i].frameRate));
                            Debug.Log(BonePosAnimationCurves[j][0].keys[0].value);
                            Vector3 Positon = BonePositions[i][k];
                            Quaternion Rotation = BoneRotations[i][k];
                            Vector3 LocalPositon = new Vector3(0,0,0); //should be named world pos oops
                            Quaternion LocalRotation = Quaternion.identity;

                            AnimationClips[i].SampleAnimation((GameObject)RigSource.value,(float)k/AnimationClips[i].frameRate);
                            for(int l = ParentCurves.Length-1; l >= 0; l--)//runs in reverse
                            {
                                //Debug.Log(l);

                                Vector3 Position_Temp = new Vector3(0,0,0);
                                Quaternion Rotation_Temp = Quaternion.identity;
                                //Debug.Log(TempPath);
                                //Debug.Log(ParentAnimationCurvesRot[l].Count);
                                //Quaternion Rotation = new Quaternion(0,0,0,0);

                                if(ParentAnimationCurvesRot[l].Count == 3)//checks if its a eular or Quaternion
                                {
                                    Rotation_Temp = Quaternion.Euler(new Vector3(ParentAnimationCurvesRot[l][0].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[l][1].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[l][2].Evaluate(k/AnimationClips[i].frameRate)));
                                    //Debug.Log("TEMP!!! Eular");
                                    //Rotation = Quaternion.Euler(new Vector3(ParentAnimationCurvesRot[ParentCurves.Length-1][0].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[ParentCurves.Length-1][1].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[ParentCurves.Length-1][2].Evaluate(k/AnimationClips[i].frameRate)));
                                }else if(ParentAnimationCurvesRot[l].Count == 4)
                                {
                                    Rotation_Temp = new Quaternion(ParentAnimationCurvesRot[l][0].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[l][1].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[l][2].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[l][3].Evaluate(k/AnimationClips[i].frameRate));
                                    //Debug.Log("TEMP!!!! Quaternion");
                                    //Rotation = new Quaternion(ParentAnimationCurvesRot[ParentCurves.Length-1][0].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[ParentCurves.Length-1][1].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[ParentCurves.Length-1][2].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesRot[ParentCurves.Length-1][3].Evaluate(k/AnimationClips[i].frameRate));
                                }

                                //Debug.Log(ParentAnimationCurvesPos[l][0].Evaluate(k/AnimationClips[i].frameRate)); //is returing null
                                Position_Temp = new Vector3(ParentAnimationCurvesPos[l][0].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesPos[l][1].Evaluate(k/AnimationClips[i].frameRate),ParentAnimationCurvesPos[l][2].Evaluate(k/AnimationClips[i].frameRate));
                                //Debug.Log("Pos Temp " + Position_Temp + " Frame: " + k/AnimationClips[i].frameRate + " Frame Rate: " + AnimationClips[i].frameRate);

                         // the current Rotation
                                Vector3 RotationalOffset;//since rotaton can change postion we need to account for it

                                //RotationalOffset = Rotation_Temp * Position_Temp;
                                Positon += Rotation* Position_Temp;
                                Rotation = Rotation * Quaternion.Euler(Position_Temp);
                                //Debug.Log(" Pos: " + Positon + " Rot: "+ Rotation + " Pos_Temp: " + Position_Temp);

                                if(l == 0)
                                {
                                    LocalPositon = Positon;
                                    LocalRotation = Rotation;
                                }
                            }

                            DataSetFile.Frames[PreviouslyEvaluatedFrames].BoneData.Add(new Bone{BoneID = j, BonePositions = LocalPositon});
                            PreviouslyEvaluatedFrames++;


                        }
                        ParentAnimationCurvesRot.Clear();
                    }

                    BonePosAnimationCurves.Clear(); // clears the List instead of making a new List<AnimationCurve>();
                    BoneRotAnimationCurves_Q.Clear();
                }
                */
                Debug.Log("Bone Curves Compleated");
            }
            EditorUtility.SetDirty(DataSetFile);
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.LogError("wolves are confused. must define Animation Clips and Future Trejectorys");
        }
        EditorUtility.ClearProgressBar();
    }

    int ExetractFrameAmount(int ClipIndex)
    {
        if(AnimationClips[ClipIndex] == null)
        {
            Debug.Log("No Animation Clip Assigned To Index: " + ClipIndex);
            return 0;
        }else
        {
            float FrameRate = AnimationClips[ClipIndex].frameRate;
            float ClipLength = AnimationClips[ClipIndex].length;

            //Debug.Log("Frame Rate: " + FrameRate);
            //Debug.Log("Clip Length: " + ClipLength);
            //Debug.Log("Total Frames: " + (FrameRate * ClipLength));
            return (int)(FrameRate * ClipLength);
        }
    }

    int ExetractTotalFrameAmount()
    {
        int TotalFrames = 0;
        for (int i = 0; i < AnimationClips.Count; i++)
        {
            TotalFrames += ExetractFrameAmount(i);
        }
        Debug.Log("Total Frames To Process: " + TotalFrames);
        return TotalFrames;
    }

    void SetTotalText()
    {
        Label FrameAmountLabel = rootVisualElement.Q<Label>("FrameAmount");
        FrameAmountLabel.text = "Frames To Process: " + ExetractTotalFrameAmount();
    }

    private void OnBoneChanged(ChangeEvent<string> evt)
    {
        var BoneTextField = evt.target as TextField;
        var root = BoneTextField.parent as Foldout;
        if(evt.target is TextField field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].BoneName = evt.newValue;
            if(IsRecyling == false)
            {
                root.text = BoneTextField.value;
            }
        }
    }

    private void GetBoneNamesAndPaths(GameObject BoneSourceObject)
    {
         RootSelect.choices = BonePaths;
         RootSelect.SetEnabled(true);
        //string SourcePath = AssetDatabase.GetAssetPath(BoneSourceObject);
        //Debug.Log(SourcePath);

        //object jim = AssetDatabase.LoadAssetAtPath(SourcePath, typeof(object));
        SkinnedMeshRenderer skinnedMeshRenderer_m = BoneSourceObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform RootAnimation = BoneSourceObject.transform; // the root of the animation (not nessisarly the ataul root bone)


        foreach (Transform bone in skinnedMeshRenderer_m.bones)//this find every bone in the SkinnedMeshRenderer / Refrence Rig 
        {
            //Debug.Log(bone);
            Transform CurrentBone = bone.parent;
            string path = "";
            while(CurrentBone != RootAnimation && CurrentBone != null)
            {

                path = path.Insert(0, "/" + CurrentBone.name);
                if(CurrentBone.parent == RootAnimation || CurrentBone.parent == null)
                {
                    path = path.Remove(0,1);
                }
                
                CurrentBone = CurrentBone.parent;
            }
            //Debug.Log(path);
            BonePaths.Add(path);

        }
    }

    private void GetCurvesFromPaths()
    {
        Debug.Log("getting Curves From Paths");
        if(BonePaths.Count < 1)
        {
            Debug.LogWarning("No Bone Paths Found");
            return;
        }else
        {
            foreach (BoneObjectsAndSettings boneObjects in Bones)
            {
                string paths = boneObjects.BoneObject;//extracts the paths from the Bone Settings field
                //paths = paths.Remove(0, 11);
                Debug.Log(paths);
                BoneEditorCurvesPos.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalPosition.z" }});
                BoneEditorCurvesRotQ.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.z" }, new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "m_LocalRotation.w" }});
                BoneEditorCurvesRotE.Add(new List<EditorCurveBinding>{new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.x" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.y" },new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "localEulerAnglesRaw.z" }});//owo what dis. just a lil cute fwickin heck 
                //Debug.Log(new EditorCurveBinding {path = paths, type = typeof(Transform), propertyName = "jimmy" });
            }
        }
    }

    private void OnRotationInclutionChanged(ChangeEvent<bool> evt)
    {
        if(evt.target is Toggle field &&  field.userData is int index && index < Bones.Count)
        {
            Bones[index].IncludeRotation = evt.newValue;
        }
    }

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
public class BoneObjectsAndSettings
{
    public string BoneName = "Bone Object Name";
    public string BoneObject;
    public bool IncludeRotation = true;
    public bool IncludePosition = true;
    public bool IncludeVelocity = true;
    public bool FoldOutValue = false; //needed so I can set the value of the fold out to see if its open or closed and to have it remeber
}

public class BoneAndTrackableObjectField: VisualElement
{
    public BoneAndTrackableObjectField()
    {
        var root = new Foldout{text = "name"};
        root.name = "root";

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

        Add(root);
        root.Add(BoneTextField);
        root.Add(BoneSelect);
        root.Add(PositionToggle);
        root.Add(RotationToggle);
        root.Add(VelocityToggle);

    }
}
