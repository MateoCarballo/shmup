using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance { get; private set; }

    //Singleton para asegurar que solo tengamos una instancia

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
        //Inicializa el numer de vidas a 3
        maxLives = 3;
        currentLives = maxLives;
        score = 0;

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
    [SerializeField] private int score;
    // Numero de vidas y lista con los sprites de la UI
    [SerializeField] private int maxLives;
    private int currentLives;

    public int getScore()
    {
        return score;
    }

    public int getCurrentLifes()
    {
        return currentLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(); // Metodo para gestionar el activar o desactivar la escena del menu de pausa
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
        uiNumberScore.text = score.ToString();
    }

    public void QuitLife()
    {
        if(currentLives >= 0)
        {
            currentLives--;
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
            return true;
        }
        return false;
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
