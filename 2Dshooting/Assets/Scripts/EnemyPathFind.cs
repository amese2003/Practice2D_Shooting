using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFind : MonoBehaviour
{
    public CircleCollider2D searchCollider;


    // Start is called before the first frame update
    void Start()
    {
        if (!searchCollider)
            searchCollider = transform.Find("FindEnemy").gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Test()
    {
    }

    



}
