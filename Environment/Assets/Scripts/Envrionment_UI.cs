using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Envrionment_UI : MonoBehaviour
{
    private TMP_InputField LogNum, AppleNum, MeatNum, OilNum, WaterNum, IronNum, GoldNum, DimondNum;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, iron_t, gold_t, diamond_t;
    public GameObject startMenu;
    public GameObject optionMenu;
    public GameObject enemyMenu;
    public Slider HealthSlider, HungerSlider, ThirstSlider;
    private float Health, Hunger, Thirst;
    private float Health_saved, Hunger_saved, Thirst_saved;
    private bool ToolEnable, GodModeEnable, RandomEnable;
    private bool ToolEnable_saved, GodModeEnable_saved, RandomEnable_saved;
    // Start is called before the first frame update
    private void Start()
    {
        ToolEnable_saved = false;
        GodModeEnable_saved = false;
        RandomEnable_saved = false;

        Health_saved = 100;
        Hunger_saved = 100;
        Thirst_saved = 100;

        enemyMenu.SetActive(false);
    }

    private void Awake()
    {
        LogNum.text = log_t.text;
        AppleNum.text = apple_t.text;
        MeatNum.text = meat_t.text;
        OilNum.text = oil_t.text;
        WaterNum.text = water_t.text;
        IronNum.text = iron_t.text;
        GoldNum.text = gold_t.text;
        DimondNum.text = diamond_t.text;

        ToolEnable = ToolEnable_saved;
        GodModeEnable = GodModeEnable_saved;
        RandomEnable = RandomEnable_saved;

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
        Time.timeScale = 1f;
        optionMenu.SetActive(false);
        /*SceneManager.LoadScene("MainMenu");*/
        startMenu.SetActive(true);
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().EndEpisode();
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

        HealthSlider.value = Health;
        HungerSlider.value = Hunger;
        ThirstSlider.value = Thirst;

        Debug.Log("Log: " + LogNum.text);
        Debug.Log("Apple: " + AppleNum.text);
        Debug.Log("meat: " + MeatNum.text);
        Debug.Log("Oil: " + OilNum.text);
        Debug.Log("Water: " + WaterNum.text);
        Debug.Log("Iron: " + IronNum.text);
        Debug.Log("Gold: " + GoldNum.text);
        Debug.Log("Dimond: " + DimondNum.text);

        Debug.Log("Health: " + Health);
        Debug.Log("Hunger: " + Hunger);
        Debug.Log("Thirst: " + Thirst);

        Debug.Log("Tool enable: " + ToolEnable);
        Debug.Log("God Mode: " + GodModeEnable);
        Debug.Log("Random: " + RandomEnable);

        ToolEnable_saved = ToolEnable;
        GodModeEnable_saved = GodModeEnable;
        RandomEnable_saved = RandomEnable;

        Health_saved = Health;
        Hunger_saved = Hunger;
        Thirst_saved = Thirst;

        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void Cancel()
    {
        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void EnemyEdit()
    {
        enemyMenu.SetActive(true);
    }
}
