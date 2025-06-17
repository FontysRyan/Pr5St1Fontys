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
    private Animator weaponAnimator;

    [Header("Weapon")]
    public Transform weaponTransform;
    public string weaponObjectName = "Weapon";
    [SerializeField] public float weaponDistance = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        characterType = 1;

        // Auto-find weapon
        if (weaponTransform == null)
        {
            Transform found = transform.Find(weaponObjectName);
            if (found != null)
            {
                weaponTransform = found;
            }
            else
            {
                Debug.LogWarning("Weapon transform not assigned and not found as child.");
            }
        }

        // Find animator on weapon
        if (weaponTransform != null)
        {
            weaponAnimator = weaponTransform.GetComponent<Animator>();
            if (weaponAnimator == null)
            {
                Debug.LogWarning("Animator not found on weapon.");
            }
        }
    }

    private void RotateWeaponToMouse()
    {
        // Rotate weapon toward mouse
        if (weaponTransform != null)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            weaponTransform.rotation = Quaternion.Euler(0, 0, angle - 45f);
            Vector3 offset = direction * weaponDistance;
            weaponTransform.position = transform.position + offset;
        }
    }
    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector2(moveX, moveY).normalized;
        isMoving = inputDirection.magnitude > 0;

        RotateWeaponToMouse();

        // Play attack animation on left click
        if (Input.GetMouseButtonDown(0))
        {
            weaponAnimator?.SetTrigger("Attack1");
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveEntity(walkSpeed);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, glideFriction * Time.fixedDeltaTime);
        }
    }

    public override void MoveEntity(float moveSpeed)
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, glideFriction * Time.fixedDeltaTime);
    }

    public int CalculateDamage()
    {
        return 0;
    }

    public override void Attack(Character target)
    {
        int damage = CalculateDamage();
        target.TakeDamage(damage);
        Debug.Log($"{characterName} attacked {target.characterName} for {damage} damage.");
    }
}
