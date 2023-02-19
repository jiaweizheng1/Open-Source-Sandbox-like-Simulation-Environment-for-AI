using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CharacterMoveScript : Agent
{
    //false if AI, true if Player
    public bool manualtesting;
    public Transform parentinteractionpoint;
    private Transform interactionpoint;
    public Animator animator;
    public CharacterController controller;
    private Vector2 input;
    private Vector3 direction;
    private bool inrange;
    public float speed;
    private float turnsmoothtime = 0.05f;
    private float turnsmoothvelocity = 1f;
    private float gravity = -9.81f;
    private float gravitymulti = 3f;
    private float velocity;
    private int action;

    void Start()
    {
        //initially, stay at current place(which is itself)
        interactionpoint = controller.transform;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(150, 1.4f, 20);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.position);
        //Add all the important observations like HP, food, thirst, inventory
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(!manualtesting)
        {
            action = actions.DiscreteActions[0];
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Interactable"))
        {
            inrange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Interactable"))
        {
            inrange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(manualtesting)
        {
            Debug.Log(Input.inputString);
            if(Input.inputString == "0")
            {
                action = 0;
            }
            if(Input.inputString == "1")
            {
                action = 1;
            }
            if(Input.inputString == "2")
            {
                action = 2;
            }
            if(Input.inputString == "3")
            {
                action = 3;
            }
            if(Input.inputString == "4")
            {
                action = 4;
            }
        }

        if(action == 0)
        {
            interactionpoint = parentinteractionpoint.GetChild(0);
            AddReward(-1f); 
        }
        if(action == 1)
        {
            interactionpoint = parentinteractionpoint.GetChild(1);
            AddReward(-1f); 
        }
        if(action == 2)
        {
            interactionpoint = parentinteractionpoint.GetChild(2);
            AddReward(-1f); 
        }
        if(action == 3)
        {
            interactionpoint = parentinteractionpoint.GetChild(3);     
            AddReward(-1f); 
        }
        if(inrange && action == 4)
        {
            animator.SetBool("harvesting", true);
            AddReward(1f);  
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
