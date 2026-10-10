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

    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        motionMatching = Player.GetComponent<MotionMatching>();
        playerMixedMotionMatching = Player.GetComponent<PlayerMixedMotionMatching>();

        SpeedForwardSlider.value = playerMovement.HumanFormRunSpeedForward;
    }
    public void SpeedForward(float PlayerSpeedForward)
    {
        playerMovement.HumanFormRunSpeedForward = PlayerSpeedForward;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SpeedForwardsText.text = PlayerSpeedForward.ToString();
    }

    public void TimeBetweenSearches(float TimeBetweenSearchesAmount)
    {
        motionMatching.TimeBetweenSerches = TimeBetweenSearchesAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TimeBetweenSearchesText.text = TimeBetweenSearchesAmount.ToString();
    }

    public void TrejectoryWeight(float TrejectoryWeightAmount)
    {
        motionMatching.FutureTrejectoryWeight = TrejectoryWeightAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryWeightText.text = TrejectoryWeightAmount.ToString();
    }

    public void RootRotationWeight(float RootRotationWeightAmount)
    {
        motionMatching.RotationWeight = RootRotationWeightAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        RootRotationWeightText.text = RootRotationWeightAmount.ToString();
    }

    public void FarFrame(int FarFrameDistance)
    {
        motionMatching.FarFrame = FarFrameDistance;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        DistanceRequirementText.text = FarFrameDistance.ToString();
    }

    public void ChangeThreshold(float ChangeThresholdAmount)
    {
        motionMatching.ChangeThreshold = ChangeThresholdAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ChangeThrethholdText.text = ChangeThresholdAmount.ToString();
    }

    public void ClipChangePenalty(float ClipChangePenaltyAmount)
    {
        motionMatching.ClipChangePenalty = ClipChangePenaltyAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ClipChangePenitlyText.text = ClipChangePenaltyAmount.ToString();
    }

    public void TrejectoryHalfLife(float TrejectoryHalfLifeAmount)
    {
        playerMixedMotionMatching.TrejectoryHalfLife = TrejectoryHalfLifeAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryHalfLifeText.text = TrejectoryHalfLifeAmount.ToString();
    }

    public void TrejectoryReponciveness(float TrejectoryReponcivenessAmount)
    {
        playerMixedMotionMatching.Responsiveness = TrejectoryReponcivenessAmount;
        //playerMovement.LastForm = PlayerMovement.Form.Neutral;
        TrejectoryReponcivenessText.text = TrejectoryReponcivenessAmount.ToString();
    }

    public void BonePositionWeight(float BonePositionWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BonePositionWeight = BonePositionWeightAmount;
        }
        BonePositionWeightsText.text = BonePositionWeightAmount.ToString();
    }

    public void BoneVelocityWeight(float BoneVelocityWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BoneVelocityWeight = BoneVelocityWeightAmount;
        }
        BoneVelocityWeightText.text = BoneVelocityWeightAmount.ToString();
    }

    public void BoneRotationWeight(float BoneRotationWeightAmount)
    {
        for(int i = 0; motionMatching.Bones.Count > i; i++)
        {
            motionMatching.Bones[i].BoneRotationWeight = BoneRotationWeightAmount;
        }
        BoneRotationWeightText.text = BoneRotationWeightAmount.ToString();
    }

    
}
