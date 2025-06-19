using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int Type;
    public bool IsBoss;
    public int DefaultDamage;
    public int DifficultyDMGAmplifier;

    public void PickAction()
    {
        // Simple example AI behavior placeholder
        Debug.Log($"{characterName} (Enemy) is deciding what to do...");
    }

    /*public override void Attack(Character target)
    {
        int baseDamage = DefaultDamage * DifficultyDMGAmplifier;
        target.TakeDamage(baseDamage);
        Debug.Log($"{characterName} attacked {target.characterName} for {baseDamage} damage.");
    }*/

    private void Start()
    {
        characterType = 2;
        SetUpComponents();
        AutoFindWeapon();

    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        inputDirection = new Vector2(moveX, moveY).normalized;
        DetermineMovementDirection();

        RotateWeaponToMouse();
        //meer gebruik van functies
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + weaponCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformWeaponAttack());
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }



}

