using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Enemy ufo;

    private Boolean enemyAlive = false;
    private Boolean isSpawning = false;
    private bool isPaused = false;
    public Canvas pauseCanvas;
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
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
        isPaused = !isPaused;
    }

    public void PauseGame()
    {
        pauseCanvas.enabled = true; // Mostrar el menú de pausa
        Time.timeScale = 0f; // Pausar el juego
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseCanvas.enabled = false; // Ocultar el menú de pausa
        Time.timeScale = 1f; // Reanudar el juego
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        SceneManager.LoadScene("MainMenu"); // Cargar el menú principal
    }

}
