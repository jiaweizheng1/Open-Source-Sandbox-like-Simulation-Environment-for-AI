using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class survival : MonoBehaviour
{   
    [Header("Player Health")]
    public float MaxHealth = 100f;
    public float Health = 0f;
    public Slider HealthSlider;
    [Header("Player Hunger")]
    public float MaxHunger = 100f;
    public float Hunger = 0f;
    public Slider HungerSlider;
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Hunger = Hunger - 2* Time.deltaTime;
        HealthSlider.value = Health / MaxHealth;
        HungerSlider.value = Hunger / MaxHunger;
        if(HungerSlider.value == 0){
            Health = Health - 2* Time.deltaTime;
        }
    }

    public void TakeFallDamage(){
        Health = Health - 10;
    }
}
