using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropPickup : MonoBehaviour
{
    private bool canCollect = false;
    private GameObject target;
    // Start is called before the first frame update
    private void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canCollect)
            {
                Destroy(target);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            canCollect = true;
            target = gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            canCollect = false;
            target = null;
        }
    }
}
