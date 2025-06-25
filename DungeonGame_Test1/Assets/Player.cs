using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public EnemySpawnerManager enemySpawnerManager;
    [Header("Player Combat WIP")]
    public float critChance = 0.1f;
    public float critMultiplier = 2f;
    public float dmgAmplifier = 1f;

    [Header("Player Coins/Upgrades (Should items)")]
    public int coins;
    public int upgrades;

    [Header("Player Movement")]
    public float glideFriction = 32f;


    private void Start()
    {
        characterType = 1;
        weaponDamage = 3;
        Basehealth = 3;
        health = 3;
        SetUpComponents();
        AutoFindWeapon();
        BoostPlayer();
        BoostPlayer();
    }
   
   void LateUpdate()
{
    RotateWeaponToMouse();
}
    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector2(moveX, moveY).normalized;
        DetermineMovementDirection();
        uI_Handler.UpdatePlayerStats(health, weaponDamage);
        // RotateWeaponToMouse();
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

    public void BoostPlayer()
    {
        Debug.Log("BoostPlayer");
        Basehealth += 5;
        weaponDamage += 5;
        health = Basehealth;
    }
    private void FixedUpdate()
    {
        if (isMoving) MoveEntity(walkSpeed);
        else rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, glideFriction * Time.fixedDeltaTime);
    }

    public override void MoveEntity(float moveSpeed)
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;
        rb.velocity = targetVelocity;
    }

    public int CalculateDamage()
    {
        int damage = Mathf.RoundToInt(weaponDamage * dmgAmplifier);

        // Apply crit chance
        if (Random.value < critChance)
        {
            damage = Mathf.RoundToInt(damage * critMultiplier);
            Debug.Log("CRITICAL HIT!");
        }

        return damage;
    }

    public override void Attack(Character target)
    {
        int damage = CalculateDamage();
        target.TakeDamage(damage);
        Debug.Log($"{characterName} attacked {target.characterName} for {damage} damage.");
    }

    public void BoostHealth()
    {
        Basehealth += 5;
    }

    public void BoostDamage()
    {
        weaponDamage += 3;
    }
}
