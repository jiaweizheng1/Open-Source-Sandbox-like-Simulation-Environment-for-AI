using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheatInitialize : MonoBehaviour
{
    public bool activate;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(activate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
