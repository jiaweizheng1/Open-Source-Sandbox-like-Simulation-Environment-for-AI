using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class EnemyParameter : MonoBehaviour
{
    public TextMeshProUGUI output;

    public void quite()
    {
        this.gameObject.SetActive(false);
    }

    public void save()
    {
        this.gameObject.SetActive(false);
    }
}
