using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float m_jumpForce = 400f;
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = 0.36f;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = 0.05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrounchDisableCollider;
    private bool m_Grounded;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;

    const float k_GroundedRadius = .2f;
    const float k_CeilingRadius = .2f;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        bool was_grounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] collider = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_GroundLayer);

        for(int i = 0; i < collider.Length; i++)
        {
            if(collider[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!was_grounded)
                    OnLandEvent.Invoke();
            }
        }
    }




}
