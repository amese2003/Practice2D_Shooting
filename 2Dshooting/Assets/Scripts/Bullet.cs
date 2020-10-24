using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    private MasterCharacter shotFrom;
    private IEnumerator setTimer;

    public void SetShotFrom(MasterCharacter character)
    {
        shotFrom = character;
    }
    private void Start()
    {
        rb.velocity = transform.right * speed;
        SetUpBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MasterCharacter target = collision.GetComponent<MasterCharacter>();

        if(target != null && target != shotFrom)        
            target.TakeDamage(damage);

        if (impactEffect)
            Instantiate(impactEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    private void SetUpBullet()
    {
        setTimer = SetBulletTime(1f);
        StartCoroutine(setTimer);
    }

    IEnumerator SetBulletTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
