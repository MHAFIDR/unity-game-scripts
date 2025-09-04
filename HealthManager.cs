using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.UI;
using TMPro;

public class HealthManager : MonoBehaviour
{
    [Header("CORE STATS")]
    public float setHealth;
    public Animator animator;
    public Component[] componentsToDisable;

    [Header("EFFECTS & AUDIO")]
    public AudioClip bloodSFX;
    public GameObject bloodFX;
    public GameObject sfxPrefabs;
    public AudioClip[] hurtClip;

    [Header("UI (Optional)")]
    public Slider healthBarSlider;
    public TMP_Text healthBarText;

    [Header("DAMAGE MULTIPLIER (For Body Parts)")]
    public bool isHealthPart;
    public float damageMultiplier;
    public HealthManager healthManager;

    [Header("DEBUG OUTPUT")]
    public bool isCustomBloodSplat;
    public Vector3 customBloodSplat;
    public bool setLayerIKMC;

    public float currentHealth;

    public float CURRENTHEALTH
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = Mathf.Max(value, 0);
                OncurrentHealthChanged();
            }
        }
    }

    private void Start()
    {
        currentHealth = setHealth;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = setHealth;
            healthBarSlider.value = currentHealth;
        }
        if (healthBarText != null)
        {
            healthBarText.text = $"{currentHealth}/{setHealth}";
        }
    }

    public void TakeDamage(float damage)
    {
        if (isHealthPart)
        {
            if (healthManager != null)
            {
                damage *= damageMultiplier;
                healthManager.TakeDamage(damage);
            }
        }
        else
        {
            CURRENTHEALTH -= damage;
            PlayBloodSFX();
        }
    }

    public void OncurrentHealthChanged()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
        if (healthBarText != null)
        {
            healthBarText.text = $"{currentHealth}/{setHealth}";
        }

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.Play("Die");
                if (setLayerIKMC)
                {
                    animator.SetLayerWeight(1, 0);
                }
            }

            if (componentsToDisable != null && componentsToDisable.Length > 0)
            {
                foreach (var component in componentsToDisable)
                {
                    if (component != null)
                    {
                        Destroy(component);
                    }
                }
            }
        }

        if (CURRENTHEALTH > setHealth)
        {
            CURRENTHEALTH = setHealth;
        }
    }

    public void PlayBloodSFX()
    {
        Vector3 bloodSplatPosition = isCustomBloodSplat
            ? customBloodSplat
            : new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        if (bloodFX != null)
        {
            LeanPool.Spawn(bloodFX, bloodSplatPosition, Quaternion.identity);
        }

        if (sfxPrefabs != null && hurtClip != null && hurtClip.Length > 0)
        {
            GameObject audioClone = LeanPool.Spawn(sfxPrefabs, bloodSplatPosition, Quaternion.identity);
            AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
            audioSourceClone.clip = hurtClip[Random.Range(0, hurtClip.Length)];
            audioSourceClone.Play();
        }

        if (bloodSFX != null)
        {
            AudioSource.PlayClipAtPoint(bloodSFX, bloodSplatPosition);
        }
    }
}