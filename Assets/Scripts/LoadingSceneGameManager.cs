using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneGameManager : MonoBehaviour
{
    [Header("Configuración del Cursor")]
    [SerializeField] private Texture2D cursorTexture; // Arrastra tu sprite aquí
    [SerializeField] private Vector2 hotspot = Vector2.zero; // Punto de clic del cursor

    [Header("Configuración de Carga")]
    [SerializeField] private float loadDelay = 5f; // Tiempo en segundos antes de cargar

    void Start()
    {
        SetCustomCursor();
        StartCoroutine(LoadNextLevelAfterDelay());
    }

    void OnDestroy()
    {
        ResetCursor();
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        // Espera el tiempo configurado
        yield return new WaitForSeconds(loadDelay);

        // Carga el siguiente nivel
        PlayGame();
    }

    public void SetCustomCursor()
    {
        // Cambia el cursor
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        // Restablece el cursor por defecto
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}