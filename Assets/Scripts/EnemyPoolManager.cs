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

    void Update()
    {
        if (!levelCompleted && enemiesSpawned < totalEnemiesToSpawn && Time.time - lastSpawnTime > spawnDelay)
        {
            TrySpawnEnemy();
            lastSpawnTime = Time.time;
        }

        // Comprobar si se debe completar el nivel
        if (!levelCompleted && enemiesSpawned >= totalEnemiesToSpawn && enemiesActive <= 0)
        {
            Debug.Log("Nivel completado. Avanzando al siguiente nivel.");
            StartCoroutine(CompleteLevel());
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemiesSpawned < totalEnemiesToSpawn)
        {
            GameObject enemy = Instantiate(ufoPrefab, spawnPoint.position, Quaternion.identity);
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            enemyComponent.SetTarget(targetPosition.position);
            enemiesSpawned++;

            Debug.Log("Enemigos generados: " + enemiesSpawned);
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
    public void IncreaseActiveEnemies()
    {
        enemiesActive++;
    }

    public void DecreaseActiveEnemies()
    {
        enemiesActive--;
    }

    private IEnumerator CompleteLevel()
    {
        levelCompleted = true;

        // Si tienes una animación o lógica para que la nave salga, puedes llamarla aquí
        if (playerShip != null)
        {
            // playerShip.ExitLevel();
        }

        yield return new WaitForSeconds(levelEndDelay);
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
            SceneManager.LoadScene(0); // Volver al menú principal si no hay más escenas
        }
    }
}
