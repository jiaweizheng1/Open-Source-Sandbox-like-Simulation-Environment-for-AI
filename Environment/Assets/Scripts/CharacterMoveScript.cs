using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

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
    
    public float BarSpeedMulti;
    [Header("Player Health")]
    private float MaxHealth = 100f;
    private float Health;
    public Slider HealthSlider;
    [Header("Player Hunger")]
    private float MaxHunger = 100f;
    private float Hunger;
    public Slider HungerSlider;
    [Header("Player Thirst")]
    private float MaxThirst = 100f;
    private float Thirst;
    public Slider ThirstSlider;

    public Light directionallight;
    public LightingPreset preset;
    private DateTime time;
    public float timemulti;
    public TMP_Text time_t;
    public TMP_Text day_t;
    
    public override void OnEpisodeBegin()
    {
        //initially, stay at current place(which is itself)
        transform.position = new Vector3(150, 1.36f, 25);
        target = transform;
        input = new Vector2(0, 0);

        time = new DateTime();

        moving = false;
        busy = false;
        animator.SetBool("deadge", false);
        animator.SetBool("harvesting", false);
        animator.SetFloat("speed", 0);

        log = 0;
        log_t.text = "x" + log;
        food = 0;
        food_t.text = "x" + food;
        droplet = 0;
        droplet_t.text = "x" + droplet;
        iron = 0;
        iron_t.text = "x" + iron;

        Health = MaxHealth;
        Hunger = MaxHunger;
        Thirst = MaxThirst;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.transform.position);
        sensor.AddObservation(trees);
        sensor.AddObservation(farm);
        sensor.AddObservation(pool);
        sensor.AddObservation(rocks);
        sensor.AddObservation(log);
        sensor.AddObservation(food);
        sensor.AddObservation(droplet);
        sensor.AddObservation(iron);
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

    IEnumerator Consumefood()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        food--;
        if(Hunger + 20 > MaxHunger){
            Hunger=MaxHunger;
        }
        else{
            Hunger += 20;
        }

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
    IEnumerator Consumedroplets()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        droplet--;
        if(Thirst + 20 > MaxThirst){
            Thirst=MaxThirst;
        }
        else{
            Thirst += 20;
        }
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

    IEnumerator Die()
    {
        busy = true;
        animator.SetBool("deadge", true);
        yield return new WaitForSeconds(2);
        EndEpisode();
    }

    public override void OnActionReceived(ActionBuffers vetcaction)
    {
        if(!moving && !busy)
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
            if(vetcaction.DiscreteActions[0] == 4)
            {
                if(food>0){
                    StartCoroutine(Consumefood());
                }
                
            }
            if(vetcaction.DiscreteActions[0] == 5)
            {
                if(droplet>0){
                    StartCoroutine(Consumedroplets());
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
        else if(Input.GetKey(KeyCode.T))
        {
            discreteactions[0] = 4;
        }
        else if(Input.GetKey(KeyCode.Y))
        {
            discreteactions[0] = 5;
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

        time = time.AddSeconds(Time.deltaTime * timemulti);
        UpdateLighting((time.Hour + time.Minute/60f)/24f);
        time_t.text = time.ToString("HH:mm");
        day_t.text = (time.Day - 1).ToString();

        HealthSlider.value = Health / MaxHealth;
        HungerSlider.value = Hunger / MaxHunger;
        ThirstSlider.value = Thirst / MaxThirst;
        if(Hunger > 0)
        {
            Hunger = Hunger - BarSpeedMulti * Time.deltaTime;
        }
        if(Thirst > 0)
        {
            Thirst = Thirst - BarSpeedMulti * Time.deltaTime;
        }
        if(Health > 0 && HungerSlider.value == 0){
            Health = Health - BarSpeedMulti * 2 * Time.deltaTime;
        }
        if(Health > 0 && ThirstSlider.value == 0){
            Health = Health - BarSpeedMulti * 2 * Time.deltaTime;
        }
        if(Health < 0)
        {
            StartCoroutine(Die());
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);
        directionallight.color = preset.DirectionalColor.Evaluate(timePercent);
        directionallight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
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
