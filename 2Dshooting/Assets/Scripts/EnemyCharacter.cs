using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MasterCharacter
{   
    private void Start()
    {
        unitHeight = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.y);
    }
    protected override void Die()
    {
        if (deathEffect)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
