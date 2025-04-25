using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody2D rb;
    public Player player;
    //Posiciones de aparicion de enemigos
    private Vector2 ufoPosition = new Vector2(0, 4);
    private float distanciaCentros = 0f;
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

        distanciaCentros = player.rb.transform.localPosition.x - rb.transform.localPosition.x;


        //Si el jugador esta a mi izquierda le doy velocidad hacia la izquierda 
        if (distanciaCentros < -0.05)
        {
            rb.linearVelocity = new Vector2(-1 * ufoVelocity, rb.linearVelocity.y);  
        }

        //Si la posicion del jugador esta a la derecha le doy velocidad hacia la derecha  
        if (distanciaCentros > 0.05)
        {
            rb.linearVelocity = new Vector2(1 * ufoVelocity, rb.linearVelocity.y);
        }
        // Busca que la distancia en x sea menor a un valor definido

        
        if (distanciaCentros >  -0.05 && distanciaCentros < 0.05)
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
