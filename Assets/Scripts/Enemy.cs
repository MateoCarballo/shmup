using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float enterSpeed = 1.5f;
    public float exitSpeed = 3f;
    public float selfDestructTime = 15f;
    private float timeAlive = 0f;
    private bool isEntering = true;
    private bool isExiting = false;
    private Vector2 moveTarget;
    private Vector2 centerOffset;
    private Vector2 exitDirection;

    [Header("Curva Infinita")]
    public float infinitySpeed = 1f;
    public float infinityWidthMultiplier = 0.1f;
    public float infinityHeightMultiplier = 0.04f;
    public float followLerpSpeed = 2f;
    private float infinityTime = 0f;

    [Header("Jugador y Disparo")]
    public GameObject enemyBulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 10f;
    public float fireInterval = 5f;
    private float fireTimer = 0f;

    [Header("Extras")]
    public GameObject explosionPrefab;
    public GameObject[] powerUpTypes;

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        GenerateEnemys.Instance.IncreaseActiveEnemies(); // notifica que hay un enemigo más
    }

    void Update()
    {
        if (isExiting) return;

        fireTimer += Time.deltaTime;
        timeAlive += Time.deltaTime;

        if (isEntering)
        {
            MoveToEntry();
        }
        else if (!isExiting)
        {
            InfinityMove();

            if (fireTimer >= fireInterval)
            {
                Shoot();
                fireTimer = 0f;
            }

            if (timeAlive >= selfDestructTime)
            {
                StartExit();
            }
        }
    }

    void FixedUpdate()
    {
        if (isExiting)
        {
            rb.linearVelocity = exitDirection * exitSpeed;
            return;
        }

        if (!isEntering) return;
    }

    public void SetTarget(Vector2 target)
    {
        moveTarget = target;
    }

    private void MoveToEntry()
    {
        Vector2 direction = (moveTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * enterSpeed;

        if (Vector2.Distance(transform.position, moveTarget) < 0.05f)
        {
            transform.position = moveTarget;
            rb.linearVelocity = Vector2.zero;
            isEntering = false;
            centerOffset = transform.position - player.position;
            infinityTime = 0f;
            timeAlive = 0f;
        }
    }

    private void InfinityMove()
    {
        infinityTime += Time.deltaTime * infinitySpeed;
        float t = infinityTime;
        float denom = 1 + Mathf.Pow(Mathf.Sin(t), 2);
        float x = (infinityWidthMultiplier * Mathf.Cos(t)) / denom;
        float y = (infinityHeightMultiplier * Mathf.Sin(t) * Mathf.Cos(t)) / denom;

        Vector2 targetCenter = (Vector2)player.position + centerOffset;
        Vector2 currentCenter = Vector2.Lerp(transform.position, targetCenter, Time.deltaTime * followLerpSpeed);
        Vector2 desiredPosition = currentCenter + new Vector2(x, y);

        Vector2 direction = (desiredPosition - (Vector2)transform.position).normalized;
        float speed = Vector2.Distance(transform.position, desiredPosition) / Time.deltaTime;

        Vector2 velocity = direction * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, velocity, Time.deltaTime * 5f); //Usar interpolacion suavia los movimientos
    }

    private void Shoot()
    {
        if (enemyBulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(enemyBulletPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null) return;

        Vector2 playerPos = player.position;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Vector2 playerVel = playerRb != null ? playerRb.linearVelocity : Vector2.zero;
        float distance = Vector2.Distance(transform.position, playerPos);
        float timeToHit = distance / bulletSpeed;
        Vector2 futurePos = playerPos + playerVel * timeToHit;
        Vector2 direction = (futurePos - (Vector2)transform.position).normalized;

        bulletRb.linearVelocity = direction * bulletSpeed;
    }

    private void StartExit()
    {
        if (isExiting) return;

        isExiting = true;
        isEntering = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        exitDirection = new Vector2(Random.value > 0.5f ? 1f : -1f, 0f);
    }

    public void ForceDeactivate()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GenerateEnemys.Instance != null)
            GenerateEnemys.Instance.DecreaseActiveEnemies();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            SpawnExplosion();
            GameManager.GameManagerInstance.AddScore(10);
            TrySpawnPowerUp();
            Destroy(gameObject);
        }

        if (other.CompareTag("KillZone"))
        {
            Destroy(gameObject);
        }
    }

    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 2f);
        }
    }

    private void TrySpawnPowerUp()
    {
        if (powerUpTypes.Length > 0 && Random.value < 1f) // siempre lanza uno, cambia si quieres probabilidad
        {
            int index = Random.Range(0, powerUpTypes.Length);
            GameObject powerUp = Instantiate(powerUpTypes[index], transform.position, Quaternion.identity);
            Rigidbody2D rbPowerUp = powerUp.GetComponent<Rigidbody2D>();
            if (rbPowerUp != null)
                rbPowerUp.linearVelocity = new Vector2(0, -5f);
        }
    }
}
