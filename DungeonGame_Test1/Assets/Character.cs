using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public UI_handler uI_Handler;
    public EnemySpawnerManager enemySpawnerManager;
    protected Animator animator; 
    [SerializeField] protected SpriteRenderer spriteRenderer; 
    [Header("Character stats")]
    public string characterName;
    public float walkSpeed;
    public int characterType; //1: player, 2: enemy, 3: special_enemy, 4: boss
    public int health;
    protected int Basehealth;
    public float resistance;
    public int weaponID;
    [SerializeField]protected Transform PlayerSpawn;
    protected GameObject PlayerChar;
    protected Rigidbody2D rb; 
    protected bool isMoving; 
    protected Animator weaponAnimator; 
    [Header("Weapon")]
    public Transform weaponTransform; 
    public string weaponObjectName = "Weapon";
    [SerializeField] public float weaponDistance = 1f; 

    [Header("Weapon Attack")]
    public float weaponCooldown = 0.5f; 
    protected float lastAttackTime ; 
    [SerializeField] protected int weaponDamage = 1; 
    protected Vector2 inputDirection;
    protected float PlayerDetectionDistance = 5f;

    public virtual void TakeDamage(int amount)
    {
        // Calculate damage after resistance
        int damageAfterResistance = Mathf.RoundToInt(amount * (1f - resistance));
        health -= damageAfterResistance;

        Debug.Log($"{characterName} took {damageAfterResistance} damage. Remaining health: {health}");

        if (health <= 0)
        {
            enemySpawnerManager.AddScore(50);
            Debug.Log("score");
            Die();

        }
    }

    public virtual void Attack(Character target)
    {
        int damage = 10; // Default base damage, you might override or calculate this
        Debug.Log("FFFFFF");
        target.TakeDamage(damage);
        Debug.Log($"{characterName} attacked {target.characterName} for {damage} damage.");
    }

    public virtual void MoveEntity(float moveSpeed)
    {
        //Debug.Log("move " + walkSpeed);
        if (PlayerChar == null || rb == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = PlayerChar.transform.position;
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance >= PlayerDetectionDistance) return;

        Vector2 direction = (targetPosition - currentPosition).normalized;
        
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
        rb.MovePosition(currentPosition + direction * moveSpeed * Time.deltaTime);



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
            Basehealth = 3;
            enemySpawnerManager.currentWave = 0;
            enemySpawnerManager.PlayerDiedResetGame();
            enemySpawnerManager.score = 0;
            health = Basehealth;

        }
        
    }

    protected void SetUpComponents()
    {
        PlayerChar = GameObject.Find("Player");
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

        // Update animator state based on movement, but is not being used yet to prevent errors commented out

        // animator.SetFloat("Speed", inputDirection.magnitude);
    }

    protected void RotateWeaponToMouse() //rotate weapon to mouse position or player position based on character type
    {
        if (weaponTransform == null) return;

        
        Vector3 PlayerPos = PlayerChar.transform.position;
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
        else
        {
            
        }
        Debug.Log("heheheh");
    }

    public void BoostHealth()
    {
        Basehealth += 5;
        health = Basehealth;
    }

    public void BoostDamage()
    {
        weaponDamage += 3;
    }

}
