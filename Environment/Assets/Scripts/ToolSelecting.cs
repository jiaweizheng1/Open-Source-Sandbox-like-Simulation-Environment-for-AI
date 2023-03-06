using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolSelecting : MonoBehaviour
{
    public GameObject itemSelector;
    private float toolXposition, toolYposition, flag;
    private Vector2 Position;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, copper_t, gold_t, iron_t;
    private int log, apple, meat, oil, water, copper, gold, iron;
    /*private int nextUpdate = 1;*/

    // Start is called before the first frame update
    void Start()
    {
        Position = itemSelector.GetComponent<RectTransform>().anchoredPosition;
        toolXposition = Position.x;
        toolYposition = Position.y;
        flag = -1;
    }

    // Update is called once per frame
    void Update()
    {
        float flag_temp;
        flag_temp = itemSelecting();
        if (flag_temp > -1)
        {
            flag = flag_temp;
            itemSelector.SetActive(true);
            Position = new Vector2(toolXposition + (50 * flag), toolYposition);
            itemSelector.GetComponent<RectTransform>().anchoredPosition = Position;
            Debug.Log(log);
            ManualUpdateAllText();
        }
        else
        {
            synchAllText();
        }
        
    }

    public float itemSelecting()
    {
        float flag = -1;
        if (Input.GetKey(KeyCode.Alpha1) && log >= 1)
        {
            flag = 0;
            log--;
        }
        else if (Input.GetKey(KeyCode.Alpha2) && log >= 1)
        {
            flag = 1;
            log--;
        }
        else if (Input.GetKey(KeyCode.Alpha3) && log >= 1)
        {
            flag = 2;
            log--;
        }
        else if (Input.GetKey(KeyCode.Alpha4) && log >= 1)
        {
            flag = 3;
            log--;
        }
        else if (Input.GetKey(KeyCode.Alpha5) && log >= 1)
        {
            flag = 4;
            log--;
        }
        else if (Input.GetKey(KeyCode.Alpha6) && log >= 1)
        {
            flag = 5;
            log--;
        }
        else
        {
            flag = -1; //else, nothing pressed no need change
        }

        return flag;
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
}
