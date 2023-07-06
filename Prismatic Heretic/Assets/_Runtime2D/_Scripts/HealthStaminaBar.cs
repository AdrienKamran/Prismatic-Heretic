using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStaminaBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider staminaSlider;

    private void Start()
    {
        InvokeRepeating("GiveBackStamina", 0f, 0.5f);
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void SetMaxStamina(int stamina)
    {
        staminaSlider.maxValue = stamina;
        staminaSlider.value = stamina;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void SetStamina(int stamina)
    {
        staminaSlider.value = stamina;
    }

    void GiveBackStamina()
    {
        if(staminaSlider.value < 100)
        {
            staminaSlider.value += 2;
            Player.currentStamina = (int)staminaSlider.value;
        }
    }
}
