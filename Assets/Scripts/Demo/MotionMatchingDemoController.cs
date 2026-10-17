using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MotionMatchingDemoController : MonoBehaviour
{
    PlayerMovement playerMovement;
    MotionMatching motionMatching;
    PlayerMixedMotionMatching playerMixedMotionMatching;
    public GameObject Player;
    
    [Header("SpeedForward")]
    public Slider SpeedForwardSlider;
    public TMP_Text SpeedForwardsText;

    [Header("TimeBetweenSearches")]
    public Slider TimeBetweenSearchesSlider;
    public TMP_Text TimeBetweenSearchesText;

    [Header("TrejectoryWeight")]
    public Slider TrejectoryWeightSlider;
    public TMP_Text TrejectoryWeightText;

    [Header("RootRotationWeight")]
    public Slider RootRotationWeightSlider;
    public TMP_Text RootRotationWeightText;

    [Header("DistanceRequirement")]
    public Slider DistanceRequirementSlider;
    public TMP_Text DistanceRequirementText;

    [Header("ChangeThrethhold")]
    public Slider ChangeThrethholdSlider;
    public TMP_Text ChangeThrethholdText;

    [Header("ClipChangePenitly")]
    public Slider ClipChangePenitlySlider;
    public TMP_Text ClipChangePenitlyText;

    [Header("TrejectoryHalfLife")]
    public Slider TrejectoryHalfLifeSlider;
    public TMP_Text TrejectoryHalfLifeText;

    [Header("TrejectoryReponciveness")]
    public Slider TrejectoryReponcivenessSlider;
    public TMP_Text TrejectoryReponcivenessText;

    [Header("BonePositionWeight")]
    public Slider BonePositionWeightSlider;
    public TMP_Text BonePositionWeightsText;

    [Header("BoneRotationWeight")]
    public Slider BoneRotationWeightSlider;
    public TMP_Text BoneRotationWeightText;

    [Header("BoneVelocityWeight")]
    public Slider BoneVelocityWeightSlider;
    public TMP_Text BoneVelocityWeightText;

    public GameObject Setting;
    public GameObject MainMenu;
    public GameObject CameraFront;
    public GameObject CameraSide;
    public GameObject CameraAngled;
    public GameObject ThirdPersonCamera;


    public GameObject TrejectoryDataParent;
    public GameObject BoneRealParent;
    public GameObject BoneDataParent;
    public GameObject TrejectoryRealParent;



    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        motionMatching = Player.GetComponent<MotionMatching>();
        playerMixedMotionMatching = Player.GetComponent<PlayerMixedMotionMatching>();

        SpeedForwardSlider.value = playerMovement.MoveSpeedFowardDebug;
        SpeedForwardsText.text = playerMovement.MoveSpeedFowardDebug.ToString("0.###");

        TimeBetweenSearchesSlider.value = motionMatching.TimeBetweenSerches;
        TimeBetweenSearchesText.text = motionMatching.TimeBetweenSerches.ToString("0.###");

        TrejectoryWeightSlider.value = motionMatching.FutureTrejectoryWeight;
        TrejectoryWeightText.text = motionMatching.FutureTrejectoryWeight.ToString("0.###");    

        RootRotationWeightSlider.value = motionMatching.RotationWeight;
        RootRotationWeightText.text = motionMatching.RotationWeight.ToString("0.###");    

        DistanceRequirementSlider.value = motionMatching.FarFrame;
        DistanceRequirementText.text = motionMatching.FarFrame.ToString("0.###");

        ChangeThrethholdSlider.value = motionMatching.ChangeThreshold;
        ChangeThrethholdText.text = motionMatching.ChangeThreshold.ToString("0.###");

        ClipChangePenitlySlider.value = motionMatching.ClipChangePenalty;
        ClipChangePenitlyText.text = motionMatching.ClipChangePenalty.ToString("0.###");    

        TrejectoryHalfLifeSlider.value = playerMixedMotionMatching.TrejectoryHalfLife;
        TrejectoryHalfLifeText.text = playerMixedMotionMatching.TrejectoryHalfLife.ToString("0.###");    

        TrejectoryReponcivenessSlider.value = playerMixedMotionMatching.Responsiveness;
        TrejectoryReponcivenessText.text = playerMixedMotionMatching.Responsiveness.ToString("0.###");

        BonePositionWeightSlider.value = motionMatching.Bones[0].BonePositionWeight;
        BonePositionWeightsText.text = motionMatching.Bones[0].BonePositionWeight.ToString("0.###");

        BoneVelocityWeightSlider.value = motionMatching.Bones[0].BoneVelocityWeight;
        BoneVelocityWeightText.text = motionMatching.Bones[0].BoneVelocityWeight.ToString("0.###");    

        BoneRotationWeightSlider.value = motionMatching.Bones[0].BoneRotationWeight;
        BoneRotationWeightText.text = motionMatching.Bones[0].BoneRotationWeight.ToString("0.###");    
    }
    public void SpeedForward(float PlayerSpeedForward)
    {
        playerMovement.MoveSpeedFowardDebug = PlayerSpeedForward;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SpeedForwardsText.text = PlayerSpeedForward.ToString("0.###");
    }

    public void TimeBetweenSearches(float TimeBetweenSearchesAmount)
    {
        motionMatching.TimeBetweenSerches = TimeBetweenSearchesAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TimeBetweenSearchesText.text = TimeBetweenSearchesAmount.ToString("0.###");
    }

    public void TrejectoryWeight(float TrejectoryWeightAmount)
    {
        motionMatching.FutureTrejectoryWeight = TrejectoryWeightAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryWeightText.text = TrejectoryWeightAmount.ToString("0.###");
    }

    public void RootRotationWeight(float RootRotationWeightAmount)
    {
        motionMatching.RotationWeight = RootRotationWeightAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        RootRotationWeightText.text = RootRotationWeightAmount.ToString("0.###");
    }

    public void FarFrame(float FarFrameDistance)
    {
        motionMatching.FarFrame = (int)FarFrameDistance;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        DistanceRequirementText.text = FarFrameDistance.ToString("0.###");
    }

    public void ChangeThreshold(float ChangeThresholdAmount)
    {
        motionMatching.ChangeThreshold = ChangeThresholdAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ChangeThrethholdText.text = ChangeThresholdAmount.ToString("0.###");
    }

    public void ClipChangePenalty(float ClipChangePenaltyAmount)
    {
        motionMatching.ClipChangePenalty = ClipChangePenaltyAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ClipChangePenitlyText.text = ClipChangePenaltyAmount.ToString("0.###");
    }

    public void TrejectoryHalfLife(float TrejectoryHalfLifeAmount)
    {
        playerMixedMotionMatching.TrejectoryHalfLife = TrejectoryHalfLifeAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryHalfLifeText.text = TrejectoryHalfLifeAmount.ToString("0.###");
    }

    public void TrejectoryReponciveness(float TrejectoryReponcivenessAmount)
    {
        playerMixedMotionMatching.Responsiveness = TrejectoryReponcivenessAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryReponcivenessText.text = TrejectoryReponcivenessAmount.ToString("0.###");
    }

    public void BonePositionWeight(float BonePositionWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BonePositionWeight = BonePositionWeightAmount;
        }
        BonePositionWeightsText.text = BonePositionWeightAmount.ToString("0.###");
    }

    public void BoneVelocityWeight(float BoneVelocityWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BoneVelocityWeight = BoneVelocityWeightAmount;
        }
        BoneVelocityWeightText.text = BoneVelocityWeightAmount.ToString("0.###");
    }

    public void BoneRotationWeight(float BoneRotationWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BoneRotationWeight = BoneRotationWeightAmount;
        }
        BoneRotationWeightText.text = BoneRotationWeightAmount.ToString("0.###");
    }

    public void UseCameraFront()
    {
        CameraFront.SetActive(true);
        CameraAngled.SetActive(false);
        ThirdPersonCamera.SetActive(false);
        CameraSide.SetActive(false);
    }

    public void UseCameraAngled()
    {
        CameraFront.SetActive(false);
        CameraAngled.SetActive(true);
        ThirdPersonCamera.SetActive(false);
        CameraSide.SetActive(false);
    }

    public void UseThirdPersonCamera()
    {
        CameraFront.SetActive(false);
        CameraAngled.SetActive(false);
        ThirdPersonCamera.SetActive(true);
        CameraSide.SetActive(false);
    }

    public void UseCameraSide()
    {
        CameraFront.SetActive(false);
        CameraAngled.SetActive(false);
        ThirdPersonCamera.SetActive(false);
        CameraSide.SetActive(true);
    }

    public void ShowSettingMenu()
    {
        Setting.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void HideSettingsMenu()
    {
        Setting.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void ShowTrejectoryReal(bool value)
    {
        TrejectoryRealParent.SetActive(value);
    }

    public void ShowTrejectoryData(bool value)
    {
        TrejectoryDataParent.SetActive(value);
    }

    public void ShowBoneReal(bool value)
    {
        BoneRealParent.SetActive(value);
    }

    public void ShowBoneData(bool value)
    {
        BoneDataParent.SetActive(value);
    }

}
