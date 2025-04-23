using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody2D rb;
    public Player player;
    //Posiciones de aparicion de jugador y enemigos
    private Vector2 ufoPosition = new Vector2(0, 4);

    public float ufoVelocity =1.5f;
    public float horizontalPositionPlayer = 0f;

    void Start()
    {
        transform.position = ufoPosition;
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>(); 
    }

    void Update()
    {
        //Si el jugador esta a mi izquierda le doy velocidad hacia la izquierda 
        if (player.rb.transform.localPosition.x < rb.transform.localPosition.x)
        {
            rb.linearVelocity = new Vector2(-1 * ufoVelocity, rb.linearVelocity.y);  
        }

        //Si la posicion del jugador esta a la derecha le doy velocidad hacia la derecha  
        if (player.rb.transform.localPosition.x > rb.transform.localPosition.x)
        {
            rb.linearVelocity = new Vector2(1 * ufoVelocity, rb.linearVelocity.y);
        }

        if (player.rb.transform.localPosition.x == rb.transform.localPosition.x)
        {
            rb.linearVelocity = new Vector2(0,0);
        }

    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject); // Destruye al enemigo
            Destroy(collision.gameObject); // Opcional: también destruye la bala
        }
    }
}
