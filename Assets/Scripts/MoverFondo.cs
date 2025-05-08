using UnityEngine;
using UnityEngine.UI;

public class MoverFondo : MonoBehaviour
{
    [Header("Configuraci�n B�sica")]
    [Tooltip("Referencia al componente RawImage del fondo")]
    public RawImage _img;

    [Tooltip("Velocidad base de desplazamiento en X e Y")]
    public Vector2 _velocidadBase = new Vector2(0.1f, 0f);

    [Header("Movimiento Oscilante (Olas)")]
    [Tooltip("Activar movimiento ondulante")]
    public bool _usarOscilacion = true;

    [Tooltip("Amplitud del movimiento (tama�o de las olas)")]
    public Vector2 _amplitud = new Vector2(0f, 0.5f);

    [Tooltip("Frecuencia de oscilaci�n (velocidad del vaiv�n)")]
    public Vector2 _frecuencia = new Vector2(0.5f, 1f);

    [Header("Zoom Din�mico")]
    [Tooltip("Activar zoom pulsante")]
    public bool _usarZoom = false;

    [Tooltip("Escala m�nima/m�xima del zoom")]
    public Vector2 _rangoZoom = new Vector2(0.9f, 1.1f);

    [Tooltip("Velocidad del efecto de zoom")]
    public float _velocidadZoom = 0.5f;

    private Vector2 _offsetInicial;
    private Vector2 _tamanoInicial;

    private void Start()
    {
        // Guardamos los valores iniciales para referencia
        _offsetInicial = _img.uvRect.position;
        _tamanoInicial = _img.uvRect.size;
    }

    private void Update()
    {
        // --- Movimiento Base ---
        Vector2 offset = _offsetInicial;

        // Desplazamiento lineal
        offset += _velocidadBase * Time.time;

        // --- Oscilaci�n (efecto ola) ---
        if (_usarOscilacion)
        {
            offset += new Vector2(
                Mathf.Sin(Time.time * _frecuencia.x) * _amplitud.x,
                Mathf.Cos(Time.time * _frecuencia.y) * _amplitud.y
            );
        }

        // --- Zoom Din�mico ---
        Vector2 tamano = _tamanoInicial;
        if (_usarZoom)
        {
            float escala = Mathf.Lerp(
                _rangoZoom.x,
                _rangoZoom.y,
                Mathf.PingPong(Time.time * _velocidadZoom, 1f)
            );
            tamano = _tamanoInicial * escala;
        }

        // Aplicamos todos los efectos al UV Rect
        _img.uvRect = new Rect(offset, tamano);
    }
}

/*
 * using UnityEngine;
using UnityEngine.UI;

public class MoverFondo : MonoBehaviour
{
    public RawImage _img;
    public float _x;
    public float _y;

    private void Update()
    {
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}
 */
