using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CharacterMoveScript : Agent
{
    public float speed;
    public Transform trees, farm, pool, rocks;
    public Animator animator;
    public CharacterController controller;
    public TMP_Text log_t, food_t, droplet_t, iron_t;

    private int log, food, droplet, iron;
    private Transform target;
    private bool moving, busy;
    private Vector2 input;
    private Vector3 direction;
    private float turnsmoothtime = 0.05f, turnsmoothvelocity = 1f;
    private float gravity = -9.81f, gravitymulti = 3f, velocity;

    public override void OnEpisodeBegin()
    {
        //initially, stay at current place(which is itself)
        target = transform;
        transform.position = new Vector3(150, 1, 20);
        log = 0;
        food = 0;
        droplet = 0;
        iron = 0;
        moving = false;
        busy = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.transform.position);
        //Add all the important observations like HP, food, thirst, inventory items
    }

    IEnumerator Log()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        log++;
        log_t.text = "x" + log;
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Gatherfood()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        food++;
        food_t.text = "x" + food;
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Collectdroplets()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        droplet++;
        droplet_t.text = "x" + droplet;
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Mine()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        iron++;
        iron_t.text = "x" + iron;
        animator.SetBool("harvesting", false);
        busy = false;
    }

    public override void OnActionReceived(ActionBuffers vetcaction)
    {
        if(!busy)
        {
            if(vetcaction.DiscreteActions[0] == 0)
            {
                if(needtomove(trees))
                {
                }
                else
                {
                    StartCoroutine(Log());
                }
            }
            if(vetcaction.DiscreteActions[0] == 1)
            {
                if(needtomove(farm))
                { 
                }
                else
                {
                    StartCoroutine(Gatherfood());
                }
            }
            if(vetcaction.DiscreteActions[0] == 2)
            {
                if(needtomove(pool))
                { 
                }
                else
                {
                    StartCoroutine(Collectdroplets());
                }
            }
            if(vetcaction.DiscreteActions[0] == 3)
            {
                if(needtomove(rocks))
                {
                }
                else
                {
                    StartCoroutine(Mine());
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsout)
    {
        ActionSegment<int> discreteactions = actionsout.DiscreteActions;
        if(Input.GetKey(KeyCode.Q))
        {
            discreteactions[0] = 0;
        }
        else if(Input.GetKey(KeyCode.W))
        {
            discreteactions[0] = 1;
        }
        else if(Input.GetKey(KeyCode.E))
        {
            discreteactions[0] = 2;
        }
        else if(Input.GetKey(KeyCode.R))
        {
            discreteactions[0] = 3;
        }
        else
        {
            discreteactions[0] = -1; //else, invalid input
        }
    }

    public bool needtomove(Transform targetlocation)
    {
        if(Vector3.Distance(targetlocation.transform.position, controller.transform.position) > 0.9)
        {
            target = targetlocation;
            moving = true;
            return true;
        }
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(moving && !busy)
        {
            if(Vector3.Distance(target.transform.position, controller.transform.position) > 0.9)
            {
                input = new Vector2(target.transform.position.x - controller.transform.position.x, target.transform.position.z - controller.transform.position.z);
                input.Normalize();
                direction = new Vector3(input.x, 0, input.y);
                ApplyGravity();
                ApplyRotation();
                ApplyMovement();
            }
            else
            {
                input = new Vector2(0, 0);
                moving = false;
            }
        }
        animator.SetFloat("speed", input.sqrMagnitude);
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
