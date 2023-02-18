using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveScript : MonoBehaviour
{
    public Vector2 input;
    public CharacterController controller;
    public Vector3 direction;
    public float speed;
    private float turnsmoothtime = 0.05f;
    private float turnsmoothvelocity = 1;
    public float gravity = -9.81f;
    public float gravitymulti = 3f;
    private float velocity;
    public Animator animator;
    [SerializeField] private UI_inventory uiInventory;
    private Inventory inventory;
    void Awake(){
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }
    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        direction = new Vector3(input.x, 0.0f, input.y);
         animator.SetFloat("speed", input.sqrMagnitude);
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if(controller.isGrounded && velocity < 0f)
        {
            velocity = -1f;
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
