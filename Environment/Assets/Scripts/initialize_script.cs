using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initialize_script : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(target, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(target, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
