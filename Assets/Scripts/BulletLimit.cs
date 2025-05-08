using UnityEngine;

public class BulletLimit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject); // Opcional: tambi�n destruye la bala
        }
    }
}
