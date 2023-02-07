using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    public Rigidbody2D rigidbody2d;
    public float movespeed;
    public Vector2 motionvector;
    public Animator animator;

    void Update()
    {
        motionvector.x = Input.GetAxisRaw("Horizontal");
        motionvector.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("horizontal", motionvector.x);
        animator.SetFloat("vertical", motionvector.y);
        animator.SetFloat("speed", motionvector.sqrMagnitude);

        if(motionvector.magnitude > 0)
        {
            animator.SetFloat("lasthorizontal", motionvector.x);
            animator.SetFloat("lastvertical", motionvector.y);
        }

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + motionvector * movespeed * Time.deltaTime);
    }
}
