using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Variable estatica para el singleton
    public static GameManager GameManagerInstance { get; private set; }

    //Variables para gestionar el menu de pausa del juego
    public Canvas pauseCanvas;
    private bool isGamePaused = false;

    //Variables de estado que deben perdurar entre niveles
    [SerializeField] private int maxLives;
    [SerializeField] private int currentLives;
    [SerializeField] private int score;

    //Esto tendria más sentido si los podemos activar con teclas(si lo tenemos disponible)
    [SerializeField] private bool isShieldEnabled;
    [SerializeField] private bool isSpeedBoostEnabled;
    [SerializeField] private bool isMultipleShootEnabled;

    //Saber si esta o no activo el power-up
    [SerializeField] private bool isShieldActive;
    [SerializeField] private bool isSpeedBoostActive;
    [SerializeField] private bool isMultipleShootActive;

    private UiManager uiManager;

    private void Awake()
    {
        //Patrón singleton
        if (GameManagerInstance != null && GameManagerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        //Inicializamos los valores que tendran al instanciar el objeto (Nivel1) con los valores base
        maxLives = 3;
        currentLives = maxLives;
        score = 0;
        isShieldActive = false;
        isSpeedBoostActive = false;
        isMultipleShootActive = false;

        //Encontramos el UiManager para poder actualizar valores frente a eventos
        uiManager = GameObject.FindGameObjectWithTag("UiPanel").GetComponent<UiManager>();
    }

    public int getScore()
    {
        return score;
    }

    public int getCurrentLifes()
    {
        return currentLives;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

    }

    public void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }



    public void AddScore(int points)
    {
        score += points;
        uiManager.UpdateScore();
    }

    public bool AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            uiManager.UpdateLifes();
            return true;
        }
        return false;
    }

    public void QuitLife()
    {
        if (currentLives >= 0)
        {
            currentLives--;
            uiManager.UpdateLifes();
        }
        else if (currentLives < 0)
        {
            //Por ahora volvemos al menu principal pero seria mejor mandarlo a la pantalla game over
            SceneManager.LoadScene("MainMenu");
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

    //Metodos asociados a pausar el juego y reanudarlo
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
