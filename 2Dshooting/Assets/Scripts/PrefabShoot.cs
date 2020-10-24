using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabShoot : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private void Update()
    {
        InputShoot();
    }

    private void InputShoot()
    {
        if (Input.GetButtonDown("Fire1"))
            Attack();
    }
    protected void Attack()
    {
        if (!bulletPrefab) return;
        Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
