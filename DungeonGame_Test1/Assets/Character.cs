using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Animator animator; //verplaats naar character
    [SerializeField] protected SpriteRenderer spriteRenderer; //verplaats naar character
    [Header("Character stats")]
    public string characterName;
    public float walkSpeed;
    public int characterType; //1: player, 2: enemy, 3: special_enemy, 4: boss
    public int health;
    public float resistance;
    public int weaponID;
    [SerializeField]protected Transform PlayerSpawn;

    protected Rigidbody2D rb; //verplaats naar character
    protected bool isMoving; //verplaats naar character
    protected Animator weaponAnimator; //verplaats naar character
    [Header("Weapon")]
    public Transform weaponTransform; //verplaats naar character
    public string weaponObjectName = "Weapon"; //verplaats naar character
    [SerializeField] public float weaponDistance = 1f; //verplaats naar character

    [Header("Weapon Attack")]
    public float weaponCooldown = 0.5f; //verplaats naar character
    protected float lastAttackTime ; //verplaats naar character
    [SerializeField] protected int weaponDamage = 1; //verplaats naar character
    protected Vector2 inputDirection;


    public virtual void TakeDamage(int amount)
    {
        // Calculate damage after resistance
        int damageAfterResistance = Mathf.RoundToInt(amount * (1f - resistance));
        health -= damageAfterResistance;

        Debug.Log($"{characterName} took {damageAfterResistance} damage. Remaining health: {health}");

        if (health <= 0)
        {

            Die();
        }
    }

    public virtual void Attack(Character target)
    {
        int damage = 10; // Default base damage, you might override or calculate this
        target.TakeDamage(damage);
        Debug.Log($"{characterName} attacked {target.characterName} for {damage} damage.");
    }

    public virtual void MoveEntity(float moveSpeed)
    {
        // Simple movement logic, can be expanded
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    protected virtual void Die()
    {
        Debug.Log($"{characterName} died.");
        if(characterType == 2)
        {
            print("die");
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject);
            return;
        }
        else
        {
            this.gameObject.transform.position = PlayerSpawn.position;
            health = 3;
        }
        
    }

    protected void SetUpComponents()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found on Player.");
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }
    protected void AutoFindWeapon()
    {
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

    protected void DetermineMovementDirection()
    {

        isMoving = inputDirection.magnitude > 0;

        // Flip only the sprite, not the whole transform!
        //verplaats naar character
        if (inputDirection.x > 0)
            spriteRenderer.flipX = false;
        else if (inputDirection.x < 0)
            spriteRenderer.flipX = true;

        animator.SetFloat("Speed", inputDirection.magnitude);
    }

    protected void RotateWeaponToMouse() //maak in character rotateToTarget en overschrijf deze met onderstaande code.
    {
        if (weaponTransform == null) return;

        GameObject Player = GameObject.Find("Player");
        Vector3 PlayerPos = Player.transform.position;
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        Vector2 direction;
        if(characterType == 1)
        {
             direction = (mouseWorldPos - transform.position).normalized;
        }
        else
        {
             direction = (PlayerPos - transform.position).normalized;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponTransform.rotation = Quaternion.Euler(0, 0, angle - 45f);
        Vector3 offset = direction * weaponDistance;
        weaponTransform.position = transform.position + offset;
    }

   

    protected void DetectEnemiesWithWeapon<T>(Collider2D weaponCol) where T : Character
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(weaponCol, new ContactFilter2D().NoFilter(), colliders);
        foreach (Collider2D collider in colliders)
        {
            T HitTarget;
            if (collider.TryGetComponent<T>(out HitTarget))
            {
                //int damage = CalculateDamage();
                Attack(HitTarget);
            }
        }
    }

    protected virtual IEnumerator PerformWeaponAttack() 
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
                if (characterType != 1)
                {
                    DetectEnemiesWithWeapon<Player>(weaponCol);
                }
                else
                {
                    DetectEnemiesWithWeapon<Enemy>(weaponCol);
                }

                weaponCol.enabled = false;
            }
        }
    }
    
}
