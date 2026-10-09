using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MovementControllPanel : MonoBehaviour
{
    PlayerMovement playerMovement;
    public GameObject Player;
    
    [Header("SpeedForward")]
    public Slider SpeedForwardSlider;
    public TMP_Text SpeedForwardsText;

    [Header("Speed Side")]
    public Slider SpeedSideSlider;
    public TMP_Text SpeedSideText;

    [Header("Forward Agility")]
    public Slider ForwardMoveAgilitySlider;
    public TMP_Text ForwardMoveAgilityText;

    [Header("Side Agility")]
    public Slider SideMoveAgilitySlider;
    public TMP_Text SideMoveAgilityText;

    [Header("Slow Down Forwards")]
    public Slider AgilitySlowForwardsSlider;
    public TMP_Text AgilitySlowForwardsText;

    [Header("Slow Down Side")]
    public Slider AgilitySlowSideSlider;
    public TMP_Text AgilitySlowSideText;

    [Header("Chnage Dir Delay X")]
    public Slider ChangeDirDelayXSlider;
    public TMP_Text ChangeDirDelayXText;

    [Header("Change Dir Delay Z")]
    public Slider ChangeDirDelayZSlider;
    public TMP_Text ChangeDirDelayZText;

    [Header("Sprint speed Forward")]
    public Slider SprintSpeedForwardsSlider;
    public TMP_Text SprintSpeedForwardsText;

    [Header("Sprint Speed Side")]
    public Slider SprintSpeedSideSlider;
    public TMP_Text SprintSpeedSideText;

    [Header("Jump")]
    public Slider JumpHeightSlider;
    public TMP_Text JumpHeightText;

    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        SpeedForwardSlider.value = playerMovement.HumanFormRunSpeedForward;
        SpeedSideSlider.value = playerMovement.HumanFormRunSpeedSide;
        ForwardMoveAgilitySlider.value = playerMovement.AgilityMovementForwardsHuman;
        SideMoveAgilitySlider.value = playerMovement.AgilityMoveSideHuman;
        AgilitySlowForwardsSlider.value = playerMovement.AgilitySlowForwardsHuman;
        AgilitySlowSideSlider.value = playerMovement.AgilitySlowSideHuman;
        ChangeDirDelayXSlider.value = playerMovement.ChangeDirDelayXHuman;
        ChangeDirDelayZSlider.value = playerMovement.ChangeDirDelayZHuman;
        SprintSpeedForwardsSlider.value = playerMovement.HumanFormSprintSpeedForward;
        SprintSpeedSideSlider.value = playerMovement.HumanFormSprintSpeedSide;
        JumpHeightSlider.value = playerMovement.HumanJumpHeight;
    }
    public void SpeedForward(float PlayerSpeedForward)
    {
        playerMovement.HumanFormRunSpeedForward = PlayerSpeedForward;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SpeedForwardsText.text = PlayerSpeedForward.ToString();
    }

    public void SpeedSide(float PlayerSpeedSide)
    {
        playerMovement.HumanFormRunSpeedSide = PlayerSpeedSide;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SpeedSideText.text = PlayerSpeedSide.ToString();
    }


    public void ForwardMoveAgility(float ForwardMoveAgilityValue)
    {
        playerMovement.AgilityMovementForwardsHuman = ForwardMoveAgilityValue;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ForwardMoveAgilityText.text = ForwardMoveAgilityValue.ToString();
    }

    public void SideMoveAgility(float SideMoveAgilityValue)
    {
        playerMovement.AgilityMoveSideHuman = SideMoveAgilityValue;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SideMoveAgilityText.text = SideMoveAgilityValue.ToString();
    }

    public void AgilitySlowDownForwards(float SlowDownForwards)
    {
        playerMovement.AgilitySlowForwardsHuman = SlowDownForwards;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        AgilitySlowForwardsText.text = SlowDownForwards.ToString();
    }

    public void AgilitySlowDownSide(float SlowDownSide)
    {
        playerMovement.AgilitySlowSideHuman = SlowDownSide;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        AgilitySlowSideText.text = SlowDownSide.ToString();
    }

    public void ChangeDirDelayX(float ChangeDirX)
    {
        playerMovement.ChangeDirDelayXHuman = ChangeDirX;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ChangeDirDelayXText.text = ChangeDirX.ToString();
    }
    
        public void ChangeDirDelayZ(float ChangeDirZ)
    {
        playerMovement.ChangeDirDelayZHuman = ChangeDirZ;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        ChangeDirDelayZText.text = ChangeDirZ.ToString();
    }

    public void SprintSpeedForwards(float SprintForwards)
    {
        playerMovement.HumanFormSprintSpeedForward = SprintForwards;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SprintSpeedForwardsText.text = SprintForwards.ToString();
    }

    public void SprintSpeedSide(float SprintSide)
    {
        playerMovement.HumanFormSprintSpeedSide = SprintSide;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        SprintSpeedSideText.text = SprintSide.ToString();
    }

    public void JumpHight(float JumpForce)
    {
        playerMovement.HumanJumpHeight = JumpForce;
        playerMovement.LastForm = PlayerMovement.Form.Neutral;
        JumpHeightText.text = JumpForce.ToString();
    }
}
