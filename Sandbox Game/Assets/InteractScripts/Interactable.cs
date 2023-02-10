using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool InRange;
    public KeyCode InteractKey;
    public UnityEvent InteractAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(InRange)
        {
            if(Input.GetKeyDown(InteractKey))
            {
                Debug.Log("Interaction Started");
                InteractAction.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            InRange = true;
            Debug.Log("Player Close");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            InRange = false;
            Debug.Log("Player Not Close");
        }
    }
}
