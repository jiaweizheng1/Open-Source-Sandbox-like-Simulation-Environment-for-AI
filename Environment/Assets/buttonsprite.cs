using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonsprite : MonoBehaviour
{
    [SerializeField] private SpriteState spriteState;
    [SerializeField] private Button button;
    public void SpriteChange()
    {
        button.spriteState = spriteState;
    }
}
