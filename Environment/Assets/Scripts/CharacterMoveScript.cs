using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random=UnityEngine.Random;

public class CharacterMoveScript : Agent
{
    public float speed;
    public Animator animator;
    public CharacterController controller;
    public Transform treeslocation, farmlocation, poollocation, rockslocation, benchlocation, firelocation, rocketlocation;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, copper_t, gold_t, iron_t;
    private int log, apple, meat, oil, water, copper, gold, iron;
    private bool moving, busy;
    private Transform target;
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

    private bool benchbuilt, campfirebuilt, rocketbuilt;
    public GameObject bench, fire, rocket;
    public GameObject benchui, fireui, rocketui;
    private int[] benchbuildmats = {3, 1, 1, 1};
    private int[] firebuildmats = {2, 1};
    private int[] cookmats = {1, 1, 1, 1};
    private int[] rocketbuildmats = {10, 10, 7, 10};
    private int[] rocketlaunchmats = {1, 5, 5, 10, 1, 1, 1};
    
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

        benchbuilt = false;
        bench.transform.Find("BenchBlueprint").gameObject.SetActive(true);
        bench.transform.Find("Bench").gameObject.SetActive(false);
        benchui.transform.Find("UIBuild").gameObject.SetActive(true);
        benchui.transform.Find("UIBuilt").gameObject.SetActive(false);

        campfirebuilt = false;
        fire.transform.Find("FireBlueprint").gameObject.SetActive(true);
        fire.transform.Find("Fire").gameObject.SetActive(false);
        fireui.transform.Find("UIBuild").gameObject.SetActive(true);
        fireui.transform.Find("UICook").gameObject.SetActive(false);

        rocketbuilt = false;
        rocket.transform.Find("RocketBlueprint").gameObject.SetActive(true);
        rocket.transform.Find("Rocket").gameObject.SetActive(false);
        rocketui.transform.Find("UIBuild").gameObject.SetActive(true);
        rocketui.transform.Find("UILaunch").gameObject.SetActive(false);

        log = 0;
        apple = 0;
        meat = 0;
        oil = 0;
        water = 0;
        copper = 0;
        gold = 0;
        iron = 0;
        ManualUpdateAllText();

