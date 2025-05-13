using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    //public GameManager gameManager;
    [Header("Variables de la nave")]
    //Variables para la posicion de la nave 
    public float speed = 5f; // This is set in the inspector
    public Rigidbody2D rb; // This is set in the inspector
    private float verticalInput, horizontalInput = 0f;
    private Vector2 startPos = new Vector2(0, -4);
    private Vector3 originalScale;

    //Variables para la interpolacion en el giro
    [Header("Variables de la interpolacion de giro")]
    public float leanAmount = 0.5f;       // Cu�nto se estrecha
    public float leanSmoothness = 3f;     // Qu� tan r�pido cambia la escala
    private float currentScaleX = 1f;     // Escala actual en X

    //Variables para los powerUp
    [Header("Power_Ups")]
    [SerializeField] private Boolean shield; //Tener o no el escudo
    [SerializeField] private int lifes; // Numero de vidas con las que partimos


    //Variables para lanzamiento de proyectil
    [Header("Bullet variables")]
    [SerializeField] public GameObject bulletPrefab; // Referencia al proycetil
    [SerializeField] public Transform shootPoint;    // Punto de disparo 
    [SerializeField] private float bulletSpeed = 10f; // Velocidad de la bala


    //Activacion de imagenes que simulan propulsion
    [Header("Thrusters")]
    [SerializeField] private GameObject thrusterL;              // Tecla A (izquierda)
    [SerializeField] private GameObject thrusterD;              // Tecla D (derecha)
    [SerializeField] private GameObject thrusterBackward;       // Tecla W (propulsión frontal)
    [SerializeField] private GameObject thrusterForward;        // Tecla S (propulsión trasera)

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem thrusterEffect1;
    [SerializeField] private ParticleSystem thrusterEffect2;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip boostPupSFX;
    [SerializeField] private AudioClip shielPupdSFX;
    [SerializeField] private AudioClip shootPupSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPos;
        lifes = 3;
        shield = false;
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * Esta parte es la que te mueve "wasd"
         */
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        UpdateThrusters();
        //ControlParticleAnimation();
        Vector2 newLinearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        rb.linearVelocity = newLinearVelocity;


        // Calcular escala objetivo segun movimiento horizontal
        float targetScaleX = originalScale.x - Mathf.Abs(horizontalInput) * leanAmount;

        /*
         Esto genera una interpolacion
        Lega de la escala actual a la escala objetivo, con un paso t. 
        Donde t es una transicion suave si importar el frame-rate " Time.deltaTime * leanSmoothness" 
        Genera una curva para llegar de un tamanho(escala) a otro y que no se aprecie un salto entre un punto y otro.
         */

        float currentXScale = Mathf.Lerp(transform.localScale.x, targetScaleX, Time.deltaTime * leanSmoothness);
        transform.localScale = new Vector3(currentXScale, originalScale.y, originalScale.z);

        /*
         Una vez definido como queremos que se "encoja" la nave pasamos este nuevo valor de "X" al parametro localScale. 
        De modo que dejamos la escala y,z como est�. 
        Y escalamos por un % el valor en X
        */
        transform.localScale = new Vector3(originalScale.x, 1.5f, 1f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            shoot();
        }

    }


    //Mi idea aqui era que salieran particulas simulando un reactor pero quedaba bastante mal y las desactive en el editor
    private void ControlParticleAnimation()
    {
        // Lógica para activar/desactivar partículas
        if (verticalInput > 0)
        {
            if (!thrusterEffect1.isPlaying) thrusterEffect1.Play();
            if (!thrusterEffect2.isPlaying) thrusterEffect2.Play();
        }
        else
        {
            if (thrusterEffect1.isPlaying) thrusterEffect1.Stop();
            if (thrusterEffect2.isPlaying) thrusterEffect2.Stop();
        }
    }
    //Esto es lo que saca las imagenes para indicar el movimiento con las imagenes asociadas a las teclas
    private void UpdateThrusters()
    {
        // Control horizontal (A/D)
        thrusterL.SetActive(horizontalInput < 0);  // Activa L con tecla A
        thrusterD.SetActive(horizontalInput > 0);  // Activa D con tecla D

        // Control vertical (W/S)
        thrusterBackward.SetActive(verticalInput > 0); // Activa backward con W
        thrusterForward.SetActive(verticalInput < 0);  // Activa forward con S
    }


    //Metodo para disparar el proyectil

    public void shoot()
    {
        ///Aqui podriamos hacer que tire rafagas con un powerup de disparomultiple

        // Instanciar un proyectil en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        //Sonido asociado a disparar
        GetComponent<AudioSource>().Play();
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.up * bulletSpeed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            // Obtener los SpriteRenderers hijos (hay varias formas; aquí es directa)
            Transform bulletTransform = collision.transform;

            SpriteRenderer[] sprites = bulletTransform.GetComponentsInChildren<SpriteRenderer>(true); // Incluye inactivos

            if (sprites.Length >= 2)
            {
                sprites[0].gameObject.SetActive(false); // Oculta sprite normal
                sprites[1].gameObject.SetActive(true);  // Muestra sprite de impacto
            }

            // Parar el movimiento
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.isKinematic = true;
            }

            // Quitar vida
            GameManager.GameManagerInstance.QuitLife();

            // Destruir después de mostrar el sprite de impacto
            Destroy(collision.gameObject, 0.1f);
        }

        // ---------- POWERUPS ----------
        if (collision.CompareTag("PowerUpHealth") ||
            collision.CompareTag("PowerUpBoost") ||
            collision.CompareTag("PowerUpShield"))
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            switch (collision.tag)
            {
                //Power up para aumentar en uno las vidas del jugador
                case "PowerUpHealth":
                    if (shootPupSFX != null) audioSource.PlayOneShot(boostPupSFX);
                    GameManager.GameManagerInstance.PowerUpLife();
                    break;
                //Power up para dar mayor velocidad al player
                case "PowerUpBoost":
                    if (boostPupSFX != null) audioSource.PlayOneShot(boostPupSFX);
                    GameManager.GameManagerInstance.PowerUpSpeedBoost();
                    break;
                // Power up para dar un escudo al personaje
                case "PowerUpShield":
                    if (shielPupdSFX != null) audioSource.PlayOneShot(shielPupdSFX);
                    GameManager.GameManagerInstance.PowerUpShield();
                    break;
                // Power up para dar un disparo multiple al personaje
                case "PowerUpMultiShoot":
                    if (shielPupdSFX != null) audioSource.PlayOneShot(shielPupdSFX);
                    GameManager.GameManagerInstance.PowerUpMultiShoot();
                    
                    break;
            }

            Destroy(collision.gameObject);
        }
    }
}