using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Player player;

    private Vector2 moveTarget;
    private bool hasTarget = false;

    public float ufoVelocity = 1.5f;
    [SerializeField] public float ufoRotationVelocity = 125f;

    private bool isEntering = true; // Si el enemigo está en proceso de entrada

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = ufoRotationVelocity;
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (hasTarget)
        {
            if (isEntering)
            {
                moveEnemyToScene();
            }
            followPlayer();
        }
    }

    private void moveEnemyToScene()
    {
        // Calculamos la dirección hacia el target
        Vector2 direction = (moveTarget - (Vector2)transform.position).normalized;

        // Movemos el enemigo hacia el objetivo
        rb.linearVelocity = direction * ufoVelocity;

        // Cuando llega al punto objetivo
        if (Vector2.Distance(transform.position, moveTarget) < 0.05f)
        {
            transform.position = moveTarget; // Asegúrate de que esté exactamente en el target
            rb.linearVelocity = Vector2.zero; // Detenemos el movimiento
            isEntering = false;
        }
    }

    private void followPlayer()
    {
        // Movimiento para seguir al jugador, solo en X
        float distanciaCentros = player.rb.transform.position.x - transform.position.x;

        if (distanciaCentros < -0.05f)
        {
            rb.linearVelocity = new Vector2(-ufoVelocity, rb.linearVelocity.y); // Solo mueve en X
        }
        else if (distanciaCentros > 0.05f)
        {
            rb.linearVelocity = new Vector2(ufoVelocity, rb.linearVelocity.y); // Solo mueve en X
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Si está cerca del jugador, lo detiene
        }
    }

    public void SetTarget(Vector2 target)
    {
        moveTarget = target;
        hasTarget = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject); // Destruye al enemigo

            // Destruye la bala
            Destroy(collision.gameObject);
        }
    }
}
