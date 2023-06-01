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
    private TMP_InputField LogNum, AppleNum, MeatNum, OilNum, WaterNum, IronNum, GoldNum, DimondNum;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, iron_t, gold_t, diamond_t;
    public GameObject startMenu, optionMenu;
    public Slider HealthSlider, HungerSlider, ThirstSlider;
    private float Health, Hunger, Thirst;
    private float Health_saved, Hunger_saved, Thirst_saved;
    private bool ToolEnable, GodModeEnable, RandomEnable, EnemyEnable;
    private bool ToolEnable_saved, GodModeEnable_saved, RandomEnable_saved, EnemyEnable_saved;
    // Start is called before the first frame update
    private void Start()
    {
        ToolEnable_saved = false;
        GodModeEnable_saved = false;
        RandomEnable_saved = false;

        Health_saved = 100;
        Hunger_saved = 100;
        Thirst_saved = 100;
    }

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

        LogNum.text = log_t.text.Split('x')[1];
        AppleNum.text = apple_t.text.Split('x')[1];
        MeatNum.text = meat_t.text.Split('x')[1];
        OilNum.text = oil_t.text.Split('x')[1];
        WaterNum.text = water_t.text.Split('x')[1];
        IronNum.text = iron_t.text.Split('x')[1];
        GoldNum.text = gold_t.text.Split('x')[1];
        DimondNum.text = diamond_t.text.Split('x')[1];

        GameObject.Find("ToggleToolEnable").GetComponent<toggle_switch>().isOn = ToolEnable_saved;
        GameObject.Find("ToggleGodMod").GetComponent<toggle_switch>().isOn = GodModeEnable_saved;
        GameObject.Find("ToggleRandomness").GetComponent<toggle_switch>().isOn = RandomEnable_saved;
        GameObject.Find("ToggleEnemy").GetComponent<toggle_switch>().isOn = EnemyEnable_saved;

        Health = Health_saved;
        Hunger = Hunger_saved;
        Thirst = Thirst_saved;
    }

    // Update is called once per frame
    private void Update()
    {
        Show();
        SliderUpdate();
        ToggleUpdate();
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
    
    public void SliderUpdate()
    {
        Health = HealthSlider.value;
        Hunger = HungerSlider.value;
        Thirst = ThirstSlider.value;
    }

    public void ToggleUpdate()
    {
        ToolEnable = GameObject.Find("ToggleToolEnable").GetComponent<toggle_switch>().isOn;
        GodModeEnable = GameObject.Find("ToggleGodMod").GetComponent<toggle_switch>().isOn;
        RandomEnable = GameObject.Find("ToggleRandomness").GetComponent<toggle_switch>().isOn;
        EnemyEnable = GameObject.Find("ToggleEnemy").GetComponent<toggle_switch>().isOn;
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

    public void BackToMenu()
    {
        optionMenu.SetActive(false);
        /*SceneManager.LoadScene("MainMenu");*/
        startMenu.SetActive(true);
    }

    public void Confirm()
    {
        log_t.text = "x" + LogNum.text;
        apple_t.text = "x" + AppleNum.text;
        meat_t.text = "x" + MeatNum.text;
        oil_t.text = "x" + OilNum.text;
        water_t.text = "x" + WaterNum.text;
        iron_t.text = "x" + IronNum.text;
        gold_t.text = "x" + GoldNum.text;
        diamond_t.text = "x" + DimondNum.text;

        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().StartUpInventory();

        HealthSlider.value = Health;
        HungerSlider.value = Hunger;
        ThirstSlider.value = Thirst;

        ToolEnable_saved = ToolEnable;
        GodModeEnable_saved = GodModeEnable;
        RandomEnable_saved = RandomEnable;
        EnemyEnable_saved = EnemyEnable;

        Health_saved = Health;
        Hunger_saved = Hunger;
        Thirst_saved = Thirst;

        Debug.Log("Log: " + log_t.text);
        Debug.Log("Apple: " + apple_t.text);
        Debug.Log("meat: " + meat_t.text);
        Debug.Log("Oil: " + oil_t.text);
        Debug.Log("Water: " + water_t.text);
        Debug.Log("Iron: " + iron_t.text);
        Debug.Log("Gold: " + gold_t.text);
        Debug.Log("Dimond: " + diamond_t.text);

        Debug.Log("Health: " + Health);
        Debug.Log("Hunger: " + Hunger);
        Debug.Log("Thirst: " + Thirst);

        Debug.Log("Tool enable: " + ToolEnable_saved);
        Debug.Log("God Mode: " + GodModeEnable_saved);
        Debug.Log("Random: " + RandomEnable_saved); 
        Debug.Log("Enemy: " + EnemyEnable_saved);

        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void Cancel()
    {
        optionMenu.SetActive(false);
        startMenu.SetActive(true);

        Debug.Log("Tool enable: " + ToolEnable_saved);
        Debug.Log("God Mode: " + GodModeEnable_saved);
        Debug.Log("Random: " + RandomEnable_saved); 
        Debug.Log("Enemy: " + EnemyEnable_saved);
    }
}
