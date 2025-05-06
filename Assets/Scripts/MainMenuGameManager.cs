using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGameManager : MonoBehaviour
{
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
