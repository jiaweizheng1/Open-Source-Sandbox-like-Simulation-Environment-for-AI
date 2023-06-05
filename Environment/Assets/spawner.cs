using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject spiderPrefab;
    void update(){
        if(Input.GetKey(KeyCode.L))
        {
            Debug.Log("This is a debug message");

            Vector3 randomSpawnPosition = new Vector3(150,5,50);
            Instantiate(spiderPrefab,randomSpawnPosition,Quaternion.identity);
        }
    }
}
