using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnEnemyDeactivated;

    [Header("Referencias")]
    public Rigidbody2D rb;
    public Player player;

    [Header("Movimiento Inicial")]
    public float ufoVelocity = 1.5f;
    public float ufoRotationVelocity = 125f;
    private Vector2 moveTarget;
    private bool hasTarget = false;
    private bool isEntering = true;

    [Header("Movimiento Curva Infinita")]
    [SerializeField] private float infinityTime = 0f;
    public float infinitySpeed = 1f;
    public float infinityWidthMultiplier = 0.1f;
    public float infinityHeightMultiplier = 0.04f;
    public float followLerpSpeed = 2f;
    private Vector2 centerOffset;

    [Header("Rotación Visual del Sprite")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float spriteRotationSpeed = 200f;

    [Header("Sistema de Disparo")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float fireInterval = 5f;
    private float fireTimer = 0f;

    [Header("Proyectiles")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Autodestrucción")]
    [SerializeField] private float selfDestructTime = 15f;
    private bool isExisting = false;
    private float timeAlive = 0f;

    [Header("Salida del Enemigo")]
    [SerializeField] private float exitSpeed = 3f;
    [SerializeField] private float exitDestroyDelay = 3f;
    private bool isExiting = false;

    [Header("Partículas y PowerUps")]
    [SerializeField] private GameObject explosionPrefab;
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
        if (isExiting)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.normalized.x * exitSpeed, 0);
        }
    }

    private void OnBecameInvisible()
    {
        if (isExiting)
        {
            OnEnemyDeactivated?.Invoke(this);
        }
    }

    private void StartExit()
    {
        isExisting = true;
        isExiting = true;
        isEntering = false;
        hasTarget = false;

        float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * exitSpeed, 0);

        Destroy(gameObject, exitDestroyDelay); // solo se destruye si se va volando
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

        Vector2 currentCenter = Vector2.Lerp(transform.position, targetCenter, Time.deltaTime * followLerpSpeed);
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
            SpawnDeathEffect();
            Destroy(collision.gameObject);
            //Registramos la puntuacion como entero
            GameManager.GameManagerInstance.AddScore(10);
            TrySpawnPowerUp();
            DeactivateEnemy(); // aquí solo se desactiva, no se destruye
        }
    }

    private void SpawnDeathEffect()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
    }

    private void DeactivateEnemy()
    {
        ResetEnemyState();
        gameObject.SetActive(false);
        OnEnemyDeactivated?.Invoke(this);
    }

    private void ResetEnemyState()
    {
        isEntering = true;
        hasTarget = true;
        isExiting = false;
        isExisting = false;
        timeAlive = 0f;
        fireTimer = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void TrySpawnPowerUp()
    {
        if (powerUpTypes != null && powerUpTypes.Length > 0 && UnityEngine.Random.value <= 1f)
        {
            int index = UnityEngine.Random.Range(0, powerUpTypes.Length);
            if (powerUpTypes[index] != null)
            {
                GameObject powerUp = Instantiate(powerUpTypes[index], transform.position, Quaternion.identity);
                Rigidbody2D rbPowerUp = powerUp.GetComponent<Rigidbody2D>();
                if (rbPowerUp != null)
                {
                    rbPowerUp.linearVelocity = new Vector2(0, -5f);
                }
            }
        }
    }
}
