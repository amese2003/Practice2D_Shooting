using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterCharacter : MonoBehaviour
{
    public Animator character_animator;

    [SerializeField] protected float moveSpeed = 40f;

    protected float horizontalMove = 0f;
    protected bool jump = false;
    protected bool crouch = false;

    protected int health = 10;
    public GameObject deathEffect;


    protected virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {       
    }
}
