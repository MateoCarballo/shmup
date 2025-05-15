using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scorePoint;
    [SerializeField] private TextMeshProUGUI highScorePoints;
    [SerializeField] private GameObject newRecordText;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        // Configurar el botón
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Mostrar puntuaciones
        int currentScore = GameManager.GameManagerInstance.GetCurrentScore();
        int highScore = GameManager.GameManagerInstance.GetHighScore();

        scorePoint.text = $"Score: {currentScore}";
        highScorePoints.text = $"High Score: {highScore}";

        // Verificar si es un nuevo récord
        if (GameManager.GameManagerInstance.IsNewRecord())
        {
            newRecordText.SetActive(true);
            GameManager.GameManagerInstance.SaveHighScore();
        }
        else
        {
            newRecordText.SetActive(false);
        }
    }

    private void ReturnToMainMenu()
    {
        GameManager.GameManagerInstance.ResetCurrentScore();
        GameManager.GameManagerInstance.ResetParameters(); ;
        SceneManager.LoadScene("MainMenu"); // Asegúrate de que esta es el nombre de tu escena principal
    }
}