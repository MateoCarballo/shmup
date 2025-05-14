using UnityEngine;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{
    [Header("Variables de la nave")]
    public float speed = 5f;
    public Rigidbody2D rb;
    private float verticalInput, horizontalInput = 0f;
    private Vector2 startPos = new Vector2(0, -4);
    private Vector3 originalScale;

    [Header("Variables de la interpolacion de giro")]
    public float leanAmount = 0.5f;
    public float leanSmoothness = 3f;

    [Header("Bullet variables")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public Transform shootPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("Power Up Settings")]
    [SerializeField] private bool hasShield = false;
    [SerializeField] private bool isSpeedBoosted = false;
    [SerializeField] private float speedBoostMultiplier = 3f;
    [SerializeField] private float speedBoostDuration = 5f;
    private float speedBoostEndTime = 0f;

    [Header("Thrusters")]
    [SerializeField] private GameObject thrusterL;
    [SerializeField] private GameObject thrusterD;
    [SerializeField] private GameObject thrusterBackward;
    [SerializeField] private GameObject thrusterForward;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem thrusterEffect1;
    [SerializeField] private ParticleSystem thrusterEffect2;
    [SerializeField] private GameObject shieldVisual;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip powerUpBoostSFX;
    [SerializeField] private AudioClip powerUpShieldSFX;
    [SerializeField] private AudioClip powerUpMultiShootSFX;
    [SerializeField] private AudioClip powerUpLifeSFX;

    [Header("MultiShoot")]
    [SerializeField] private bool isMultiShooting;
    [SerializeField] private float multiShootDuration = 3f;
    private float multiShootEndTime = 0f;
    [SerializeField] private float multiShootAngle = 15f; // Disparo en un cono
    [SerializeField] private float multiShootFireRate = 0.1f; // Fuego mas rapido no usado

    [SerializeField] private AudioClip shootSFX;
    private AudioSource audioSource;

    private GameObject spriteDefault;
    private GameObject spritePowerUpBoost;

    [Header("UiManager")]
    [SerializeField] private UiManager uiManager;

    void Start()
    {
        transform.position = startPos;
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        shieldVisual.SetActive(false);

        //Poner los powerups a cero la primera vez que arrancamos
        isMultiShooting = false;
        hasShield = false;
        isSpeedBoosted = false;

        //Para tener el sprite default y el del boost referenciados
        spriteDefault = GameObject.Find("Player/Sprite/SpriteDefautl");
        spritePowerUpBoost = GameObject.Find("Player/Sprite/SpritePowerUpBoost");
       
    }

    void Update()
    {
        HandleInput();
        HandleLeanAnimation();
        HandleShooting();
        HandleShield();
        HandlePowerUpTimers();
    }

    private void HandleInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        UpdateThrusters();
        float currentSpeed;
        if (isSpeedBoosted)
        {
            currentSpeed = speed * speedBoostMultiplier;
            uiManager.turnPowerUpsOn(0);
        }
        else
        {
            currentSpeed = speed;
            uiManager.turnPowerUpsOff(0);
        }
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, verticalInput * currentSpeed);
    }

    private void HandleLeanAnimation()
    {
        float targetScaleX = originalScale.x - Mathf.Abs(horizontalInput) * leanAmount;
        float currentXScale = Mathf.Lerp(transform.localScale.x, targetScaleX, Time.deltaTime * leanSmoothness);
        transform.localScale = new Vector3(currentXScale, originalScale.y, originalScale.z);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void HandlePowerUpTimers()
    {
        if (isSpeedBoosted && Time.time >= speedBoostEndTime)
        {
            isSpeedBoosted = false;
            uiManager.turnPowerUpsOff(2);
            if (spriteDefault != null)
                spriteDefault.SetActive(true);

            if (spritePowerUpBoost != null)
                spritePowerUpBoost.SetActive(false);
        }

        if (isMultiShooting && Time.time >= multiShootEndTime)
        {
            uiManager.turnPowerUpsOff(0);
            isMultiShooting = false;
        }
    }

    public void Shoot()
    {
        if (isMultiShooting)
        {
            // Opci�n A: Disparo triple tipo abanico
            float[] angles = { -multiShootAngle, 0, multiShootAngle };

            foreach (float angle in angles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, rotation);
                Vector2 direction = rotation * Vector2.up;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
            }
        }
        else
        {
            // Disparo normal
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * bulletSpeed;
        }

        if (shootSFX != null) audioSource.PlayOneShot(shootSFX);
    }

    private void UpdateThrusters()
    {
        thrusterL.SetActive(horizontalInput < 0);
        thrusterD.SetActive(horizontalInput > 0);
        thrusterBackward.SetActive(verticalInput > 0);
        thrusterForward.SetActive(verticalInput < 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            HandleBulletCollision(collision);

            if (!hasShield)
            {
                GameManager.GameManagerInstance.QuitLife();
            }
            else
            {
                hasShield = false;
                shieldVisual.SetActive(false);
            }
        }
        else if (collision.CompareTag("PowerUpLife"))
        {
            ActivePowerUpLife();
            PlayPowerUpSound(powerUpLifeSFX);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("PowerUpBoost"))
        {
            ActivateSpeedBoost();
            PlayPowerUpSound(powerUpBoostSFX);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("PowerUpShield"))
        {
            ActivateShield();
            PlayPowerUpSound(powerUpShieldSFX);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("PowerUpMultiShoot"))
        {
            ActivateMultiShoot();
            PlayPowerUpSound(powerUpMultiShootSFX);
            Destroy(collision.gameObject);
        }
    }

    private void HandleBulletCollision(Collider2D bulletCollider)
    {
        var sprites = bulletCollider.GetComponentsInChildren<SpriteRenderer>(true);
        if (sprites.Length >= 2)
        {
            sprites[0].gameObject.SetActive(false);
            sprites[1].gameObject.SetActive(true);
        }

        var rb = bulletCollider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(bulletCollider.gameObject, 0.1f);
    }

    private void ActivePowerUpLife()
    {
        GameManager.GameManagerInstance.AddLife();
    }

    private void ActivateSpeedBoost()
    {
        uiManager.turnPowerUpsOn(0);
        isSpeedBoosted = true;
        speedBoostEndTime = Time.time + speedBoostDuration;

        if (spriteDefault != null)
            spriteDefault.SetActive(false);

        if (spritePowerUpBoost != null)
            spritePowerUpBoost.SetActive(true);
    }

    private void ActivateShield()
    {
        uiManager.turnPowerUpsOn(1);
        hasShield = true;
        shieldVisual.SetActive(true);
    }

    private void ActivateMultiShoot()
    {
        uiManager.turnPowerUpsOn(2);
        isMultiShooting = true;
        multiShootEndTime = Time.time + multiShootDuration;
    }

    private void HandleShield()
    {
        if (hasShield)
        {
            shieldVisual.SetActive(true);
        }
        else
        {
            shieldVisual.SetActive(false);
        }
    }

    private void PlayPowerUpSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}