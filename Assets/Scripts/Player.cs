using UnityEngine;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{

    [Header("Variables de la nave")]
    public float speed;
    public Rigidbody2D rb;
    private float verticalInput, horizontalInput;
    private Vector2 startPos;

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

    [Header("Player sprites")]
    [SerializeField] private GameObject spriteDefault;
    [SerializeField] private GameObject spritePowerUpBoost;

    [Header("Power Ups Effects")]
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

    [Header("UiManager")]
    [SerializeField] private UiManager uiManager;


    //public static Player Instance { get; private set; }
    private void Awake()
    {
        /*
         * //Patrón singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
         */

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //Asignar variables de la nave
        speed = 5f;
        verticalInput = 0f;
        horizontalInput = 0f;
        startPos = new Vector2(0, -4);

        transform.position = startPos;
        
        shieldVisual.SetActive(false);

        //Poner los powerups a cero la primera vez que arrancamos
        isMultiShooting = false;
        hasShield = false;
        isSpeedBoosted = false;

        //Para tener el sprite default y el del boost referenciados
        spriteDefault = GameObject.Find("Player/PlayerSprites/SpriteDefautl");
        spritePowerUpBoost = GameObject.Find("Player/PlayerSprites/SpritePowerUpBoost");

    }

    void Update()
    {
        HandleInput();
        HandleShooting();
        HandleShield();
        HandlePowerUpTimers();
        Debug.Log("Time scale: " + Time.timeScale);

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
            uiManager.turnPowerUpOnByIndex(0);
        }
        else
        {
            currentSpeed = speed;
            uiManager.turnPowerUpOffByIndex(0);
        }
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, verticalInput * currentSpeed);
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
            uiManager.turnPowerUpOffByIndex(2);
            if (spriteDefault != null)
                spriteDefault.SetActive(true);

            if (spritePowerUpBoost != null)
                spritePowerUpBoost.SetActive(false);
        }

        if (isMultiShooting && Time.time >= multiShootEndTime)
        {
            uiManager.turnPowerUpOffByIndex(0);
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
        uiManager.turnPowerUpOnByIndex(0);
        isSpeedBoosted = true;
        speedBoostEndTime = Time.time + speedBoostDuration;

        if (spriteDefault != null)
            spriteDefault.SetActive(false);

        if (spritePowerUpBoost != null)
            spritePowerUpBoost.SetActive(true);
    }

    private void ActivateShield()
    {
        uiManager.turnPowerUpOnByIndex(1);
        hasShield = true;
        shieldVisual.SetActive(true);
    }

    private void ActivateMultiShoot()
    {
        uiManager.turnPowerUpOnByIndex(2);
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