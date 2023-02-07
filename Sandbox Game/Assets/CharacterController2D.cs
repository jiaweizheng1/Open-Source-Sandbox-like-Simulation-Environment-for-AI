using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    public float collisionOffset = 0.05f;
    public Rigidbody2D rigidbody2d;
    public float movespeed;
    public Vector2 motionvector;
    public Animator animator;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

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
        if(motionvector != Vector2.zero){
            bool success = TryMove(motionvector);
            if(!success){
                success = TryMove(new Vector2(motionvector.x,0));
                if(!success){
                    success = TryMove(new Vector2(0,motionvector.y));
                }
            }
        }
        Move();
    }
    private bool TryMove(Vector2 direction){
            int count = rigidbody2d.Cast(
                motionvector,
                movementFilter,
                castCollisions,
                movespeed * Time.fixedDeltaTime + collisionOffset);
            if(count == 0){
                Move();
                return true;
            }
            else{
                return false;
            }
    }
    private void Move()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + motionvector * movespeed * Time.deltaTime);
    }
}
