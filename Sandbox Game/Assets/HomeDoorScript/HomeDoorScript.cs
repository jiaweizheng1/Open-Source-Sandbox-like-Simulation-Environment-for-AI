using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomeDoorScript : MonoBehaviour
{
    public bool InRange;
    public KeyCode InteractKey;
    public UnityEvent InteractAction;
    public Animator Animator;
    public Transform TeleportTarget;
    public Vector3 TPCoordinates;

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
                Animator.SetTrigger("Trigger");   
                StartCoroutine(Coroutine());
            }
        }
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(2);
        TeleportTarget.transform.position = TPCoordinates;
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
