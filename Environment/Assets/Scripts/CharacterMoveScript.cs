using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro; 
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random=UnityEngine.Random;
using System.IO;

public class CharacterMoveScript : Agent
{
    private bool Unity_ML_Agent_Mode = false; // 'true' for Unity ML Agent, 'false' for Python PPO AI
    public float speed;
    public Animator animator;
    public Animator animator_spider;
    public CharacterController controller;
    public Transform treeslocation, farmlocation, poollocation, rockslocation, benchlocation, firelocation, rocketlocation;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, iron_t, gold_t, diamond_t;
    private float[] inventory = {0, 0, 0, 0, 0, 0, 0, 0};
    private float reward;
    private bool alive, moving, busy, done;
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
    
    public GameObject spiderPrefab;
    public int spider_count = 0;
    Vector3 rotationAngles = new Vector3(0f, 180f, 0f);
    
    int nighttime_start_hour = 22;
    int nighttime_end_hour = 6;
    int random_nighttime_hour = 1;
    private GameObject spider;
    public int spider_health = 2;
    bool spider_spawned = false;
    public Transform Target;
    public Transform Escape;
    Coroutine myCoroutine;
    int debug_count = 0;
    private bool benchbuilt, campfirebuilt, rocketbuilt, axebuilt, scythebuilt, pickaxebuilt;
    public GameObject tools;
    public GameObject bench, fire, rocket, toolblueprints;
    public GameObject benchui, fireui, rocketui, toolui, toolbarui;
    private float[] benchbuildmats = {3, 0, 0, 0, 0, 1, 1, 1};
    private float[] firebuildmats = {2, 0, 0, 0, 0, 1, 0, 0};
    private float[] cookmats = {1, 1, 1, 0, 1, 0, 0, 0};
    private float[] rocketbuildmats = {10, 0, 0, 0, 0, 10, 10, 10};
    private float[] rocketlaunchmats = {1, 5, 5, 0, 10, 1, 1, 1};
    private float[] axebuildmats = {2, 0, 0, 0, 0, 3, 0, 0};
    private float[] scythebuildmats = {2, 0, 0, 0, 0, 0, 3, 0};
    private float[] pickaxebuildmats = {2, 0, 0, 0, 0, 0, 0, 3};
    NavMeshAgent agent;
    public float closeEnoughDistance = 3.0f;
    private float logReward, foodReward, waterReward, mineReward, campfireReward, recoverReward, benchReward, toolReward, rocketReward, launchReward, spiderReward;

    public GameObject environmentmenu, rewardmenu;
    private bool ToolEnable, GodModeEnable, RandomEnable, EnemyEnable;
    private int[] actions = {0, 1, 2, 3, 4, 5, 6, 7};

