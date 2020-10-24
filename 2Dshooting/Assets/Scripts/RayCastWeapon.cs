using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeapon : MonoBehaviour
{
    public Transform firePos;
    public int damage = 40;
    public GameObject impactEffect;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputShoot();
    }

    private void InputShoot()
    {
        if (Input.GetButtonDown("Fire1"))
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePos.position, firePos.right);

        if (hitInfo)
        {
            MasterCharacter enemy = hitInfo.transform.GetComponent<MasterCharacter>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Instantiate(impactEffect, hitInfo.point, Quaternion.identity);

            lineRenderer.SetPosition(0, firePos.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }

        else
        {
            lineRenderer.SetPosition(0, firePos.position);
            lineRenderer.SetPosition(1, firePos.position + firePos.right * 100);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.02f);
        lineRenderer.enabled = false;
    }
}
