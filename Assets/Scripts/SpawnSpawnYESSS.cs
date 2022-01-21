using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpawnYESSS : MonoBehaviour
{
    Vector3 curpos = new Vector3(0f, 0f, 0f);
    public GameObject[] spawns;

    // Start is called before the first frame update
    void Start()
    {
        int spawnNum = 5;
        for (int i = 0; i < spawnNum; i++)
        {
            int rand = Random.Range(0, spawns.Length);
            //GameObject buff1 = Instantiate(spawns[rand], curpos, new Qewuaternion());
            //curpos = new Vector3(0f, 50f * (i+1), 0f);
            GameObject buff1 = Instantiate(spawns[rand], new Vector3(0f, 2f * (i + 1), 0f), new Quaternion());

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}