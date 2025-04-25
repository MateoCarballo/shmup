using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player1;
    public Enemy ufo;

    private Boolean enemyAlive = false;
    private Boolean isSpawning = false;

    void Start()
    {
        //Aqui le meto el el vector 3 zero porque la posicion la sabe cada objeto. En su propio metodo start se setea la posicion
        Player playerInstance = Instantiate(player1,Vector3.zero, Quaternion.identity);
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
