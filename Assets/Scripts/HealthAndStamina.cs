using System;
 using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthAndStamina : MonoBehaviour
{
    public int Health = 100;
    public float Energy = 100;
    public float BloodLust = 0;
    public Slider HealthBar;
    public Slider EnergyBar;
    public Slider BloodLustBar;

    public TMP_Text EnergyText;
    public TMP_Text HealthText;
    public TMP_Text BloodLustText;

    public string EnergyName = "Energy: ";
    public string HealthName = "Health: ";
    public string BloodLustName = "BLOOD LUST: ";
    public float ShowEnergyBarTime = 1f;

    public GameObject GameController;
    private Stats Stats;

    void Start()
    {
        Stats = GameController.GetComponent<Stats>();
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
        HealthText.text = HealthName + Health.ToString();
        HealthBar.value = Health;
        if (Health < 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }


    public void UseEnergy(float EnergyUsed)
    {
        Energy -= EnergyUsed;
        EnergyText.text = EnergyName + MathF.Round(Energy).ToString();
        //Debug.Log("Energy used: " + EnergyUsed);
        EnergyBar.value = Energy;

    }

    public float CurrentEnergy()
    {
        return Energy;
    }

    public void RegainEngergy(float RegainAmount)
    {
        if (Energy != 100)
        {
            Energy = Mathf.Clamp(Energy + RegainAmount, 0, 100);

        }
        EnergyBar.value = Energy;
        EnergyText.text = EnergyName + MathF.Round(Energy).ToString();
    }

    public void AddBloodLust(float BloodLustAmount, float AddedFeral)
    {

        BloodLustAmount = Mathf.Clamp(BloodLust + BloodLustAmount, 0, 100);
        BloodLustText.text = BloodLustName + MathF.Round(BloodLust).ToString();

    }

    public float CurrentBloodLust()
    {
        return BloodLust;
    }
    
    public void RemoveBloodLust(float BloodLustRemoveAmount)
    {
        BloodLust = Mathf.Clamp(BloodLust - BloodLustRemoveAmount, 0, 100);
        BloodLustText.text = BloodLustName + MathF.Round(BloodLust).ToString();
    }
}
