using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Combat WIP")]
    public float critChance;
    public float critMultiplier;
    public float dmgAmplifier;
    [Header("Coins/Upgrades WIP")]
    public int coins;
    public int upgrades;
    [Header("Movement")]
    public float glideFriction = 5f;
    private Rigidbody2D rb;
    private Vector2 inputDirection;
    private bool isMoving;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        characterType = 1;
    }

    private void Update()
    {
        // Get input from WASD or arrow keys
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector2(moveX, moveY).normalized;

        // Set isMoving flag if there's input
        isMoving = inputDirection.magnitude > 0;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveEntity(walkSpeed);
        }
        else
        {
            // Apply friction to slow down gradually when no input
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, glideFriction * Time.fixedDeltaTime);
        }
    }

    public override void MoveEntity(float moveSpeed)
    {
        // Smoothly glide toward input direction velocity
        Vector2 targetVelocity = inputDirection * moveSpeed;

        // Lerp current velocity towards target velocity for smooth glide effect
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, glideFriction * Time.fixedDeltaTime);
    }


    public int CalculateDamage()
    {
        //wip
        return 0;
    }

    public override void Attack(Character target)
    {
        int damage = CalculateDamage();
        target.TakeDamage(damage);
        Debug.Log($"{characterName} attacked {target.characterName} for {damage} damage.");
    }

}
