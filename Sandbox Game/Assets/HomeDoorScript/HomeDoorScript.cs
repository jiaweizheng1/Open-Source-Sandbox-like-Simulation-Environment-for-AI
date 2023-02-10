using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomeDoorScript : MonoBehaviour
{
    public bool InRange;
    public KeyCode InteractKey;
    public UnityEvent InteractAction;

    public Animator animator;

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
                animator.SetTrigger("Trigger");   
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            InRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            InRange = false;
        }
    }
}
