using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Enemy ufo;

    private Boolean enemyAlive = false;
    private Boolean isSpawning = false;

    void Start()
    {
        //TODO la idea es que el objeto que tiene los puntos de spawn cree todos los enemigos y los active y desactive segun condiciones
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        isSpawning = true;
        yield return new WaitForSeconds(10f);

        if (FindFirstObjectByType<Enemy>() == null)
        {
            Instantiate(ufo, new Vector3(0, 4, 0), Quaternion.identity);
        }
        isSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindFirstObjectByType<Enemy>() == null)
        {
            StartCoroutine(SpawnEnemy());
        }

    }
}
