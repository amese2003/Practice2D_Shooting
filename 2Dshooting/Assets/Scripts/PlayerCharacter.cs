using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MasterCharacter
{
    [SerializeField] private PlayerController2D controller;

    // Update is called once per frame
    void Update()
    {
        InputMove();
        InputShoot();
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    private void InputMove()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
        character_animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            character_animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))        
            crouch = true;
        
        else if (Input.GetButtonUp("Crouch"))        
            crouch = false;        
    }

    private void InputShoot()
    {
        if (Input.GetButtonDown("Fire1"))
            Attack();
    }

    public void OnLanding()
    {
        character_animator.SetBool("IsJumping", false);
    }

    public void OnCrouching()
    {
        character_animator.SetBool("IsCrouching", crouch);
    }
    protected override void Attack()
    {
        if (!bulletPrefab) return;
        Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }

}
