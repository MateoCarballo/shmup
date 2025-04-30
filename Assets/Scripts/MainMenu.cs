using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GameScreen()
    {
        SceneManager.LoadScene("01_FirstScene");
    }
    public void OptionScreen()
    {
        //Aqui abrir un segundo menu 
    }

    public void CloseGame()
    {
        //Cerrar el juego si lo pulsamos
    }

}
