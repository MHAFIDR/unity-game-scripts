using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    
    private void Start()
    {
        // Validate weapon reference
        if (weapon == null)
        {
            Debug.LogError($"Weapon not assigned on {gameObject.name}");
            enabled = false;
            return;
        }
    }

    public void Hit()
    {
        if (weapon != null)
        {
            weapon.MeleeAttack();
        }
        else
        {
            Debug.LogWarning($"Cannot attack - Weapon is null on {gameObject.name}");
        }
    }
}
