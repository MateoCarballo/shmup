using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Velociddad de caida")]
    // Velocidad de movimiento en vez de gravedad le damos velocidad constante
    [SerializeField] private float moveSpeed = 10f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip healthClip;
    [SerializeField] private AudioClip speedClip;
    [SerializeField] private AudioClip shieldClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Detecta colisión con el jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aquí activaremos el efecto más adelante
            Debug.Log("PowerUp recogido!");

            PlaySoundByTag();

            // Destruye el powerup al tocarlo
            Destroy(gameObject);
        }
    }

    private void PlaySoundByTag()
    {
        switch (tag)
        {
            case "PowerUpHealth":
                audioSource.PlayOneShot(healthClip);
                break;
            case "PowerUpBoost":
                audioSource.PlayOneShot(speedClip);
                break;
            case "PowerUpShield":
                audioSource.PlayOneShot(shieldClip);
                break;
            default:
                Debug.LogWarning("PowerUp con tag desconocido: " + tag);
                break;
        }
    }
}
