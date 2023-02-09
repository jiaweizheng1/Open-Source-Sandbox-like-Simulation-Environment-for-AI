using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_generate_seed : MonoBehaviour
{
    public GameObject crop_seed;
    public GameObject lt;
    public GameObject ld;
    public GameObject rt;

    public int num_crop_seed;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < num_crop_seed; i++)
        {
            transform.position = new Vector2(Random.Range(Mathf.Round(lt.transform.position.x * 1000.0f) * 0.001f,
                                                      Mathf.Round(rt.transform.position.x * 1000.0f) * 0.001f),
                                        Random.Range(Mathf.Round(lt.transform.position.y * 1000.0f) * 0.001f,
                                                     Mathf.Round(ld.transform.position.y * 1000.0f) * 0.001f));

            GameObject clone = Instantiate(crop_seed, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
