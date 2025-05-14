using System.IO.IsolatedStorage;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("GameManager")]
    //GameManager gameManager;
    [Header("Referencias para la puntuacion")]
    [SerializeField] private TextMeshProUGUI textScore;
    [Header("Objetos visibles en la UI")]
    [SerializeField] private SpriteRenderer[] lifesSprites;
    private int uiCurrenLifesSprite;
    [SerializeField] private SpriteRenderer[] powerUpsSprites;

    void Start()
    {
        UpdateScore();
        UpdateLifes();
    }
    //Actualizar la puntuacion en la UI
    public void UpdateScore()
    {
        textScore.text = GameManager.GameManagerInstance.getScore().ToString();
    }
    //Actualizar las vidas en la UI

    public void UpdateLifes()
    {
        uiCurrenLifesSprite = GameManager.GameManagerInstance.getCurrentLifes();

        for (int i = 0; i < lifesSprites.Length; i++)
        {
            lifesSprites[i].gameObject.SetActive(i < uiCurrenLifesSprite);
        }
    }

    //Activar simbolo de power up
    public void turnPowerUpOnByIndex(int index)
    {
        powerUpsSprites[index].gameObject.SetActive(true);
    }
    //Desactivar simbolo de power up
    public void turnPowerUpOffByIndex(int index)
    {
        powerUpsSprites[index].gameObject.SetActive(false);
    }


}

