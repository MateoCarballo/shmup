using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    public Transform[] backgroundLayers; // cada 2 objetos son parte de una capa
    public float[] speeds; // velocidad para cada capa

    public float imageHeight = 10f; // altura de cada imagen

    void Update()
    {
        for (int i = 0; i < backgroundLayers.Length; i += 2)
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
}

