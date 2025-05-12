using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGameManager : MonoBehaviour
{

    [Header("Configuración del Cursor")]
    [SerializeField] private Texture2D cursorTexture; // Arrastra tu sprite aquí
    [SerializeField] private Vector2 hotspot = Vector2.zero; // Punto de clic del cursor
    void Start()
    {
        SetCustomCursor();
    }

    void OnDestroy()
    {
        ResetCursor();
    }

    public void SetCustomCursor()
    {
        // Cambia el cursor
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

        // Opcional: Oculta el cursor del sistema
        // Cursor.visible = false;
    }

    public void ResetCursor()
    {
        // Restablece el cursor por defecto
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void PlayGame()
    {
        //SceneManager.LoadScene("FirstScene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Salir");
        Application.Quit();
        /*
         * #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
         */
    }
}
