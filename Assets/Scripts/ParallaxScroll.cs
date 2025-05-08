using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{

    Vector3 startPosA1;
    Vector3 startPosA2;
    Vector3 startPosB1;
    Vector3 startPosB2;

    //Referencia a las posiciones de cada uno de los sprites usandos para el fondo
    //Los que tienen 1 aparecen en pantalla y van saliendo, los numero 2 estan fuera de la escena y van apareciendo

    //public Transform[] backgrounds;
    public Transform backgroundA1;
    public Transform backgroundB1;
    public Transform backgroundA2;
    public Transform backgroundB2;

    //Los fondos que comienzan por A estan mas cerca y van a moverse mas rapido
    //Los fondos con B van mas lento por estar mas lejos

    public float speedA = 3f;
    public float speedB = 2f;

    Vector3 startPointParallax = new Vector3(0,20,0);

    public float imageHeight = 10f; // altura de cada imagen
    private void Start()
    {
        //Registro de las posiciones de inicio ( parte superior fuera de pantalla)
        startPosA1 = backgroundA1.position;
        startPosB1 = backgroundB1.position;
    }

    void Update()
    {
        moveBackgroundA();
        moveBackgroundB();
        restarPositions();
        //comprobamos si tenemos que volver a colocarlo arriba
    }

    void moveBackgroundA()
    {
        backgroundA1.Translate(Vector3.down * speedA * Time.deltaTime);
        backgroundA2.Translate(Vector3.down * speedA * Time.deltaTime);
    }

    void moveBackgroundB()
    {
        backgroundB1.Translate(Vector3.down * speedB * Time.deltaTime);
        backgroundB2.Translate(Vector3.down * speedB * Time.deltaTime);
    }

    //Si la imagen llega abajo se resetea su posicion arriba, fuera de la pantalla
    void restarPositions()
    {

        if (backgroundA1.position.y <= -11)
        {
            backgroundA1.Translate(startPointParallax);
        }
        if (backgroundA2.position.y <= -11)
        {
            backgroundA2.Translate(startPointParallax);
        }
        if (backgroundB1.position.y <= -11)
        {
            backgroundB1.Translate(startPointParallax);
        }
        if (backgroundB2.position.y <= -11)
        {
            backgroundB2.Translate(startPointParallax);
        }
        /*
         * if (backgroundA1.position.y <= -11)
        {
            backgroundA1.Translate(startPosA1);
        }
        if (backgroundA2.position.y <= -11)
        {
            backgroundA2.Translate(startPosA1);
        }
        if (backgroundB1.position.y <= -11)
        {
            backgroundB1.Translate(startPosB1);
        }
        if (backgroundB2.position.y <= -11)
        {
            backgroundB2.Translate(startPosB1);
        }
         * */
    }
}

/*
 *  for (int i = 0; i < backgroundLayers.Length; i += 2)
    {
        float speed = speeds[i / 2];

        // Mover ambos elementos de la capa
        backgroundLayers[i].Translate(Vector3.down * speed * Time.deltaTime);
        backgroundLayers[i + 1].Translate(Vector3.down * speed * Time.deltaTime);

        // Verifica si alguno de los dos salió completamente de cámara
        if (backgroundLayers[i].position.y <= -imageHeight)
        {
            backgroundLayers[i].position = new Vector3(
                backgroundLayers[i].position.x,
                backgroundLayers[i + 1].position.y + imageHeight,
                backgroundLayers[i].position.z
            );

            // intercambia referencias si lo deseas, para mantener orden
            var temp = backgroundLayers[i];
            backgroundLayers[i] = backgroundLayers[i + 1];
            backgroundLayers[i + 1] = temp;
        }
    }
}
 */