    void Start()
    {
        Time.timeScale = timescale;
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0);
        logReward = float.Parse(rewardmenu.transform.Find("Log/LogReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        foodReward = float.Parse(rewardmenu.transform.Find("Food/FoodReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        waterReward = float.Parse(rewardmenu.transform.Find("Water/WaterReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        mineReward = float.Parse(rewardmenu.transform.Find("Mine/MineReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        campfireReward = float.Parse(rewardmenu.transform.Find("Campfire/CampfireReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        recoverReward = float.Parse(rewardmenu.transform.Find("Recover/RecoverReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        benchReward = float.Parse(rewardmenu.transform.Find("Bench/BenchReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        toolReward = float.Parse(rewardmenu.transform.Find("Tool/ToolReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        rocketReward = float.Parse(rewardmenu.transform.Find("Rocket/RocketReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        launchReward = float.Parse(rewardmenu.transform.Find("Launch/LaunchReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        spiderReward = float.Parse(rewardmenu.transform.Find("Spider/SpiderReward").gameObject.transform.GetComponent<TMP_InputField>().text);
        Debug.Log(spiderReward);

        reward = 0;
        done = false;

        animator.SetBool("deadge", false);
        animator.SetBool("chopping", false);
        animator.SetBool("harvesting", false);
        animator.SetBool("mining", false);
        animator.SetBool("eating", false);
        animator.SetBool("waving", false);
        animator.SetFloat("speed", 0);

        ToolEnable = environmentmenu.transform.Find("ToggleToolEnable").GetComponent<toggle_switch>().isOn;
        GodModeEnable = environmentmenu.transform.Find("ToggleGodMod").GetComponent<toggle_switch>().isOn;
        RandomEnable = environmentmenu.transform.Find("ToggleRandomness").GetComponent<toggle_switch>().isOn;
        EnemyEnable = environmentmenu.transform.Find("ToggleEnemy").GetComponent<toggle_switch>().isOn;
        actions[0] = 0;
        actions[1] = 1;
        actions[2] = 2;
        actions[3] = 3;
        actions[4] = 4;
        actions[5] = 5;
        actions[6] = 6;
        // Fisher–Yates shuffle Algorithm
        if(RandomEnable)
        {
            Debug.Log("Fisher Algorithm");
            for (int i = actions.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                int temp = actions[i];
                actions[i] = actions[j];
                actions[j] = temp;
            }
        }

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

        toolbarui.transform.Find("InventoryImage").GetComponent<RectTransform>().sizeDelta = new Vector2(30, 110);
        toolbarui.transform.Find("Tools").localPosition = new Vector3(0, 0, 0);
        toolbarui.gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("AxeSelector").gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheSelector").gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeSelector").gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheV1").gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeV1").gameObject.SetActive(false);

        Health = environmentmenu.transform.Find ("Health/HealthValue").GetComponent<Slider>().value;
        Hunger = environmentmenu.transform.Find ("Hunger/HungerValue").GetComponent<Slider>().value;
        Thirst = environmentmenu.transform.Find ("Thirst/ThirstValue").GetComponent<Slider>().value;

        alive = true;
        moving = false;
        busy = false;

        inventory[0] = int.Parse(environmentmenu.transform.Find("Log/LogNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[1] = int.Parse(environmentmenu.transform.Find("Apple/AppleNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[2] = int.Parse(environmentmenu.transform.Find("Meat/MeatNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[3] = int.Parse(environmentmenu.transform.Find("Oil/OilNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[4] = int.Parse(environmentmenu.transform.Find("Water/WaterNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[5] = int.Parse(environmentmenu.transform.Find("Iron/IronNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[6] = int.Parse(environmentmenu.transform.Find("Gold/GoldNum").gameObject.transform.GetComponent<TMP_InputField>().text);
        inventory[7] = int.Parse(environmentmenu.transform.Find("Dimond/DimondNum").gameObject.transform.GetComponent<TMP_InputField>().text);
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

        sensor.AddObservation(inventory);
        
        sensor.AddObservation(Health);
        sensor.AddObservation(Hunger);
        sensor.AddObservation(Thirst);

        sensor.AddObservation(benchbuilt);
        sensor.AddObservation(campfirebuilt);
        sensor.AddObservation(rocketbuilt);
        sensor.AddObservation(axebuilt);
        sensor.AddObservation(scythebuilt);
        sensor.AddObservation(pickaxebuilt);

        /* observations commented for backward compatiblity with our previously Trained Unity ML model
        sensor.AddObservation(benchbuildmats);
        sensor.AddObservation(firebuildmats);
        sensor.AddObservation(cookmats);
        sensor.AddObservation(rocketbuildmats);
        sensor.AddObservation(rocketlaunchmats);
        sensor.AddObservation(axebuildmats);
        sensor.AddObservation(scythebuildmats);
        sensor.AddObservation(pickaxebuildmats);
        */
    }

    IEnumerator Log()
    {
        busy = true;
        if(axebuilt)
        {
            tools.transform.Find("Axe").gameObject.SetActive(true);
            tools.transform.Find("Scythe").gameObject.SetActive(false);
            tools.transform.Find("Pickaxe").gameObject.SetActive(false);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("AxeSelector").gameObject.SetActive(true);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheSelector").gameObject.SetActive(false);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeSelector").gameObject.SetActive(false);
            animator.SetBool("chopping", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if (axebuilt)
        {
            inventory[0] += 3;
        }
        else
        {
            inventory[0]++;
        }
        // ManualUpdateAllText();
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
            toolbarui.transform.Find("Tools").gameObject.transform.Find("AxeSelector").gameObject.SetActive(false);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheSelector").gameObject.SetActive(true);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeSelector").gameObject.SetActive(false);
            animator.SetBool("chopping", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if(scythebuilt)
        {
            inventory[1]++;
            inventory[2]++;
            inventory[3]++;
        }
        else
        {
            int whichfood = Random.Range(0, 2);
            if (whichfood == 0)
            {
                inventory[1]++;
            }
            else
            {
                inventory[2]++;
            }
            int chanceoil = Random.Range(0, 10);
            if (chanceoil <= 2)
            {
                inventory[3]++;
            }
        }
        // ManualUpdateAllText();
        animator.SetBool("chopping", false);
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator CollectWater()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        inventory[4]++;
        // ManualUpdateAllText();
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
            toolbarui.transform.Find("Tools").gameObject.transform.Find("AxeSelector").gameObject.SetActive(false);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheSelector").gameObject.SetActive(false);
            toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeSelector").gameObject.SetActive(true);
            animator.SetBool("mining", true);
        }
        else
        {
            animator.SetBool("harvesting", true);
        }
        yield return new WaitForSeconds(2);
        if(pickaxebuilt)
        {
            inventory[5]++;
            inventory[6]++;
            inventory[7]++;
        }
        else
        {
            int whichmineral = Random.Range(0, 2);
            if (whichmineral == 0)
            {
                inventory[5]++;
            }
            else
            {
                inventory[6]++;
            }
            int chancediamond = Random.Range(0, 10);
            if (chancediamond <= 2)
            {
                inventory[7]++;
            }
        }
        // ManualUpdateAllText();
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
        inventory[0] -= benchbuildmats[0];
        inventory[5] -= benchbuildmats[5];
        inventory[6] -= benchbuildmats[6];
        inventory[7] -= benchbuildmats[7];
        // ManualUpdateAllText();
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
        toolbarui.gameObject.SetActive(true);
        inventory[0] -= axebuildmats[0];
        inventory[5] -= axebuildmats[5];
        // ManualUpdateAllText();
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
        toolbarui.transform.Find("Tools").gameObject.transform.Find("ScytheV1").gameObject.SetActive(true);
        toolbarui.transform.Find("Tools").Translate(-5, 0, 0);
        toolbarui.transform.Find("InventoryImage").GetComponent<RectTransform>().sizeDelta = new Vector2(45, 110);
        inventory[0] -= scythebuildmats[0];
        inventory[6] -= scythebuildmats[6];
        // ManualUpdateAllText();
        animator.SetBool("harvesting", false);
        busy = false;
    }

    IEnumerator BuildPickaxe()
    {
        busy = true;
        animator.SetBool("harvesting", true);
        yield return new WaitForSeconds(2);
        pickaxebuilt=true;
        toolblueprints.transform.Find("ToolBlueprint").gameObject.transform.Find("PickAxe").gameObject.SetActive(false);
        toolui.transform.Find("UIBuildPickaxe").gameObject.SetActive(false);
        toolbarui.transform.Find("Tools").gameObject.transform.Find("PickAxeV1").gameObject.SetActive(true);
        toolbarui.transform.Find("InventoryImage").GetComponent<RectTransform>().sizeDelta = new Vector2(60, 110);
        toolbarui.transform.Find("Tools").Translate(-5, 0, 0);
        inventory[0] -= pickaxebuildmats[0];
        inventory[7] -= pickaxebuildmats[7];
        // ManualUpdateAllText();
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
        inventory[0] -= firebuildmats[0];
        inventory[5] -= firebuildmats[5];
        // ManualUpdateAllText();
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
        inventory[0] -= cookmats[0];
        inventory[1] -= cookmats[1];
        inventory[2] -= cookmats[2];
        inventory[4] -= cookmats[4];
        // ManualUpdateAllText();
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
        inventory[0] -= rocketbuildmats[0];
        inventory[5] -= rocketbuildmats[5];
        inventory[6] -= rocketbuildmats[6];
        inventory[7] -= rocketbuildmats[7];
        // ManualUpdateAllText();
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

    IEnumerator Attack()
    {
        direction = new Vector3(-1, 0, -3);
        animator.SetBool("mining", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("mining", false);
        spider_health --;

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
        if(Unity_ML_Agent_Mode)
        {
            EndEpisode();
        }
        done = true;
        busy = false;
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
        if(Unity_ML_Agent_Mode)
        {
            EndEpisode();
        }
        done = true;
        busy = false;
    }

    IEnumerator Destroy_camp()
    {
        // debug_count++;
        // Debug.Log(debug_count);
        if(spider!=null && campfirebuilt){
            animator_spider.SetBool("attack", true);
        }
        yield return new WaitForSeconds(10);

        if(spider!=null && campfirebuilt){

            fire.transform.Find("FireBlueprint").gameObject.SetActive(true);
            fire.transform.Find("Fire").gameObject.SetActive(false);
            fireui.transform.Find("UIBuild").gameObject.SetActive(true);
            fireui.transform.Find("UICook").gameObject.SetActive(false);
            campfirebuilt = false;
            animator_spider.SetBool("attack", false);
        }

    }
    public string myToString(float[] arr)
    {
        string s = "";
        s += "(";
        for(int i = 0; i < 8; i++)
        {
            s += arr[i].ToString();
            if(i != 7)
            {
                s += ", ";
            }
        }
        s += ")";
        return s;
    }
    public void ManualUpdateAllText()
    {
        log_t.text = "x" + inventory[0];
        apple_t.text = "x" + inventory[1];
        meat_t.text = "x" + inventory[2];
        oil_t.text = "x" + inventory[3];
        water_t.text = "x" + inventory[4];
        iron_t.text = "x" + inventory[5];
        gold_t.text = "x" + inventory[6];
        diamond_t.text = "x" + inventory[7];

        // string contents = "RobotLocation: " + controller.transform.position + Environment.NewLine;
        // contents += "TreesLocation: " + treeslocation.transform.position + Environment.NewLine;
        // contents += "FarmLocation: " + farmlocation.transform.position + Environment.NewLine;
        // contents += "PoolLocation: " + poollocation.transform.position + Environment.NewLine;
        // contents += "RocksLocation: " + rockslocation.transform.position + Environment.NewLine;
        // contents += "BenchLocation: " + benchlocation.transform.position + Environment.NewLine;
        // contents += "FireLocation: " + firelocation.transform.position + Environment.NewLine;
        // contents += "RocketLocation: " + rocketlocation.transform.position + Environment.NewLine;

        string contents = "Moving: " + moving + Environment.NewLine;
        contents += "Busy: " + busy + Environment.NewLine;

        contents += Environment.NewLine;

        contents += "Inventory: " + myToString(inventory) + Environment.NewLine;

        contents += "Health: " + Health + Environment.NewLine;
        contents += "Hunger: " + Hunger + Environment.NewLine;

        contents += "BenchBuilt: " + benchbuilt + Environment.NewLine;
        contents += "CampfireBuilt: " + campfirebuilt + Environment.NewLine;
        contents += "RocketBuilt: " + rocketbuilt + Environment.NewLine;
        contents += "AxeBuilt: " + axebuilt + Environment.NewLine;
        contents += "ScytheBuilt: " + scythebuilt + Environment.NewLine;
        contents += "PickaxehBuilt: " + pickaxebuilt + Environment.NewLine;
        if(spider_count == 0){
            contents += "Spider: " + "False" + Environment.NewLine;
        }
        else{
            contents += "Spider: " + "True" + Environment.NewLine;
        }
        contents += Environment.NewLine;

        contents += "Reward: " + reward + Environment.NewLine;

        contents += Environment.NewLine;

        contents += "Done: " + done + Environment.NewLine;



        File.WriteAllText(@"observations.txt", contents);
    }

    public override void OnActionReceived(ActionBuffers vetcaction)
    {
        if (alive && !moving && !busy)
        {
            if (vetcaction.DiscreteActions[0] == actions[0])
            {
                AddReward(logReward);
                reward = logReward;
                needtomove(treeslocation);
                StartCoroutine(WaitForMove(Log()));
            }
            if (vetcaction.DiscreteActions[0] == actions[1])
            {
                AddReward(foodReward);
                reward = foodReward;
                needtomove(farmlocation);
                StartCoroutine(WaitForMove(Gatherfood()));
            }
            if (vetcaction.DiscreteActions[0] == actions[2])
            {
                AddReward(waterReward);
                reward = waterReward;
                needtomove(poollocation);
                StartCoroutine(WaitForMove(CollectWater()));
            }
            if (vetcaction.DiscreteActions[0] == actions[3])
            {
                AddReward(mineReward);
                reward = mineReward;
                needtomove(rockslocation);
                StartCoroutine(WaitForMove(Mine()));
            }
            if (vetcaction.DiscreteActions[0] == actions[4])
            {
                if(spider_count == 1 && spider_health > 0 && spider != null){
                    AddReward(spiderReward);
                    reward = spiderReward;
                    Transform spiderlocation = spider.transform;
                    needtomove(spiderlocation);
                    StartCoroutine(WaitForMove(Attack()));
                }
                else if (!campfirebuilt && inventory[0] >= firebuildmats[0] && inventory[5] >= firebuildmats[5])
                {
                    AddReward(campfireReward);
                    reward = campfireReward;
                    needtomove(firelocation);
                    StartCoroutine(WaitForMove(BuildFire()));
                }
                else if(campfirebuilt && Hunger<25 && inventory[0] >= cookmats[0] && inventory[1] >= cookmats[1] && inventory[2] >= cookmats[2] && inventory[4] >= cookmats[4])
                {
                    AddReward(recoverReward);
                    reward = recoverReward;
                    needtomove(firelocation);
                    StartCoroutine(WaitForMove(Cook()));
                }
                else
                {
                    reward = 0;
                }
            }
            if (vetcaction.DiscreteActions[0] == actions[5])
            {
                if (ToolEnable && !benchbuilt && inventory[0] >= benchbuildmats[0] && inventory[5] >= benchbuildmats[5] && inventory[6] >= benchbuildmats[6] && inventory[7] >= benchbuildmats[7])
                {
                    AddReward(benchReward);
                    reward = benchReward;
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildBench()));
                }
                else if (ToolEnable && benchbuilt && !axebuilt && inventory[0] >= axebuildmats[0] && inventory[5] >= axebuildmats[5])
                {  
                    AddReward(toolReward);
                    reward = toolReward;
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildAxe()));  
                }
                else if (ToolEnable && benchbuilt && axebuilt & !scythebuilt && inventory[0] >= scythebuildmats[0] && inventory[6] >= scythebuildmats[6])
                {
                    AddReward(toolReward);
                    reward = toolReward;
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildScythe()));  
                }
                else if (ToolEnable && benchbuilt && axebuilt & scythebuilt && !pickaxebuilt && inventory[0] >= pickaxebuildmats[0] && inventory[7] >= pickaxebuildmats[7])
                {
                    AddReward(toolReward);
                    reward = toolReward;
                    needtomove(benchlocation);
                    StartCoroutine(WaitForMove(BuildPickaxe())); 
                }
                else
                {
                    reward = 0;
                }
            }
            if (vetcaction.DiscreteActions[0] == actions[6])
            {
                if (!rocketbuilt && inventory[0] >= rocketbuildmats[0] && inventory[5] >= rocketbuildmats[5] && inventory[6] >= rocketbuildmats[6] && inventory[7] >= rocketbuildmats[7])
                {
                    AddReward(rocketReward);
                    reward = rocketReward;
                    needtomove(rocketlocation);
                    StartCoroutine(WaitForMove(BuildRocket()));
                }
                else if (rocketbuilt && inventory[0] >= rocketlaunchmats[0] && inventory[1] >= rocketlaunchmats[1] && inventory[2] >= rocketlaunchmats[2] && inventory[3] >= rocketlaunchmats[3] && inventory[5] >= rocketlaunchmats[5] && inventory[6] >= rocketlaunchmats[6] && inventory[7] >= rocketlaunchmats[7])
                {
                    AddReward(launchReward);
                    reward = launchReward;
                    inventory[0] -= rocketlaunchmats[0];
                    inventory[1] -= rocketlaunchmats[1];
                    inventory[2] -= rocketlaunchmats[2];
                    inventory[3] -= rocketlaunchmats[3];
                    inventory[5] -= rocketlaunchmats[5];
                    inventory[6] -= rocketlaunchmats[6];
                    inventory[7] -= rocketlaunchmats[7];
                    // ManualUpdateAllText();
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
        int current_hour = time.Hour;
        int random_offset = Random.Range(0, 7);
        if(spider == null){
            spider_count = 0;
            spider_health = 0;
        }
        if (myCoroutine != null && (!campfirebuilt || spider_count == 0)) {
            StopCoroutine(myCoroutine);
            myCoroutine = null;
            Destroy(spider);
        }
        Quaternion rotation = Quaternion.Euler(rotationAngles);
        if(current_hour == random_nighttime_hour)
        {
            if(spider_count == 0 && campfirebuilt && !spider_spawned && EnemyEnable){
                spider_spawned = true;
                spider_health = 5;
                Vector3 randomSpawnPosition = new Vector3(Random.Range(120,170),2,Random.Range(50,60));
                spider = Instantiate(spiderPrefab,randomSpawnPosition,rotation);
                animator_spider = spider.GetComponent<Animator>();
                agent = spider.GetComponent<NavMeshAgent>();
                if (agent == null || !agent.enabled) {
                    Debug.LogError("No enabled NavMeshAgent on spider");
                } else {
                    agent.SetDestination(Target.position);
                    animator_spider.SetBool("walk",true);
                }
                random_nighttime_hour = (nighttime_start_hour + random_offset) % 24;
                spider_count ++;
            }
        }
        if(current_hour == nighttime_end_hour){
            spider_spawned = false;
        }
        if(agent!= null && spider_count != 0 && campfirebuilt)
        {
            closeEnoughDistance = 3.0f;
            if (!agent.pathPending && agent.remainingDistance <= closeEnoughDistance && agent.remainingDistance > 0)
            {
                // Perform your action here
                animator_spider.SetBool("walk",false);
                myCoroutine = StartCoroutine(WaitForMove(Destroy_camp()));
                
            }
        }
        if(spider_health == 0 && spider!= null){
            spider_count = 0;
            if (myCoroutine != null) {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            Destroy(spider);
        }
        if(!campfirebuilt){
            if (myCoroutine != null) {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
        }
        if(alive && !moving && !busy)
        {
            Debug.Log("Reward: " + reward);
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
        if (!GodModeEnable & Hunger > 0)
        {
            Hunger = Hunger - BarSpeedMulti * Time.deltaTime;
        }
        if (!GodModeEnable & Thirst > 0)
        {
            Thirst = Thirst - BarSpeedMulti * Time.deltaTime;
        }
        if (!GodModeEnable & Health > 0 && HungerSlider.value == 0)
        {
            Health = Health - BarSpeedMulti * Time.deltaTime;
        }
        if (!GodModeEnable & Health > 0 && ThirstSlider.value == 0)
        {
            Health = Health - BarSpeedMulti * Time.deltaTime;
        }
        if (!GodModeEnable & Health < 0 && alive)
        {
            alive = false;
            AddReward(-5);
            reward = -5;
            // ManualUpdateAllText();
            StopAllCoroutines();
            StartCoroutine(Die());
        }

        ManualUpdateAllText();
        if (Input.GetKey(KeyCode.I))
        {
            if(spider != null){
                Destroy(spider);
                spider_count = 0;
                spider_spawned = false;
            }
            EndEpisode();

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

    public void StopAllCor()
    {
        StopAllCoroutines();
    }
}