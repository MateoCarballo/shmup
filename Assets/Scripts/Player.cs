using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public Rigidbody2D rb;
    private float verticalInput, horizontalInput;
    private Vector2 startPos;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("Power Ups State")]
    [SerializeField] private bool[] powerupsState = { false, false, false, false }; // 0-Speed, 1-Shield, 2-MultiShoot, 3-Life

    [Header("Power Ups Contants")]
    [SerializeField] private float speedBoostMultiplier = 2f;
    [SerializeField] private float speedBoostDuration = 5f;
    [SerializeField] private float multiShootDuration = 5f;
    [SerializeField] private float multiShootAngle = 15f;
    [SerializeField] private float shieldDuration = 5f;

    [Header("Power Up Sprites")]
    [SerializeField] private GameObject[] powerupsSprites = new GameObject[2]; // 0-SpriteShield,
                                                                               // 1-SpriteBoost

    [Header("Movement Sprites")]
    [SerializeField] private GameObject[] movementSprites = new GameObject[4]; // 0:Left,
                                                                               // 1:Right,
                                                                               // 2:Back,
                                                                               // 3:Front   

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] powerUpsSFX = new AudioClip[5]; // 0-SpeedBoostSFX,
                                                                         // 1-ShieldSFX,
                                                                         // 2-MultiShootSFX,
                                                                         // 3-AddLifeSFX,
                                                                         // 4-ShootSFX
    private AudioSource audioSource;

    [Header("UI Reference")]
    [SerializeField] private UiManager uiManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        startPos = new Vector2(0, -4);
        transform.position = startPos;

        // Asegurar que todos los efectos visuales empiezan desactivados
        for (int i = 0; i < powerupsSprites.Length; i++)
        {
            if (powerupsSprites[i] != null) powerupsSprites[i].SetActive(false);
        }
       
        // Resetear estados de power-ups
        for (int i = 0; i < powerupsState.Length; i++)
        {
            powerupsState[i] = false;
        }
    }

    void Update()
    {
        HandleInput();
        HandleShooting();
    }

    private void HandleInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        UpdateMovementSprites();

        float currentSpeed = powerupsState[0] ? speed * speedBoostMultiplier : speed;
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, verticalInput * currentSpeed);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void Shoot()
    {
        if (powerupsState[2]) // Multi-shoot activo
        {
            float[] angles = { -multiShootAngle, 0, multiShootAngle };
            foreach (float angle in angles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                InstantiateBullet(shootPoint.position, rotation);
            }
        }
        else // Disparo normal
        {
            InstantiateBullet(shootPoint.position, Quaternion.identity);
        }

        PlaySound(powerUpsSFX[4]);
    }

    private void InstantiateBullet(Vector2 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = rotation * Vector2.up * bulletSpeed;
    }

    private void UpdateMovementSprites()
    {
        if (movementSprites.Length >= 4)
        {
            movementSprites[0].SetActive(horizontalInput < 0); // Left
            movementSprites[1].SetActive(horizontalInput > 0); // Right
            movementSprites[2].SetActive(verticalInput > 0);   // Back
            movementSprites[3].SetActive(verticalInput < 0);   // Front
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            //HandleBulletCollision(collision);

            if (!powerupsState[1]) // No tiene escudo
            {
                GameManager.GameManagerInstance.QuitLife();
            }
            else // Tiene escudo
            {
                powerupsState[1] = false;
            }
        }
        else if (collision.CompareTag("PowerUpBoost"))
        {
            ActiveBoolPowerUp(0);
        }
        else if (collision.CompareTag("PowerUpShield"))
        {
            ActiveBoolPowerUp(1);
        }        
        else if (collision.CompareTag("PowerUpMultiShoot"))
        {
            ActiveBoolPowerUp(2);
        }
        else if (collision.CompareTag("PowerUpLife"))
        {
            ActivePowerUpLife();
        }
        if (!collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void ActiveBoolPowerUp(int powerUpStateIndex)
    {

        powerupsState[powerUpStateIndex] = true;

        PlaySound(powerUpsSFX[powerUpStateIndex]);

        if (powerUpStateIndex == 0 || powerUpStateIndex == 1)
        {
            powerupsSprites[powerUpStateIndex].SetActive(true);
        }

        // Iniciar la corrutina para desactivarlo después del tiempo
        StartCoroutine(DeactivatePowerUpAfterTime(powerUpStateIndex, 5f));  //############# AQUI METER CADA POWER UP CON SU TIEMPO SI QUIERO MODIFICARLOS ############# 
    }

    // Corrutina para desactivar el power-up
    private IEnumerator DeactivatePowerUpAfterTime(int powerUpStateIndex, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Desactivar el power-up
        powerupsState[powerUpStateIndex] = false;

        if (powerUpStateIndex == 0 || powerUpStateIndex == 1)
        {
            powerupsSprites[powerUpStateIndex].SetActive(false);
        }
        // Opcional: Mostrar en consola para debug
        Debug.Log($"PowerUp {powerUpStateIndex} desactivado automáticamente");
    }

    private void ActivePowerUpLife()
    {
        // Llamamos a añadir vida y si tengo las maximas no sumo nada ni en el estado ni en el UI
        GameManager.GameManagerInstance.AddLife();
        PlaySound(powerUpsSFX[3]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}