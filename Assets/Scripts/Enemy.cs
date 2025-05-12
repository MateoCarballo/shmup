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

    private float timeAlive = 0f;

    // -------------------- EFECTOS VISUALES --------------------
    [Header("Partículas")]
    [Tooltip("Efecto al ser golpeado")]
    [SerializeField] private ParticleSystem hittedEfect;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        RotateSprite();
        if (hasTarget)
        {
            if (isEntering)
            {
                moveEnemyToScene();
            }
            else
            {
                InfinityMove();
                
                //Este metodo simula la rotacion pero solo mueve la imagen
                //TODO BombDropLogic();
                //followPlayerOnX();

                //Contadores para autodestruccion y disparo jugador
                fireTimer += Time.deltaTime;
                timeAlive += Time.deltaTime;

                if (fireTimer >= fireInterval)
                {
                    ShootPlayer();
                    fireTimer = 0f;
                }

                if (timeAlive >= selfDestructTime)
                {
                    ExitSceneAndDestroy();
                }
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

            timeAlive = 0f;
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
        float denominator = 1 + Mathf.Pow(Mathf.Sin(t), 2);

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


    private void ShootPlayer()
    {
        if (enemyBulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            // Posición y velocidad del jugador
            Vector2 playerPos = player.transform.position;
            Vector2 playerVel = player.rb != null ? player.rb.linearVelocity : Vector2.zero;

            // Tiempo estimado que tardará la bala en llegar al jugador
            float distance = Vector2.Distance(transform.position, playerPos);
            float estimatedTime = distance / bulletSpeed;

            // Posición futura estimada del jugador
            Vector2 futurePos = playerPos + playerVel * estimatedTime;

            // Dirección mejorada con predicción
            Vector2 direction = (futurePos - (Vector2)transform.position).normalized;

            bulletRb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject); // Destruye al enemigo
            ControlParticleAnimation(); //Activa particulas del coche
            // Destruye la bala
            Destroy(collision.gameObject);
            GameManager.GameManagerInstance.AddScore(10);
        }
    }

    private void ExitSceneAndDestroy()
    {
        // Sale hacia arriba o dirección deseada
        rb.linearVelocity = Vector2.up * 3f;

        // Destruir tras unos segundos para asegurarse de que sale de pantalla
        Destroy(gameObject, 2f);
    }


    private void ControlParticleAnimation()
    {
        // Lógica para activar/desactivar partículas
        {
            if (!hittedEfect.isPlaying) hittedEfect.Play();
        }
    }
}
