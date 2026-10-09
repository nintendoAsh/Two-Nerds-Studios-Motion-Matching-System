using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class WerewolfTransformation : MonoBehaviour
{
    public GameObject HumanForm;
    public GameObject WolfForm;
    public GameObject HumanFormMesh;
    public GameObject WolfFormMesh;
    Animator WolfAnim;
    Animator HumanAnim; 
    PlayerMovement PlayerMoveScript;
    HealthAndStamina HealthAndStamina;
    bool IsTransforming = false;
    public float EnergyRequiredToTransformWolf = 10;
    public float EnergyRequiredToTransformHuman = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HumanAnim = HumanForm.GetComponent<Animator>();
        WolfAnim = WolfForm.GetComponent<Animator>();
        PlayerMoveScript = GetComponent<PlayerMovement>();
        HealthAndStamina = GetComponent<HealthAndStamina>();
    }

    public void OnStanceSwitch()
    {
                WolfAnim.ResetTrigger("ToTwos");
        WolfAnim.ResetTrigger("ToFours");
        if (PlayerMoveScript.CurrentForm == PlayerMovement.Form.Wolf2Legs)
        {
            PlayerMoveScript.CurrentForm = PlayerMovement.Form.Wolf4Legs;
            WolfAnim.SetTrigger("ToFours");
        }
        else if (PlayerMoveScript.CurrentForm == PlayerMovement.Form.Wolf4Legs)
        {
            PlayerMoveScript.CurrentForm = PlayerMovement.Form.Wolf2Legs;
            WolfAnim.ResetTrigger("ToTwos");
            WolfAnim.SetTrigger("ToTwos");
        }
    }

    public void OnTransform()
    {
        StartCoroutine(TransformAnimation()); //this make the 2 models play the same animation to sync up the TF
    }

    private IEnumerator TransformAnimation()
    {
        if (PlayerMoveScript.CurrentForm == PlayerMovement.Form.Human && HealthAndStamina.CurrentEnergy() > EnergyRequiredToTransformWolf && IsTransforming == false)//gives a quick check to see if you should be transforming into a wolf or a human
        {
            WolfAnim.ResetTrigger("WolfForm");//finaly starts the animations
            HumanAnim.ResetTrigger("WolfForm");
            WolfFormMesh.SetActive(false);//makes sure all of the models are being shown or hiden properly 
            HumanFormMesh.SetActive(true);
            IsTransforming = true;
            HealthAndStamina.UseEnergy(EnergyRequiredToTransformWolf);
            WolfAnim.SetTrigger("WolfForm");//finaly starts the animations
            HumanAnim.SetTrigger("WolfForm");

            PlayerMoveScript.CurrentForm = PlayerMovement.Form.Wolf2Legs;

            yield return new WaitForSeconds(2.1833f);//waits to swaps the models at the peek of animation

            HumanFormMesh.SetActive(false);
            WolfFormMesh.SetActive(true);//swaps the model from human to wolf.
            IsTransforming = false;
        }
        else if (PlayerMoveScript.CurrentForm != PlayerMovement.Form.Human && HealthAndStamina.CurrentEnergy() > EnergyRequiredToTransformHuman && IsTransforming == false)//if is not human then go back from were-st you came beast
        {
            yield return new WaitForSeconds(2.1833f);
            WolfFormMesh.SetActive(false);
            HumanFormMesh.SetActive(true);
            IsTransforming = true;
            HealthAndStamina.UseEnergy(EnergyRequiredToTransformHuman);
            PlayerMoveScript.CurrentForm = PlayerMovement.Form.Human;
            IsTransforming = false;
        }
    }


}
