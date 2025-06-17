using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Combat WIP")]
    public float critChance = 0.1f;
    public float critMultiplier = 2f;
    public float dmgAmplifier = 1f;

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

    [Header("Weapon Attack")]
    public float weaponCooldown = 0.5f;
    private float lastAttackTime;
    [SerializeField] private int weaponDamage = 1;

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
            if (found != null) weaponTransform = found;
            else Debug.LogWarning("Weapon transform not assigned and not found as child.");
        }

        if (weaponTransform != null)
        {
            weaponAnimator = weaponTransform.GetComponent<Animator>();
            if (weaponAnimator == null)
                Debug.LogWarning("Animator not found on weapon.");
        }
    }

    private void Update()
    {
        // Handle input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector2(moveX, moveY).normalized;
        isMoving = inputDirection.magnitude > 0;

        RotateWeaponToMouse();

        // Handle attack
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + weaponCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformWeaponAttack());
        }
    }

    private void FixedUpdate()
    {
        if (isMoving) MoveEntity(walkSpeed);
        else rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, glideFriction * Time.fixedDeltaTime);
    }

    public override void MoveEntity(float moveSpeed)
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, glideFriction * Time.fixedDeltaTime);
    }

    private void RotateWeaponToMouse()
    {
        if (weaponTransform == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponTransform.rotation = Quaternion.Euler(0, 0, angle - 45f);
        Vector3 offset = direction * weaponDistance;
        weaponTransform.position = transform.position + offset;
    }

    private IEnumerator PerformWeaponAttack()
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("Attack1");
        }

        if (weaponTransform != null)
        {
            Collider2D weaponCol = weaponTransform.GetComponent<Collider2D>();
            if (weaponCol != null)
            {
                weaponCol.enabled = true;
                yield return new WaitForSeconds(0.1f);
                DetectEnemiesWithWeapon(weaponCol);
                weaponCol.enabled = false;
            }
        }
    }

    private void DetectEnemiesWithWeapon(Collider2D weaponCol)
    {
        Collider2D[] hits = new Collider2D[10];
        int count = Physics2D.OverlapCollider(weaponCol, new ContactFilter2D().NoFilter(), hits);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = hits[i];
            if (hit != null && hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int damage = CalculateDamage();
                    enemy.TakeDamage(damage);
                    Debug.Log($"Hit enemy: {enemy.name}, damage: {damage}");
                }
            }
        }
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
}
