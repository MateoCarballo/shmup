using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Variables para la posicion de la nave 
   
    public float speed = 5f; // This is set in the inspector
    public Rigidbody2D rb; // This is set in the inspector
    private float verticalInput, horizontalInput = 0f;
    private Vector2 startPos = new Vector2(0, -4);

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
    public float bulletSpeed = 35f; // Velocidad de la bala



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPos;
        lifes = 3;
        shield = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * Esta parte es la que te mueve "wasd"
         */
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);


        // Calcular escala objetivo seg�n movimiento horizontal
        float targetScaleX = 1f - Mathf.Abs(horizontalInput) * leanAmount;

        /*
         Esto genera una interpolacion
        Lega de la escala actual a la escala objetivo, con un paso t. 
        Donde t es una transicion suave si importar el frame-rate " Time.deltaTime * leanSmoothness" 
        Genera una curva para llegar de un tama�o(escala) a otro y que no se aprecie un salto entre un punto y otro.
         */
        currentScaleX = Mathf.Lerp(currentScaleX, targetScaleX, Time.deltaTime * leanSmoothness);
        /*
         Una vez definido como queremos que se "encoja" la nave pasamos este nuevo valor de "X" al parametro localScale. 
        De modo que dejamos la escala y,z como est�. 
        Y escalamos por un % el valor en X
        */
        transform.localScale = new Vector3(currentScaleX, 1f, 1f);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shoot();
        }
    
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
