using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int Type;
    public bool IsBoss;
    public int DefaultDamage = 2;
    public int DifficultyDMGAmplifier;

    protected bool SeesPlayer = false;
    protected bool PlayerInWeaponRange = false;
    protected float DetectionRange;
    protected float WeaponRange;


    public void PickAction()
    {
        // Simple example AI behavior placeholder

        FindPlayer();
        if (SeesPlayer)
        {
            RotateWeaponToMouse();
            //Debug.Log($"{characterName} Spotted a player");
            
                //attack
                //Debug.Log($"{characterName} Attacked a player");
                if (Time.time >= lastAttackTime + weaponCooldown)
                {
                    lastAttackTime = Time.time;
                    StartCoroutine(PerformWeaponAttack());
                }
                //Debug.Log($"{characterName} Moves towards a player");

            
        } 
        
        //Debug.Log($"{characterName} (Enemy) is deciding what to do...");

    }
    private void FindPlayer()
    {
        if (CalculatePlayerDistance() <= PlayerDetectionDistance)
        {
            SeesPlayer = true;
            weaponTransform.gameObject.SetActive(true);
        }
        else
        {
            SeesPlayer = false;
            weaponTransform.gameObject.SetActive(false);
        }
        
    }
    public override void Attack(Character target)
    {   
        int baseDamage = DefaultDamage;
        Debug.Log(baseDamage + "Default: " + DefaultDamage + " Difficulty: " + DifficultyDMGAmplifier);
        target.TakeDamage(baseDamage);
        Debug.Log($"{characterName} attacked {target.characterName} for {baseDamage} damage.");
    }
    private float CalculatePlayerDistance()
    {
        if (PlayerChar != null)
        {
            Vector2 enemyPos = transform.position;
            Vector2 playerPos = PlayerChar.transform.position;
            return Vector2.Distance(enemyPos, playerPos);
        }

        return -1f; // return -1 if player is missing
    }
    private void Start()
    {
        uI_Handler = GameObject.Find("Canvas").GetComponent<UI_handler>();
        enemySpawnerManager = GameObject.Find("EnemySpawns").GetComponent<EnemySpawnerManager>();
        characterType = 2;
        weaponCooldown = 0.8f;
        SetUpComponents();
        AutoFindWeapon();
        //print("AAAA" + DifficultyDMGAmplifier);
        SetAttributes();


    }

    private void Update()
    {
        //print(CalculatePlayerDistance());
        PickAction();
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveY = Input.GetAxisRaw("Vertical");
        //inputDirection = new Vector2(moveX, moveY).normalized;
        //DetermineMovementDirection();

        if (SeesPlayer) { }


        //meer gebruik van functies

        // Update animator state based on movement (Dont have animator yet so commented out to prevent errors)
        // if (animator != null)
        // {
        //     animator.SetBool("IsMoving", isMoving);
        // }
        
    }

    private void SetAttributes()
    {
        int bonus = (DifficultyDMGAmplifier + 2) / 2;

        Basehealth += bonus;
        health = Basehealth; // reset or sync health to new max
        DefaultDamage += bonus;
    }
    private void FixedUpdate()
    {
        if (SeesPlayer)
        {
            MoveEntity(walkSpeed);
        }
    }
}

