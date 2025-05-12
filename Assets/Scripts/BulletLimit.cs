using UnityEngine;

public class BulletLimit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") || collision.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject); // Opcional: también destruye la bala
        }
    }
}
