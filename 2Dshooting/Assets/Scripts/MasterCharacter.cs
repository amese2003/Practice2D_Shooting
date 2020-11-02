using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterCharacter : MonoBehaviour
{
    public Animator character_animator;

    [SerializeField] public float moveSpeed;

    protected float horizontalMove = 0f;
    protected bool jump = false;
    protected bool crouch = false;

    protected int health = 10;
    public int unitHeight { get; protected set; }
    public GameObject deathEffect;

    public Transform firePos;
    public GameObject bulletPrefab;


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Attack() {}
    protected virtual void Die() {}
}
