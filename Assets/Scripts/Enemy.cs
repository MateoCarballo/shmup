using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // -------------------- REFERENCIAS --------------------
    [Header("Referencias")]
    public Rigidbody2D rb;
    public Player player;

    // -------------------- MOVIMIENTO DE ENTRADA --------------------
    [Header("Movimiento Inicial")]
    [Tooltip("Velocidad con la que entra a la escena")]
    public float ufoVelocity = 1.5f;

    [Tooltip("Velocidad angular (si se usa para rotación física)")]
    [SerializeField] public float ufoRotationVelocity = 125f;

    private Vector2 moveTarget;
    private bool hasTarget = false;
    private bool isEntering = true;

    // -------------------- MOVIMIENTO INFINITO --------------------
    [Header("Movimiento Curva Infinita")]
    [SerializeField] private float infinityTime = 0f;

    [Tooltip("Velocidad de avance por la curva")]
    [SerializeField] public float infinitySpeed = 1f;

    [Tooltip("Escala horizontal de la curva")]
    [SerializeField] public float infinityWidthMultiplier = 0.1f;

    [Tooltip("Escala vertical de la curva")]
    [SerializeField] public float infinityHeightMultiplier = 0.04f;

    [Tooltip("Qué tan rápido sigue al jugador en su movimiento")]
    [SerializeField] public float followLerpSpeed = 2f;

    private Vector2 centerOffset;

    // -------------------- ROTACIÓN VISUAL --------------------
    [Header("Rotación Visual del Sprite")]
    [Tooltip("Transform del sprite que rota visualmente")]
    [SerializeField] private Transform spriteTransform;

    [Tooltip("Velocidad de rotación visual del sprite")]
    [SerializeField] private float spriteRotationSpeed = 200f;

    // -------------------- DISPARO AL JUGADOR --------------------
    [Header("Sistema de Disparo")]
    [Tooltip("Prefab de la bala enemiga")]
    [SerializeField] private GameObject enemyBulletPrefab;

    [Tooltip("Tiempo entre cada disparo")]
    [SerializeField] private float fireInterval = 5f;

    private float fireTimer = 0f;

    // -------------------- CONFIGURACIÓN DE BALAS --------------------
    [Header("Parámetros de Proyectil")]
    [Tooltip("Prefab del proyectil (opcional alternativo)")]
    public GameObject bulletPrefab;

    [Tooltip("Transform desde donde se dispara")]
    public Transform shootPoint;

    [Tooltip("Velocidad de la bala")]
    [SerializeField] private float bulletSpeed = 10f;

    // -------------------- AUTODESTRUCCIÓN --------------------
    [Header("Autodestrucción")]
    [Tooltip("Tiempo máximo de vida del enemigo")]
    [SerializeField] private float selfDestructTime = 15f;
    [SerializeField] private bool isExisting = false;
    [SerializeField] private float timeAlive = 0f;

    // -------------------- SISTEMA DE SALIDA --------------------
    [Header("Configuración de Salida")]
    [Tooltip("Velocidad al salir de pantalla")]
    [SerializeField] private float exitSpeed = 3f;
    [Tooltip("Tiempo antes de destruir al salir")]
    [SerializeField] private float exitDestroyDelay = 3f;
    private bool isExiting = false;

    // -------------------- EFECTOS VISUALES --------------------
    [Header("Partículas")]
    [Tooltip("Efecto al ser golpeado")]
    [SerializeField] private ParticleSystem hittedEfect;
    [SerializeField] private GameObject[] powerUpTypes;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        RotateSprite();

        // Si está en modo salida, no procesar otros movimientos
        if (isExiting) return;

        if (hasTarget)
        {
            if (isEntering)
            {
                moveEnemyToScene();
            }
            else
            {
                InfinityMove();

                fireTimer += Time.deltaTime;
                timeAlive += Time.deltaTime;

                if (fireTimer >= fireInterval)
                {
                    ShootPlayer();
                    fireTimer = 0f;
                }

                if (timeAlive >= selfDestructTime && !isExisting)
                {
                    StartExit();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Mantener velocidad constante durante la salida
        if (isExiting)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.normalized.x * exitSpeed, 0);
        }
    }

    private void StartExit()
    {
        isExisting = true;
        isExiting = true;
        isEntering = false;
        hasTarget = false;

        // Elegir dirección aleatoria (izquierda o derecha)
        float direction = Random.value > 0.5f ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * exitSpeed, 0);

        // Programar destrucción
        Destroy(gameObject, exitDestroyDelay);
    }

    private void moveEnemyToScene()
    {
        Vector2 direction = (moveTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * ufoVelocity;

        if (Vector2.Distance(transform.position, moveTarget) < 0.05f)
        {
            transform.position = moveTarget;
            rb.linearVelocity = Vector2.zero;
            centerOffset = transform.position - player.transform.position;
            infinityTime = 0f;
            isEntering = false;
            timeAlive = 0f;
        }
    }

    private void InfinityMove()
    {
        infinityTime += Time.deltaTime * infinitySpeed;
        float t = infinityTime;
        float denominator = 1 + Mathf.Pow(Mathf.Sin(t), 2);
        float x = (infinityWidthMultiplier * Mathf.Cos(t)) / denominator;
        float y = (infinityHeightMultiplier * Mathf.Sin(t) * Mathf.Cos(t)) / denominator;
        Vector2 targetCenter = (Vector2)player.transform.position + centerOffset;

        Vector2 currentCenter = Vector2.Lerp(
            transform.position,
            targetCenter,
            Time.deltaTime * followLerpSpeed);

        Vector2 newPosition = currentCenter + new Vector2(x, y);
        rb.MovePosition(newPosition);
    }

    private void RotateSprite()
    {
        if (spriteTransform != null)
        {
            spriteTransform.Rotate(Vector3.forward * spriteRotationSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Vector2 target)
    {
        moveTarget = target;
        hasTarget = true;
    }

    private void ShootPlayer()
    {
        if (enemyBulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            Vector2 playerPos = player.transform.position;
            Vector2 playerVel = player.rb != null ? player.rb.linearVelocity : Vector2.zero;
            float distance = Vector2.Distance(transform.position, playerPos);
            float estimatedTime = distance / bulletSpeed;
            Vector2 futurePos = playerPos + playerVel * estimatedTime;
            Vector2 direction = (futurePos - (Vector2)transform.position).normalized;
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            ControlParticleAnimation();
            Destroy(collision.gameObject);
            GameManager.GameManagerInstance.AddScore(10);
            
            //Probabilidad de que nos suelte un powerUp
            if (Random.value > 0.8)
            {
                int powerUpIndex = Random.Range(0, powerUpTypes.Length);

                GameObject powerUp = Instantiate(powerUpTypes[powerUpIndex], transform.position, Quaternion.identity);
                //Añade un efecto cuando aparece el powerUp
                powerUp.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1f), ForceMode2D.Impulse);
            }
        }
    }

    private void ControlParticleAnimation()
    {
        if (!hittedEfect.isPlaying) hittedEfect.Play();
    }
}