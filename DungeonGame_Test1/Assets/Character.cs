using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character stats")]
    public string characterName;
    public float walkSpeed;
    public int characterType; //1: player, 2: enemy, 3: special_enemy, 4: boss
    public int health;
    public float resistance;
    public int weaponID;

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
        if(characterType != 1)
        {
            Destroy(gameObject);
            return;
        }
        
    }
}
