using System.IO.IsolatedStorage;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("GameManager")]
    GameManager gameManager;
    [Header("Referencias para la puntuacion")]
    [SerializeField] private TextMeshProUGUI textScore;
    [Header("Objetos visibles en la UI")]
    [SerializeField] private SpriteRenderer[] lifesSprites;
    private int uiCurrenLifesSprite;
    [SerializeField] private SpriteRenderer[] powerUpsSprites;

    void Start()
    {
        RestoreUiValues();
    }

    private void Update()
    {
        //Aqui podria hacer que solo se actualicen las vidas ?
        RestoreLifesInUI();
    }

    private void RestoreUiValues()
    {
        //Recargar la puntuacion de la ui
        textScore.text = GameManager.GameManagerInstance.getScore().ToString();

        //Recargar el numero de vidas 
        uiCurrenLifesSprite = GameManager.GameManagerInstance.getCurrentLifes();
        RestoreLifesInUI();
        //Regargar los powerUp o no aun lo estamos decidiendo

    }

    private void RestoreLifesInUI()
    {
        // Activar los sprites del numero de vidas que tenemos realmente
        for (int i = 0; i < lifesSprites.Length; i++)
        {
            lifesSprites[i].gameObject.SetActive(i < uiCurrenLifesSprite);
        }
    }


    public void turnPowerUpsOn(int index)
    {
        powerUpsSprites[index].gameObject.SetActive(true);
    }

    public void turnPowerUpsOff(int index)
    {
        powerUpsSprites[index].gameObject.SetActive(false);
    }


}

