using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Envrionment_UI : MonoBehaviour
{
    public GameObject startMenu, optionMenu;

    private TMP_InputField LogNum, AppleNum, MeatNum, OilNum, WaterNum, IronNum, GoldNum, DimondNum;
    public int log, apple, meat, oil, water, iron, gold, diamond;
    private float Health, Hunger, Thirst;

    private void Awake()
    {
        LogNum = transform.Find("Log/LogNum").GetComponent<TMP_InputField>();
        AppleNum = transform.Find("Apple/AppleNum").GetComponent<TMP_InputField>();
        MeatNum = transform.Find("Meat/MeatNum").GetComponent<TMP_InputField>();
        OilNum = transform.Find("Oil/OilNum").GetComponent<TMP_InputField>();
        WaterNum = transform.Find("Water/WaterNum").GetComponent<TMP_InputField>();
        IronNum = transform.Find("Iron/IronNum").GetComponent<TMP_InputField>();
        GoldNum = transform.Find("Gold/GoldNum").GetComponent<TMP_InputField>();
        DimondNum = transform.Find("Dimond/DimondNum").GetComponent<TMP_InputField>();

        Health = GameObject.Find ("Health/HealthValue").GetComponent<Slider>().value;
        Hunger = GameObject.Find ("Hunger/HungerValue").GetComponent<Slider>().value;
        Thirst = GameObject.Find ("Thirst/ThirstValue").GetComponent<Slider>().value;

        // ToolEnable = GameObject.Find("ToggleToolEnable").GetComponent<toggle_switch>().isOn;
        // GodModeEnable = GameObject.Find("ToggleGodMod").GetComponent<toggle_switch>().isOn;
        // RandomEnable = GameObject.Find("ToggleRandomness").GetComponent<toggle_switch>().isOn;
        // EnemyEnable = GameObject.Find("ToggleEnemy").GetComponent<toggle_switch>().isOn;
    }

    // Update is called once per frame
    private void Start()
    {
        Show();
    }

    public void Show()
    {
        LogNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        AppleNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        MeatNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        OilNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        WaterNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        IronNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        GoldNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        DimondNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
    }

    private char ValidateChar(string validCharacters, char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
        {
            // Valid
            return addedChar;
        } 
        else
        {
            return '\0';
        }
        
    }

    public void Confirm()
    {
        log = int.Parse(LogNum.text);
        apple = int.Parse(AppleNum.text);
        meat = int.Parse(MeatNum.text);
        oil = int.Parse(OilNum.text);
        water = int.Parse(WaterNum.text);
        iron = int.Parse(IronNum.text);
        gold = int.Parse(GoldNum.text);
        diamond = int.Parse(DimondNum.text);

        Health = GameObject.Find ("Health/HealthValue").GetComponent<Slider>().value;
        Hunger = GameObject.Find ("Hunger/HungerValue").GetComponent<Slider>().value;
        Thirst = GameObject.Find ("Thirst/ThirstValue").GetComponent<Slider>().value;

        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void Cancel()
    {
        LogNum.text = log.ToString();
        AppleNum.text = apple.ToString();
        MeatNum.text = meat.ToString();
        OilNum.text = oil.ToString();
        WaterNum.text =  water.ToString();
        IronNum.text = iron.ToString();
        GoldNum.text = gold.ToString();
        DimondNum.text = diamond.ToString();

        Health = GameObject.Find ("Health/HealthValue").GetComponent<Slider>().value = Health;
        Hunger = GameObject.Find ("Hunger/HungerValue").GetComponent<Slider>().value = Hunger;
        Thirst = GameObject.Find ("Thirst/ThirstValue").GetComponent<Slider>().value = Thirst;

        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }
}
