using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectData : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 5f;
    public float staminaCost = 20f;
    public Slider staminaSlider;
    private float currentStamina;
    private bool canDash = false;

    [Header("Health")]
    public bool isDead = false;
    public float maxHealth = 100f;
    public float healthRegenRate = 5f;
    public Slider healthSlider;
    public float currentHealth;

    [SerializeField] private ParticleSystem blood;
    private void Awake()
    {
        currentStamina = maxStamina;
        if (CompareTag("Player"))
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        
        currentHealth = maxHealth;
        if (CompareTag("Player"))
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }            
    }

    private void Update()
    {
        if (CompareTag("Player"))
        {
            if (currentStamina > staminaCost) canDash = true;
            else canDash = false;

            RegenerateStamina();
            UpdateStaminaUI();
            RegenerateHealth();
            UpdateHealthUI();
        }

        if (currentHealth <= 0f)
            isDead = true;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public bool PlayerCanDash()
    {
        return canDash;
    }

    public void AddHealth(float health)
    {
        currentHealth += health;
    }

    public void DealDamage(float damage, bool isThatExplosion)
    {
        currentHealth -= damage;
        if (!isThatExplosion) return;

        if(currentHealth <= 0f)
            Instantiate(blood, transform.position, blood.transform.rotation);
    }

    public void AddStamina(float stamina)
    {
        currentStamina += stamina;
    }

    public void RemoveStamina()
    {
        currentStamina -= staminaCost;
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private void UpdateStaminaUI()
    {
        staminaSlider.value = currentStamina;
    }

    private void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
    }
}
