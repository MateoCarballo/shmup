using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateEnemys : MonoBehaviour
{
    public static GenerateEnemys Instance { get; private set; }

    [Header("Enemy Generation Settings")]
    public GameObject ufoPrefab;
    public Transform spawnPoint;
    public Transform targetPosition;
    [Tooltip("Tiempo entre aparición de enemigos")]
    public float spawnDelay = 8f;
    [Tooltip("Número total de enemigos a generar en este nivel")]
    public int totalEnemiesToSpawn = 10;

    [Header("Level Transition Settings")]
    [Tooltip("Tiempo de espera después de completar el nivel")]
    public float levelEndDelay = 3f;
    [Tooltip("Referencia al jugador")]
    public Player playerShip;

    private List<GameObject> enemyPool = new List<GameObject>();
    private float lastSpawnTime;
    private int enemiesSpawned = 0;
    private int enemiesActive = 0;
    private bool levelCompleted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePool(5);
        TrySpawnEnemy();
    }

    void Update()
    {
        if (!levelCompleted && enemiesSpawned < totalEnemiesToSpawn && Time.time - lastSpawnTime > spawnDelay)
        {
            TrySpawnEnemy();
            lastSpawnTime = Time.time;
        }

        // Comprobar si se completó el nivel
        if (!levelCompleted && enemiesSpawned >= totalEnemiesToSpawn && enemiesActive <= 0)
        {
            StartCoroutine(CompleteLevel());
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
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.SetTarget(targetPosition.position);

        // Registrar eventos
        enemyComponent.OnEnemyDestroyed += HandleEnemyDestroyed;
        enemyComponent.OnEnemyDeactivated += HandleEnemyDeactivated;

        enemy.SetActive(false);
        enemyPool.Add(enemy);
    }

    private void TrySpawnEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy && enemiesSpawned < totalEnemiesToSpawn)
            {
                ActivateEnemy(enemy);
                enemiesSpawned++;
                enemiesActive++;
                return;
            }
        }

        // Si no hay enemigos disponibles y aún podemos generar más
        if (enemiesSpawned < totalEnemiesToSpawn)
        {
            CreateNewEnemy();
            ActivateEnemy(enemyPool[enemyPool.Count - 1]);
            enemiesSpawned++;
            enemiesActive++;
        }
    }

    private void ActivateEnemy(GameObject enemy)
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.z = 0f;
        enemy.transform.position = spawnPos;
        enemy.GetComponent<Enemy>().SetTarget(targetPosition.position);
        enemy.SetActive(true);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        enemiesActive--;
        enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        enemy.OnEnemyDeactivated -= HandleEnemyDeactivated;
    }

    private void HandleEnemyDeactivated(Enemy enemy)
    {
        enemiesActive--;
    }

    private IEnumerator CompleteLevel()
    {
        levelCompleted = true;

        // 1. Hacer que la nave salga volando
        if (playerShip != null)
        {
           // playerShip.ExitLevel();
        }

        // 2. Esperar el tiempo de delay
        yield return new WaitForSeconds(levelEndDelay);

        // 3. Cargar siguiente nivel
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Si no hay más escenas, volver al menú principal
            SceneManager.LoadScene(0);
        }
    }
}