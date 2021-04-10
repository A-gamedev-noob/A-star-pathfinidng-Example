using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public Transform[] locations;
    public GameObject[] enemies;
    public float rate = 1f;
    float time;

    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(time>=rate)
        {
            Spawn();
            time = 0;
        }
        time+=Time.deltaTime;
    }

    void Spawn()
    {
        transform.position = locations[Random.Range(0,locations.Length)].position;
        GameObject enemy =  Instantiate(
            enemies[Random.Range(0,enemies.Length)],
            transform.position,
            Quaternion.identity
        );

    }

}
