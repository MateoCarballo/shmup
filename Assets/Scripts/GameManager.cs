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

    //Canvas del ui con las variables asociadas con las vidas, powerups y puntuacion
    public Canvas uiCanvas;
    // Puntuacion
    [SerializeField] private TextMeshProUGUI uiNumberScore;
    [SerializeField] private int score = 0;
    // Numero de vidas y lista con los sprites de la UI
    [SerializeField] private int maxLives = 3;
    private int currentLives;
    private int lifesIndex = 2;
    [SerializeField] private SpriteRenderer[] lifesSprites;
    // Activado o no powerup
    private bool shield;
    private bool speedBoost;
    private bool life;
    private bool multishoot = false;
    //Variables para controlar los powerups
    private int speedMultiplier;
    private int speedBoostTime;
    private int multipleShootTime;

    private void Start()
    {
        //Metodo para inicializar las vidas
        InitLifes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(); // Metodo para gestionar el activar o desactivar la escena del menu de pausa
        }

    }

    private void InitLifes()
    {
        currentLives = maxLives;
        // Activar todos los sprites de vida inicialmente
        for (int i = 0; i < lifesSprites.Length; i++)
        {
            lifesSprites[i].gameObject.SetActive(i < currentLives);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        uiNumberScore.text = score.ToString();
    }

    public void PowerUpSpeedBoost()
    {
       //Para que el GameManager sepa que tenemos este powerUp
    }

    public void PowerUpShield()
    {
        //Para que el GameManager sepa que tenemos este powerUp
    }

    public void PowerUpMultiShoot()
    {
        //Para que el GameManager sepa que tenemos este powerUp
    }

    public void QuitLife()
    {
        if (lifesIndex >= 0)
        {
            lifesSprites[lifesIndex].gameObject.SetActive(false);
            lifesIndex--;
        }
        else
        {
            // Escena final con la puntuacion y la opcion de ir al menú principal
            // SceneManager.LoadScene("GameOverScene");
            SceneManager.LoadScene("MainMenu");
        }
    }

    //Llamado desde el player cuando colisiona contra un powerUp de vida
    public bool AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLifeUI();
            return true;
        }
        return false;
    }

    // Actualizar UI de vidas
    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifesSprites.Length; i++)
        {
            lifesSprites[i].gameObject.SetActive(i < currentLives);
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
