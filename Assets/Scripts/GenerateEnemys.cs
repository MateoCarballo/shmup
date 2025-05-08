using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemys : MonoBehaviour
{

    public GameObject ufoPrefab;
    public Transform spawnPoint;
    public Transform targetPosition;


    private List<GameObject> enemys = new List<GameObject>();
    private int currentEnemyIndex = 0;

    void Start()
    {
        //Genero todos los enemigos fuera de la pantalla cuando creamos la escena
        for (int i = 0; i < 5; i++)
        {
            GameObject enemy = Instantiate(ufoPrefab, spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().SetTarget(targetPosition.position); // ← nuevo
            enemy.SetActive(false); // Opcional: empieza oculto
            enemys.Add(enemy);
        }
        ActivateNextEnemy();
    }

    void Update()
    {
        // Si el indice de enemigo actual es menor al total de enemigos creados
        if (currentEnemyIndex < enemys.Count)
        {
            //Si mi enemigo actual es igual a nulo activo al siguiente
            if (currentEnemyIndex == 0 || enemys[currentEnemyIndex-1]==null) ActivateNextEnemy();
            
        }
    }
    void ActivateNextEnemy()
    {
        if (currentEnemyIndex >= enemys.Count) return;

        GameObject nextEnemy = enemys[currentEnemyIndex];

        // Seguridad por si fue destruido antes de activar
        if (nextEnemy != null)
        {
            nextEnemy.SetActive(true);
        }

        currentEnemyIndex++;
    }

}
