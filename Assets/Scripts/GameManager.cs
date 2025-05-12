using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance { get; private set; }

    //Singleton para asegurar que solo tengamos una instancia

    private void Awake()
    {
        if (GameManagerInstance == null)
        {
            GameManagerInstance = this;
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


    //Canvas del ui con las variables asociadas con las vidas, powerups y puntuacion
    public Canvas uiCanvas;
    // Puntuacion
    [SerializeField] private TextMeshProUGUI uiNumberScore;
    [SerializeField] private int score = 0;
    // Vidas
    private int lifes;
    // Activado o no powerup
    private bool shield;
    private bool speedBoost;
    private bool multipleShoot;
    //Variables para controlar los powerups
    private int speedMultiplier;
    private int speedBoostTime;
    private int multipleShootTime;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(); // Metodo para gestionar el activar o desactivar la escena del menu de pausa
        }
 
    }

    public void AddScore(int points)
    {
        score += points;
        uiNumberScore.text = score.ToString();
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
