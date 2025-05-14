using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("GameManager")]
    GameManager gameManager;
    [Header("Referencias para la puntuacion")]
    [SerializeField] private TextMeshProUGUI textScore;

    void Start()
    {
        RestoreUiValues();
    }

    private void RestoreUiValues()
    {
        textScore.text = GameManager.GameManagerInstance.getScore().ToString();
    }
}