        Health = MaxHealth;
        Hunger = MaxHunger;
        Thirst = MaxThirst;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.transform.position);

        sensor.AddObservation(treeslocation);
        sensor.AddObservation(farmlocation);
        sensor.AddObservation(poollocation);
        sensor.AddObservation(rockslocation);

        sensor.AddObservation(log);
        sensor.AddObservation(apple);
        sensor.AddObservation(meat);
        sensor.AddObservation(oil);
        sensor.AddObservation(water);
        sensor.AddObservation(copper);
        sensor.AddObservation(gold);
        sensor.AddObservation(iron);
        //Add all the important observations like HP, food, thirst, inventory items
    }

    IEnumerator Log()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        log++;
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Gatherfood()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        int whichfood = Random.Range(0, 2);
        if(whichfood == 0)
        {
            apple++;
        }
        else
        {
            meat++;
        }
        int chanceoil = Random.Range(0, 10);
        if(chanceoil <= 2)
        {
            oil++;
        }
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator CollectWater()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        water++;
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Mine()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        int whichmineral = Random.Range(0, 3);
        if(whichmineral == 0)
        {
            copper++;
        }
        else if(whichmineral == 1)
        {
            gold++;
        }
        else
        {
            iron++;
        }
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildBench()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        benchbuilt = true;
        bench.transform.Find("BenchBlueprint").gameObject.SetActive(false);
        bench.transform.Find("Bench").gameObject.SetActive(true);
        benchui.transform.Find("UIBuild").gameObject.SetActive(false);
        benchui.transform.Find("UIBuilt").gameObject.SetActive(true);
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildFire()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        campfirebuilt = true;
        fire.transform.Find("FireBlueprint").gameObject.SetActive(false);
        fire.transform.Find("Fire").gameObject.SetActive(true);
        fireui.transform.Find("UIBuild").gameObject.SetActive(false);
        fireui.transform.Find("UICook").gameObject.SetActive(true);
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Cook()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        Health = MaxHealth;
        Hunger = MaxHunger;
        Thirst = MaxThirst;
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildRocket()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        rocketbuilt = true;
        rocket.transform.Find("RocketBlueprint").gameObject.SetActive(false);
        rocket.transform.Find("Rocket").gameObject.SetActive(true);
        rocketui.transform.Find("UIBuild").gameObject.SetActive(false);
        rocketui.transform.Find("UILaunch").gameObject.SetActive(true);
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator LaunchRocket()
    {
        busy = true;
        yield return new WaitForSeconds(5);
        EndEpisode();
    }

    IEnumerator Die()
    {
        busy = true;
        animator.SetBool("deadge", true);
        yield return new WaitForSeconds(2);
        EndEpisode();
    }

    public void ManualUpdateAllText()
    {
        log_t.text = "x" + log;
        apple_t.text = "x" + apple;
        meat_t.text = "x" + meat;
        oil_t.text = "x" + oil;
        water_t.text = "x" + water;
        copper_t.text = "x" + copper;
        gold_t.text = "x" + gold;
        iron_t.text = "x" + iron;
    }

    public override void OnActionReceived(ActionBuffers vetcaction)
    {
        if(!moving && !busy)
        {
            if(vetcaction.DiscreteActions[0] == 0)
            {
                if(needtomove(treeslocation))
                {
                }
                else
                {
                    StartCoroutine(Log());
                }
            }
            if(vetcaction.DiscreteActions[0] == 1)
            {
                if(needtomove(farmlocation))
                { 
                }
                else
                {
                    StartCoroutine(Gatherfood());
                }
            }
            if(vetcaction.DiscreteActions[0] == 2)
            {
                if(needtomove(poollocation))
                { 
                }
                else
                {
                    StartCoroutine(CollectWater());
                }
            }
            if(vetcaction.DiscreteActions[0] == 3)
            {
                if(needtomove(rockslocation))
                {
                }
                else
                {
                    StartCoroutine(Mine());
                }
            }
            if(vetcaction.DiscreteActions[0] == 4)
            {
                if(needtomove(benchlocation))
                {
                }
                else if(!benchbuilt && log >= benchbuildmats[0] && copper >= benchbuildmats[1] && gold >= benchbuildmats[2] && iron >= benchbuildmats[3])
                {
                    log -= benchbuildmats[0];
                    copper -= benchbuildmats[1];
                    gold -= benchbuildmats[2];
                    iron -= benchbuildmats[3];
                    ManualUpdateAllText();
                    StartCoroutine(BuildBench());
                }
            }
            if(vetcaction.DiscreteActions[0] == 5)
            {
                if(needtomove(firelocation))
                {
                }
                else if(!campfirebuilt && log >= firebuildmats[0] && iron >= firebuildmats[1])
                {
                    log -= firebuildmats[0];
                    iron -= firebuildmats[1];
                    ManualUpdateAllText();
                    StartCoroutine(BuildFire());
                }
                else if(campfirebuilt && log >= cookmats[0] && apple >= cookmats[1] && meat >= cookmats[2] && water >= cookmats[3])
                {
                    log -= cookmats[0];
                    apple -= cookmats[1];
                    meat -= cookmats[2];
                    water -= cookmats[3];
                    ManualUpdateAllText();
                    StartCoroutine(Cook());
                }
            }
            if(vetcaction.DiscreteActions[0] == 6)
            {
                if(needtomove(rocketlocation))
                {
                }
                else if(!rocketbuilt)
                //  && log >= rocketbuildmats[0] && copper >= rocketbuildmats[1] && gold >= rocketbuildmats[2] && iron >= rocketbuildmats[3])
                {
                    log -= rocketbuildmats[0];
                    copper -= rocketbuildmats[1];
                    gold -= rocketbuildmats[2];
                    iron -= rocketbuildmats[3];
                    ManualUpdateAllText();
                    StartCoroutine(BuildRocket());

                }
                else if(rocketbuilt)
                //  && log >= rocketlaunchmats[0] && apple >= rocketlaunchmats[1] && meat >= rocketlaunchmats[2] && oil >= rocketlaunchmats[3] 
                // && copper >= rocketlaunchmats[4] && gold >= rocketlaunchmats[5] && iron >= rocketlaunchmats[6])
                {
                    log -= rocketlaunchmats[0];
                    apple -= rocketlaunchmats[1];
                    meat -= rocketlaunchmats[2];
                    oil -= rocketlaunchmats[3];
                    copper -= rocketlaunchmats[4];
                    gold -= rocketlaunchmats[5];
                    iron -= rocketlaunchmats[6];
                    ManualUpdateAllText();
                    StartCoroutine(LaunchRocket());
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
        else if(Input.GetKey(KeyCode.U))
        {
            discreteactions[0] = 6;
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
            Health = Health - BarSpeedMulti * Time.deltaTime;
        }
        if(Health > 0 && ThirstSlider.value == 0){
            Health = Health - BarSpeedMulti * Time.deltaTime;
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
