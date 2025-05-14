using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemys : MonoBehaviour
{
    public static GenerateEnemys GenerateEnemysInstance { get; private set; }

    public GameObject ufoPrefab;
    public Transform spawnPoint;
    public Transform targetPosition;

    private List<GameObject> enemyPool = new List<GameObject>();
    private float spawnDelay = 8f; // Tiempo entre aparición de enemigos
    private float lastSpawnTime;

    void Awake()
    {
        if (GenerateEnemysInstance == null)
        {
            GenerateEnemysInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePool(5); // Crear pool inicial
        TrySpawnEnemy();
    }

    void Update()
    {
        if (Time.time - lastSpawnTime > spawnDelay)
        {
            TrySpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    private void InitializePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewEnemy();
        }
    }

    private void CreateNewEnemy()
    {
        GameObject enemy = Instantiate(ufoPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().SetTarget(targetPosition.position);
        enemy.SetActive(false);
        enemyPool.Add(enemy);
    }

    private void TrySpawnEnemy()
    {
        // Buscar primer enemigo inactivo
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                ActivateEnemy(enemy);
                return;
            }
        }

        // Si todos están activos, crear uno nuevo
        CreateNewEnemy();
        ActivateEnemy(enemyPool[enemyPool.Count - 1]);
    }

    private void ActivateEnemy(GameObject enemy)
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.z = 0f; // o -1f si es necesario
        enemy.transform.position = spawnPos;

        enemy.GetComponent<Enemy>().SetTarget(targetPosition.position);
        enemy.SetActive(true);
    }

    public void OnEnemyDeactivated(Enemy enemy)
    {
        // Podemos añadir lógica adicional aquí si es necesario
    }
}