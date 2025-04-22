using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // This is set in the inspector
    public Rigidbody2D rb; // This is set in the inspector
    private float verticalInput, horizontalInput = 0f;
    private Vector2 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);
    }
}
