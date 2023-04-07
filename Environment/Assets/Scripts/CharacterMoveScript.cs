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
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, iron_t, gold_t, diamond_t;
    private int log, apple, meat, oil, water, iron, gold, diamond;
    private bool alive, moving, busy;
    private Vector3 target;
    private Vector2 input;
    private Vector3 direction;
    private float turnsmoothtime = 0.1f, turnsmoothvelocity = 0.1f;
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
    public float timescale;
    public TMP_Text time_t;
    public TMP_Text day_t;

    private bool benchbuilt, campfirebuilt, rocketbuilt, axebuilt, scythebuilt, pickaxebuilt;
    public GameObject tools;
    public GameObject bench, fire, rocket, toolblueprints;
    public GameObject benchui, fireui, rocketui, toolui, toolbarui;
    private int[] benchbuildmats = { 3, 1, 1, 1 };
    private int[] firebuildmats = { 2, 1 };
    private int[] cookmats = { 1, 1, 1, 1 };
    private int[] rocketbuildmats = { 10, 10, 10, 10 };
    private int[] rocketlaunchmats = { 1, 5, 5, 10, 1, 1, 1 };
    private int[] toolbuildmats = { 2, 3 };

    void Start()
    {
        Time.timeScale = timescale;
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0);

        log = 0;
        apple = 0;
        meat = 0;
        oil = 0;
        water = 0;
        iron = 0;
        gold = 0;
        diamond = 0;
        ManualUpdateAllText();

        animator.SetBool("deadge", false);
        animator.SetFloat("speed", 0);

        //initially, stay at current place(which is itself)
        transform.position = new Vector3(150, 1.36f, 25);
        target = transform.position;
        input = new Vector2(0, 0);

        time = new DateTime();

        benchbuilt = false;
        bench.transform.Find("BenchBlueprint").gameObject.SetActive(true);
        bench.transform.Find("Bench").gameObject.SetActive(false);
        benchui.transform.Find("UIBuild").gameObject.SetActive(true);

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

        rocket.transform.Find("Rocket").position = new Vector3(160.8f, 4.42f, 19.3f);
        rocket.transform.Find("BigExplosionEffect").gameObject.SetActive(false);
        rocket.transform.Find("SmokeEffect").gameObject.SetActive(false);

        // tool initialize
        axebuilt = false;
        scythebuilt = false; 
        pickaxebuilt = false;
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Axe").gameObject.SetActive(false);
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("PickAxe").gameObject.SetActive(false);
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Scythe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildAxe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildScythe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildPickaxe").gameObject.SetActive(false);

        tools.transform.Find("Axe").gameObject.SetActive(false);
        tools.transform.Find("Scythe").gameObject.SetActive(false);
        tools.transform.Find("Pickaxe").gameObject.SetActive(false);
        tools.transform.Find("Chickenleg").gameObject.SetActive(false);

        // toolbar initialize (axe -> scythe -> pickaxe)
        toolbarui.transform.Find("AxeV1").gameObject.SetActive(false);
        toolbarui.transform.Find("ScytheV1").gameObject.SetActive(false);
        toolbarui.transform.Find("PickAxeV1").gameObject.SetActive(false);

        Health = MaxHealth;
        Hunger = MaxHunger;
        Thirst = MaxThirst;

        alive = true;
        moving = false;
        busy = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.transform.position);
        sensor.AddObservation(treeslocation);
        sensor.AddObservation(farmlocation);
        sensor.AddObservation(poollocation);
        sensor.AddObservation(rockslocation);
        sensor.AddObservation(benchlocation);
        sensor.AddObservation(firelocation);
        sensor.AddObservation(rocketlocation);

        sensor.AddObservation(log);
        sensor.AddObservation(apple);
        sensor.AddObservation(meat);
        sensor.AddObservation(oil);
        sensor.AddObservation(water);
        sensor.AddObservation(iron);
        sensor.AddObservation(gold);
        sensor.AddObservation(diamond);
        
        sensor.AddObservation(Health);
        sensor.AddObservation(Hunger);
        sensor.AddObservation(Thirst);

        sensor.AddObservation(benchbuilt);
        sensor.AddObservation(campfirebuilt);
        sensor.AddObservation(rocketbuilt);
        sensor.AddObservation(axebuilt);
        sensor.AddObservation(scythebuilt);
        sensor.AddObservation(pickaxebuilt);
    }

    IEnumerator Log()
    {
        busy = true;
        if(axebuilt)
        {
            tools.transform.Find("Axe").gameObject.SetActive(true);
            tools.transform.Find("Scythe").gameObject.SetActive(false);
            tools.transform.Find("Pickaxe").gameObject.SetActive(false);
            animator.SetBool("chopping", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if (axebuilt)
        {
            log += 3;
        }
        else
        {
            log++;
        }
        ManualUpdateAllText();
        animator.SetBool("chopping", false);
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Gatherfood()
    {
        busy = true;
        if(scythebuilt)
        {
            tools.transform.Find("Axe").gameObject.SetActive(false);
            tools.transform.Find("Scythe").gameObject.SetActive(true);
            tools.transform.Find("Pickaxe").gameObject.SetActive(false);
            animator.SetBool("chopping", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if(scythebuilt)
        {
            apple++;
            meat++;
            oil++;
        }
        else
        {
            int whichfood = Random.Range(0, 2);
            if (whichfood == 0)
            {
                apple++;
            }
            else
            {
                meat++;
            }
            int chanceoil = Random.Range(0, 10);
            if (chanceoil <= 2)
            {
                oil++;
            }
        }
        ManualUpdateAllText();
        animator.SetBool("chopping", false);
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
        if(pickaxebuilt)
        {
            tools.transform.Find("Axe").gameObject.SetActive(false);
            tools.transform.Find("Scythe").gameObject.SetActive(false);
            tools.transform.Find("Pickaxe").gameObject.SetActive(true);
            animator.SetBool("mining", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if(pickaxebuilt)
        {
            iron++;
            gold++;
            diamond++;
        }
        else
        {
            int whichmineral = Random.Range(0, 2);
            if (whichmineral == 0)
            {
                iron++;
            }
            else
            {
                gold++;
            }
            int chancediamond = Random.Range(0, 10);
            if (chancediamond <= 2)
            {
                diamond++;
            }
        }
        ManualUpdateAllText();
        animator.SetBool("mining", false);
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
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Axe").gameObject.SetActive(true);
        toolui.transform.Find("UIBuildAxe").gameObject.SetActive(true);
        log -= benchbuildmats[0];
        iron -= benchbuildmats[1];
        gold -= benchbuildmats[2];
        diamond -= benchbuildmats[3];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildAxe()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        axebuilt=true; 
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Axe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildAxe").gameObject.SetActive(false);
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Scythe").gameObject.SetActive(true);
        toolui.transform.Find("UIBuildScythe").gameObject.SetActive(true);
        toolbarui.transform.Find("AxeV1").gameObject.SetActive(true);
        log -= toolbuildmats[0];
        iron -= toolbuildmats[1];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildScythe()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        scythebuilt=true;
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("Scythe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildScythe").gameObject.SetActive(false);
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("PickAxe").gameObject.SetActive(true);
        toolui.transform.Find("UIBuildPickaxe").gameObject.SetActive(true);
        toolbarui.transform.Find("ScytheV1").gameObject.SetActive(true);
        log -= toolbuildmats[0];
        gold -= toolbuildmats[1];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;

        RectTransform rt = toolbarui.transform.Find("InventoryImage").gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(40, 110);
        toolbarui.transform.Find("AxeV1").gameObject.transform.position += new Vector3 (-4, 0, 0);
    }

    IEnumerator BuildPickaxe()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        pickaxebuilt=true;
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("PickAxe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildPickaxe").gameObject.SetActive(false);
        toolbarui.transform.Find("PickAxeV1").gameObject.SetActive(true);
        log -= toolbuildmats[0];
        diamond -= toolbuildmats[1];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;

        RectTransform rt = toolbarui.transform.Find("InventoryImage").gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 110);

        toolbarui.transform.Find("AxeV1").gameObject.transform.position += new Vector3 (-4, 0, 0);
        toolbarui.transform.Find("ScytheV1").gameObject.transform.position += new Vector3 (-4.5f, 0, 0);
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
        log -= firebuildmats[0];
        iron -= firebuildmats[1];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Cook()
    {
        busy = true;
        tools.transform.Find("Axe").gameObject.SetActive(false);
        tools.transform.Find("Scythe").gameObject.SetActive(false);
        tools.transform.Find("Pickaxe").gameObject.SetActive(false);
        tools.transform.Find("Chickenleg").gameObject.SetActive(true);
        animator.SetBool("eating", true);
        yield return new WaitForSeconds(2);
        Health = MaxHealth;
        Hunger = MaxHunger;
        Thirst = MaxThirst;
        log -= cookmats[0];
        apple -= cookmats[1];
        meat -= cookmats[2];
        water -= cookmats[3];
        ManualUpdateAllText();
        animator.SetBool("eating", false);
        tools.transform.Find("Chickenleg").gameObject.SetActive(false);
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
        log -= rocketbuildmats[0];
        iron -= rocketbuildmats[1];
        gold -= rocketbuildmats[2];
        diamond -= rocketbuildmats[3];
        ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator Wave()
    {
        direction = new Vector3(-1, 0, -3);
        animator.SetBool("waving", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("waving", false);
    }

    IEnumerator GetInRocket()
    {
        moving = true;
        busy = false;
        target = new Vector3(161.3f, 1.7f, 18.9f);
        yield return new WaitForSeconds(0.2f);
        busy = true;
        moving = false;
    }

    IEnumerator RocketUp()
    {
        rocket.transform.Find("BigExplosionEffect").gameObject.SetActive(true);
        rocket.transform.Find("SmokeEffect").gameObject.SetActive(true);
        for (int i = 1; i <= 200; i++)
        {
            rocket.transform.Find("Rocket").Translate(0, -10.1f*Time.deltaTime, 0);
            yield return null;
        }
    }

    IEnumerator LaunchRocket()
    {
        busy = true;
        yield return StartCoroutine(Wave());
        yield return StartCoroutine(GetInRocket());
        controller.transform.position = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(RocketUp());
        EndEpisode();
    }

    IEnumerator WaitForMove(IEnumerator Action)
    {
        while(moving)
        {
            yield return null;
        }
        StartCoroutine(Action);
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
        iron_t.text = "x" + iron;
        gold_t.text = "x" + gold;
        diamond_t.text = "x" + diamond;
    }

    public override void OnActionReceived(ActionBuffers vetcaction)
    {
        if (!moving && !busy)
        {
            if (vetcaction.DiscreteActions[0] == 0)
            {
                needtomove(treeslocation);
                StartCoroutine(WaitForMove(Log()));
                AddReward(0.001f);
            }
            if (vetcaction.DiscreteActions[0] == 1)
            {
                needtomove(farmlocation);
                StartCoroutine(WaitForMove(Gatherfood()));
                AddReward(0.0009f);
            }
            if (vetcaction.DiscreteActions[0] == 2)
            {
                needtomove(poollocation);
                StartCoroutine(WaitForMove(CollectWater()));
                AddReward(0.0008f);
            }
            if (vetcaction.DiscreteActions[0] == 3)
            {
                needtomove(rockslocation);
                StartCoroutine(WaitForMove(Mine()));
                AddReward(0.001f);
            }
            if (vetcaction.DiscreteActions[0] == 4)
            {
                if (!campfirebuilt && log >= firebuildmats[0] && iron >= firebuildmats[1])
                {
                    needtomove(firelocation);
                    StartCoroutine(WaitForMove(BuildFire()));
                    AddReward(0.9f);
                }
                else if(campfirebuilt && Hunger<25 && log >= cookmats[0] && apple >= cookmats[1] && meat >= cookmats[2] && water >= cookmats[3])
                {
                    needtomove(firelocation);
                    StartCoroutine(WaitForMove(Cook()));
                    AddReward(0.1f);
                }
            }
            if (vetcaction.DiscreteActions[0] == 5)
            {
                if (!benchbuilt && log >= benchbuildmats[0] && iron >= benchbuildmats[1] && gold >= benchbuildmats[2] && diamond >= benchbuildmats[3])
                {
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildBench()));
                    AddReward(0.85f);
                }
                else if (benchbuilt && !axebuilt && log >= toolbuildmats[0] && iron >= toolbuildmats[1])
                {  
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildAxe()));  
                    AddReward(0.85f);
                }
                else if (benchbuilt && axebuilt & !scythebuilt && log >= toolbuildmats[0] && gold >= toolbuildmats[1])
                {
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildScythe()));  
                    AddReward(0.85f);
                }
                else if (benchbuilt && axebuilt & scythebuilt && !pickaxebuilt && log >= toolbuildmats[0] && diamond >= toolbuildmats[1])
                {
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildPickaxe())); 
                    AddReward(0.85f);
                }
            }
            if (vetcaction.DiscreteActions[0] == 6)
            {
                if (!rocketbuilt && log >= rocketbuildmats[0] && iron >= rocketbuildmats[1] && gold >= rocketbuildmats[2] && diamond >= rocketbuildmats[3])
                {
                    needtomove(rocketlocation);
                    StartCoroutine(WaitForMove(BuildRocket()));
                    AddReward(1);
                }
                else if (rocketbuilt && log >= rocketlaunchmats[0] && apple >= rocketlaunchmats[1] && meat >= rocketlaunchmats[2] && oil >= rocketlaunchmats[3] && iron >= rocketlaunchmats[4] && gold >= rocketlaunchmats[5] && diamond >= rocketlaunchmats[6])
                {
                    //maybe also add water later
                    log -= rocketlaunchmats[0];
                    apple -= rocketlaunchmats[1];
                    meat -= rocketlaunchmats[2];
                    oil -= rocketlaunchmats[3];
                    iron -= rocketlaunchmats[4];
                    gold -= rocketlaunchmats[5];
                    diamond -= rocketlaunchmats[6];
                    ManualUpdateAllText();
                    AddReward(1);
                    AddReward(time.Day * -0.5f); //penality based on number of days taken to finish rocket
                    needtomove(rocketlocation);
                    StartCoroutine(WaitForMove(LaunchRocket()));
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsout)
    {
        ActionSegment<int> discreteactions = actionsout.DiscreteActions;
        if (Input.GetKey(KeyCode.Q))
        {
            discreteactions[0] = 0;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteactions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            discreteactions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            discreteactions[0] = 3;
        }
        else if (Input.GetKey(KeyCode.T))
        {
            discreteactions[0] = 5;
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            discreteactions[0] = 4;
        }
        else if (Input.GetKey(KeyCode.U))
        {
            discreteactions[0] = 6;
        }
        else
        {
            discreteactions[0] = -1; //else, invalid input
        }
    }

    public void needtomove(Transform targetlocation)
    {
        if (Vector3.Distance(targetlocation.transform.position, controller.transform.position) > 0.9)
        {
            target = targetlocation.position;
            moving = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(alive && !moving && !busy)
        {
            Debug.Log("Reward: " + GetCumulativeReward());
            RequestDecision();
        }
        if (moving && !busy)
        {
            if (Vector3.Distance(target, controller.transform.position) > 0.9)
            {
                input = new Vector2(target.x - controller.transform.position.x, target.z - controller.transform.position.z);
                input.Normalize();
                direction = new Vector3(input.x, 0, input.y);
                ApplyGravity();
                ApplyMovement();
            }
            else
            {
                input = new Vector2(0, 0);
                moving = false;
            }
        }
        ApplyRotation();
        animator.SetFloat("speed", input.sqrMagnitude);

        time = time.AddSeconds(Time.deltaTime * timemulti);
        UpdateLighting((time.Hour + time.Minute / 60f) / 24f);
        time_t.text = time.ToString("HH:mm");
        day_t.text = (time.Day - 1).ToString();

        HealthSlider.value = Health / MaxHealth;
        HungerSlider.value = Hunger / MaxHunger;
        ThirstSlider.value = Thirst / MaxThirst;
        if (Hunger > 0)
        {
            Hunger = Hunger - BarSpeedMulti * Time.deltaTime;
        }
        if (Thirst > 0)
        {
            Thirst = Thirst - BarSpeedMulti * Time.deltaTime;
        }
        if (Health > 0 && HungerSlider.value == 0)
        {
            Health = Health - BarSpeedMulti * Time.deltaTime;
        }
        if (Health > 0 && ThirstSlider.value == 0)
        {
            Health = Health - BarSpeedMulti * Time.deltaTime;
        }
        if (Health < 0 && alive)
        {
            alive = false;
            AddReward(-5);
            StopAllCoroutines();
            StartCoroutine(Die());
        }
        if (!rocketbuilt && log >= rocketbuildmats[0] && iron >= rocketbuildmats[1] && gold >= rocketbuildmats[2] && diamond >= rocketbuildmats[3])
        {
            AddReward(-0.001f);
        }
        if(rocketbuilt && log >= rocketlaunchmats[0] && apple >= rocketlaunchmats[1] && meat >= rocketlaunchmats[2] && oil >= rocketlaunchmats[3] && iron >= rocketlaunchmats[4] && gold >= rocketlaunchmats[5] && diamond >= rocketlaunchmats[6])
        {
            AddReward(-0.001f);
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
        if (controller.isGrounded && velocity < 0)
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
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnsmoothvelocity, turnsmoothtime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void ApplyMovement()
    {
        controller.Move(direction * speed * Time.deltaTime);
    }
}
    