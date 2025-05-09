using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //Variables para la posicion de la nave 

    public float speed = 5f; // This is set in the inspector
    public Rigidbody2D rb; // This is set in the inspector
    private float verticalInput, horizontalInput = 0f;
    private Vector2 startPos = new Vector2(0, -4);
    private Vector3 originalScale;

    //Variables para la interpolacion en el giro

    public float leanAmount = 0.5f;       // Cu�nto se estrecha
    public float leanSmoothness = 3f;     // Qu� tan r�pido cambia la escala
    private float currentScaleX = 1f;     // Escala actual en X

    //Variables para los powerUp

    private Boolean shield; //Tener o no el escudo
    private int lifes; // Numero de vidas con las que partimos


    //Variables para lanzamiento de proyectil
    public GameObject bulletPrefab; // Referencia al proycetil
    public Transform shootPoint;    // Punto de disparo 
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
        //UpdateThrusters();
        ControlParticleAnimation();
        Vector2 newLinearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        rb.linearVelocity = newLinearVelocity;


        // Calcular escala objetivo seg�n movimiento horizontal
        float targetScaleX = originalScale.x - Mathf.Abs(horizontalInput) * leanAmount;

        /*
         Esto genera una interpolacion
        Lega de la escala actual a la escala objetivo, con un paso t. 
        Donde t es una transicion suave si importar el frame-rate " Time.deltaTime * leanSmoothness" 
        Genera una curva para llegar de un tama�o(escala) a otro y que no se aprecie un salto entre un punto y otro.
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
        // Instanciar un proyectil en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.up * bulletSpeed;

    }

    // La idea es que cuando choquemos con un objeto (Ataque enemigo, asteroide) nos reste la vida mediante este metodo
    public void lessLife()
    {
        lifes--;
    }


    // La idea es que cuando choquemos con un powerUp nos aumente la vida en 1
    public void powerUpLife()
    {
        lifes++;
    }

    //Si no tenemos escudo y chocamos contra un escudo hacemos que nos cambie el booleano escudo a true
    public void powerUpShield()
    {
        if (!shield)
        {
            shield = true;
        }
    }
}