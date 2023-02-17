using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveScript : MonoBehaviour
{
    public Transform parentinteractionpoint;
    private Transform interactionpoint;
    private bool inrange;
    public Animator animator;
    public CharacterController controller;
    private Vector2 input;
    private Vector3 direction;
    public float speed;
    private float turnsmoothtime = 0.05f;
    private float turnsmoothvelocity = 1f;
    private float gravity = -9.81f;
    private float gravitymulti = 3f;
    private float velocity;

    void Start()
    {
        // initially, stay at current place(which is itself)
        interactionpoint = controller.transform;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Interactable"))
        {
            inrange = true;
            Debug.Log("player in range");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Interactable"))
        {
            inrange = false;
            Debug.Log("player not in range");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // input.x = Input.GetAxisRaw("Horizontal");
        // input.y = Input.GetAxisRaw("Vertical");
        if(inrange && Input.inputString == "e")
        {
            animator.SetBool("harvesting", true);
        }

        if(Input.anyKeyDown)
        {
            if(Input.inputString == "1")
            {
                interactionpoint = parentinteractionpoint.GetChild(0);
            }
            if(Input.inputString == "2")
            {
                interactionpoint = parentinteractionpoint.GetChild(1);
            }
            if(Input.inputString == "3")
            {
                interactionpoint = parentinteractionpoint.GetChild(2);
            }
            if(Input.inputString == "4")
            {
                interactionpoint = parentinteractionpoint.GetChild(3);
            }
        }

        if(Vector3.Distance(interactionpoint.transform.position, controller.transform.position) > 0.9)
        {
            animator.SetBool("harvesting", false);
            input = new Vector2(interactionpoint.transform.position.x - controller.transform.position.x, interactionpoint.transform.position.z - controller.transform.position.z);
        }
        else
        {
            input = new Vector2(0, 0);
        }
        input.Normalize();
        animator.SetFloat("speed", input.sqrMagnitude);
        direction = new Vector3(input.x, 0, input.y);
        
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if(controller.isGrounded && velocity < 0)
        {
            velocity = -1;
        }
        else
        {
            velocity += gravity * gravitymulti * Time.deltaTime;
        }

        direction.y = velocity;
    }

    private void ApplyRotation()
    {
        if(input.sqrMagnitude == 0) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnsmoothvelocity, turnsmoothtime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void ApplyMovement()
    {
        controller.Move(direction * speed * Time.deltaTime);
    }
}
