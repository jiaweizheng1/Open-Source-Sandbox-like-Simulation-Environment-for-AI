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
    public TMP_Text time_t;
    public TMP_Text day_t;

    private bool benchbuilt, campfirebuilt, rocketbuilt;
    public GameObject bench, fire, rocket;
    public GameObject benchui, fireui, rocketui;
    private int[] benchbuildmats = { 3, 1, 1, 1 };
    private int[] firebuildmats = { 2, 1 };
    private int[] cookmats = { 1, 1, 1, 1 };
    private int[] rocketbuildmats = { 10, 10, 7, 10 };
    private int[] rocketlaunchmats = { 1, 5, 5, 10, 1, 1, 1 };

    public GameObject toolSelector;
    private int[] toolChoose = { 350, 400, 450, 500, 550, 600 };
    private int log_tool, apple_tool, meat_tool, oil_tool, water_tool, copper_tool, gold_tool, iron_tool;

    public override void OnEpisodeBegin()
    {
        //initially, stay at current place(which is itself)
        transform.position = new Vector3(150, 1.36f, 25);
        target = transform.position;
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

        rocket.transform.Find("Rocket").position = new Vector3(160.8f, 4.42f, 19.3f);
        rocket.transform.Find("SmokeEffect").position = new Vector3(160.8f, 1.75f, 19.3f);
        rocket.transform.Find("BigExplosionEffect").gameObject.SetActive(false);
        rocket.transform.Find("SmokeEffect").gameObject.SetActive(false);

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

        log_tool = 0;
        apple_tool = 0;
        meat_tool = 0;
        oil_tool = 0;
        water_tool = 0;
        copper_tool = 0;
        gold_tool = 0;
        iron_tool = 0;
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
        Debug.Log(log_tool);
        log += 1 + log_tool;
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
        if (whichfood == 0)
        {
            apple += 1 + apple_tool;
        }
        else
        {
            meat += 1 + meat_tool;
        }
        int chanceoil = Random.Range(0, 10);
        if (chanceoil <= 2)
        {
            oil += 1 + oil_tool;
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
        water += 1 + water_tool;
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
        if (whichmineral == 0)
        {
            copper += 1 + copper_tool;
        }
        else if (whichmineral == 1)
        {
            gold += 1 + gold_tool;
        }
        else
        {
            iron += 1 + iron_tool;
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
        log -= benchbuildmats[0];
        copper -= benchbuildmats[1];
        gold -= benchbuildmats[2];
        iron -= benchbuildmats[3];
        ManualUpdateAllText();
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
        log -= firebuildmats[0];
        iron -= firebuildmats[1];
        ManualUpdateAllText();
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
        log -= cookmats[0];
        apple -= cookmats[1];
        meat -= cookmats[2];
        water -= cookmats[3];
        ManualUpdateAllText();
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
        log -= rocketbuildmats[0];
        copper -= rocketbuildmats[1];
        gold -= rocketbuildmats[2];
        iron -= rocketbuildmats[3];
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
            rocket.transform.Find("Rocket").Translate(0, -0.2f, 0);
            rocket.transform.Find("SmokeEffect").Translate(0, 0.2f, 0);
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
        if (!moving && !busy)
        {
            if (vetcaction.DiscreteActions[0] == 0)
            {
                if (needtomove(treeslocation))
                {
                }
                else
                {
                    StartCoroutine(Log());
                }
            }
            if (vetcaction.DiscreteActions[0] == 1)
            {
                if (needtomove(farmlocation))
                {
                }
                else
                {
                    StartCoroutine(Gatherfood());
                }
            }
            if (vetcaction.DiscreteActions[0] == 2)
            {
                if (needtomove(poollocation))
                {
                }
                else
                {
                    StartCoroutine(CollectWater());
                }
            }
            if (vetcaction.DiscreteActions[0] == 3)
            {
                if (needtomove(rockslocation))
                {
                }
                else
                {
                    StartCoroutine(Mine());
                }
            }
            if (vetcaction.DiscreteActions[0] == 4)
            {
                if (needtomove(benchlocation))
                {
                }
                else if (!benchbuilt && log >= benchbuildmats[0] && copper >= benchbuildmats[1] && gold >= benchbuildmats[2] && iron >= benchbuildmats[3])
                {
                    StartCoroutine(BuildBench());
                }
            }
            if (vetcaction.DiscreteActions[0] == 5)
            {
                if (needtomove(firelocation))
                {
                }
                else if (!campfirebuilt && log >= firebuildmats[0] && iron >= firebuildmats[1])
                {
                    StartCoroutine(BuildFire());
                }
                else if (campfirebuilt && log >= cookmats[0] && apple >= cookmats[1] && meat >= cookmats[2] && water >= cookmats[3])
                {
                    StartCoroutine(Cook());
                }
            }
            if (vetcaction.DiscreteActions[0] == 6)
            {
                if (needtomove(rocketlocation))
                {
                }
                else if (!rocketbuilt)
                //  && log >= rocketbuildmats[0] && copper >= rocketbuildmats[1] && gold >= rocketbuildmats[2] && iron >= rocketbuildmats[3])
                {
                    StartCoroutine(BuildRocket());

                }
                else if (rocketbuilt)
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
            discreteactions[0] = 4;
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            discreteactions[0] = 5;
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

    public bool needtomove(Transform targetlocation)
    {
        if (Vector3.Distance(targetlocation.transform.position, controller.transform.position) > 0.9)
        {
            target = targetlocation.position;
            moving = true;
            return true;
        }
        {
            return false;
        }
    }

    public void toolAction()
    {
        if (toolSelector.activeInHierarchy == true)
        {
            float toolXposition = toolSelector.GetComponent<RectTransform>().anchoredPosition.x;
            if (toolXposition >= 340 && toolXposition <= 360)
            {
                log_tool = 1;
            }
            else
            {
                log_tool = 0;
            }
        }
    }

    public void synchAllText()
    {
        log = int.Parse(log_t.text.Split('x')[1]);
        apple = int.Parse(apple_t.text.Split('x')[1]);
        meat = int.Parse(meat_t.text.Split('x')[1]);
        oil = int.Parse(oil_t.text.Split('x')[1]);
        water = int.Parse(water_t.text.Split('x')[1]);
        copper = int.Parse(copper_t.text.Split('x')[1]);
        gold = int.Parse(gold_t.text.Split('x')[1]);
        iron = int.Parse(iron_t.text.Split('x')[1]);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Health < 0)
        {
            StartCoroutine(Die());
        }

        // if(setrocketfree)
        // {
        //     rocket.transform.Find("Rocket").Translate(0, -0.2f, 0);
        //     rocket.transform.Find("SmokeEffect").Translate(0, 0.2f, 0);
        // }
        toolAction();
        synchAllText();
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
    
