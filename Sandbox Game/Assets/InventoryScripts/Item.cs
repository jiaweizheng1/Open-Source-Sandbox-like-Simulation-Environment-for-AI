using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType{
        Seed,
        Tool,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite(){
        switch(itemType){
            default:
            case ItemType.Seed:     return ItemAssets.Instance.seedSprite;


        }
    }
}
