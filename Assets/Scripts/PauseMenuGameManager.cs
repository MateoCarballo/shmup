using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGameManager : MonoBehaviour
{
    public static PauseMenuGameManager Instance;
    public string pauseSceneName = "PauseMenu";
    private bool isPaused;
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pausar juego y cargar escena de pausa
            Time.timeScale = 0f;
            SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
        }
        else
        {
            // Reanudar juego y descargar escena de pausa
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(pauseSceneName);
        }
    }
}
