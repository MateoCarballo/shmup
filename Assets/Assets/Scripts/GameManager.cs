using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Singleton para asegurar que solo tengamos una instancia

    private void Awake()
    {
        if (Instance = null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    public Canvas pauseCanvas;
    private bool isGamePaused = false;

    /*
     * Referciado al prefab del ufo y al player de la escena 
     * para poder saber sus propiedades como vida, powerups,etc.
     */
    public Player player;
    public Enemy ufo;
    private Boolean isSpawning = false;


    void Start()
    {
        //TODO la idea es que el objeto que tiene los puntos de spawn cree todos los enemigos y los active y desactive segun condiciones
        StartCoroutine(SpawnEnemy());
       // Canvas[] canvasIntances = (Canvas[])Resources.FindObjectsOfTypeAll(typeof(Canvas)); 
       // pauseCanvas = canvasIntances[0];
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(); // Metodo para gestionar el activar o desactivar la escena del menu de pausa
        }
 
    }
    
    public void TogglePause()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
        isGamePaused = !isGamePaused;
    }

    public void PauseGame()
    {
        pauseCanvas.enabled = true; // Mostrar el menú de pausa
        Time.timeScale = 0f; // Pausar el juego
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseCanvas.enabled = false; // Ocultar el menú de pausa
        Time.timeScale = 1f; // Reanudar el juego
        isGamePaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        SceneManager.LoadScene("MainMenu"); // Cargar el menú principal
    }

}
