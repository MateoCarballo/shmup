using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Velocidad de movimiento en vez de gravedad le damos velocidad constante
    [SerializeField]public float moveSpeed = 2f;

    void Update()
    {
        // Mueve el powerup hacia abajo
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    // Detecta colisi�n con el jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aqu� activaremos el efecto m�s adelante
            Debug.Log("PowerUp recogido!");

            // Destruye el powerup al tocarlo
            Destroy(gameObject);
        }
    }
}
