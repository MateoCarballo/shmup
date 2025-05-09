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


    //Variables para movimiento infinito en la curva 
    [SerializeField] private float infinityTime = 0f;
    [SerializeField] public float infinitySpeed = 1f;
    [SerializeField] public float infinityWidthMultiplier = 0.3f;
    [SerializeField] public float infinityHeightMultiplier = 0.12f;
    [SerializeField] public float followLerpSpeed = 2f;
    private Vector2 centerOffset;

    // Referencia al componente hijo encargado de la parte visual unicamente y la velocidad de gito
    [SerializeField] private Transform spriteTransform; // Referencia al transform de la imagen del ufo
    [SerializeField] private float spriteRotationSpeed = 200f; // Velocidad de rotación visual


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (hasTarget)
        {
            if (isEntering)
            {
                moveEnemyToScene();
            }
            else
            {
                InfinityMove();
                RotateSprite(); //Este metodo simula la rotacion pero solo mueve la imagen
                //TODO BombDropLogic();
                //followPlayerOnX();
            }
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

            //Necesario para el movimiento infinito
            centerOffset = transform.position - player.transform.position;
            infinityTime = 0f;


            isEntering = false;
        }
    }
    private void InfinityMove()
    {
        /*
         * Intento de que describa un simbolo del infinito basandome en las ecuaciones de la curba Lemniscata de Bernuilli cuyas ecuaciones parametricas en funcion de t son:
            Ecuación paramétrica
           x(t) = (a * cos(t)) / (1 + sin²(t))
           y(t) = (a * sin(t) * cos(t)) / (1 + sin²(t))

            Ecuacion implicita
            (x² + y²)² = 2a²(x² - y²)

         */

        //Avanza el tiempo usando Time.deltaTime y multiplicado por infinitySpeed para controlar la velocidad del recorrido por la curva.
        infinityTime += Time.deltaTime * infinitySpeed;

        //Nuestro tiempo, simplemente uso t porque es mas legible
        float t = infinityTime;

        // Calculo de las posiciones sobre la curva (Ecuaciones parametricas)

        //Calculo del denominador comun de las ecuaciones (1 + sin²(t))
        float denominator = 1 + Mathf.Pow(Mathf.Sin(t),2);

        //Calculo de la posicion x sobre la curva(infinityWidth es el multiplicador que lo escala en el ancho, coordenada x)
        float x = (infinityWidthMultiplier * Mathf.Cos(t)) / denominator;

        //Calculo de la posicion y sobre la curva(infinityHeight es el multiplicador que lo escala en el alto, coordenada y)
        float y = (infinityHeightMultiplier * Mathf.Sin(t) * Mathf.Cos(t)) / denominator;

        //Seguimiento del jugador a través de la escena

        //Calcula en centro objetivo sobre el que se realizara la curva con un offset
        Vector2 targetCenter = (Vector2)player.transform.position + centerOffset;

        //Interpola suavemente entre un centro y el siguiente, haciendo una transicion suave.
        //FollowSpeed hace que consiga llegar a su posicion mas rapido o lento, util si queremos hacer que cada enemigo sea mas rapido que el anterior.

        
        Vector2 currentCenter = Vector2.Lerp(
            transform.position,                 //  Posición actual
            targetCenter,                       //  Poiscion objetivo
            Time.deltaTime * followLerpSpeed);  //  Factor de interpolacion, cuanto de rapido sera la transici;on


        // ##############       APLICACION DEL MOVIMIENTO       ##############

        //Nueva posicion del enemigo
        Vector2 newPosition = currentCenter + new Vector2(x, y);

        //Mueve el rigibody manteniendo las fisicas 
        rb.MovePosition(newPosition);
    }



    //Este metodo fue el primero seguia al jugador sin moverse en Y a través del eje X

    private void followPlayerOnX()
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

    private void RotateSprite()
    {
        if (spriteTransform != null)
        {
            // Rota solo el sprite, no el GameObject principal
            spriteTransform.Rotate(Vector3.forward * spriteRotationSpeed * Time.deltaTime);
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
