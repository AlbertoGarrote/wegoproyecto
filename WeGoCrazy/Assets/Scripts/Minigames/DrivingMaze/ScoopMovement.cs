using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScoopMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 9.5f; // Aumentado para dificultad alta
    
    [Header("References")]
    public MazeController mazeController;
    [Tooltip("True guiado por WASD (Mano Izquierda), False guiado por Flechas (Mano Derecha)")]
    public bool isLeftScoop = true;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        
        // Reducir tamaño del jugador
        transform.localScale = new Vector3(0.35f, 0.35f, 1f);
    }

    void Update()
    {
        if (isLeftScoop)
        {
            movementInput.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            movementInput.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        }
        else
        {
            movementInput.x = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            movementInput.y = Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        }

        if (movementInput.magnitude > 1f)
        {
            movementInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        // Compatible con Unity 6+ (rb.linearVelocity o rb.velocity)
        rb.linearVelocity = movementInput * moveSpeed;
    }

    // --------------------- Triggers (Sin colisión física) ---------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("MazeWall")) { if (mazeController) mazeController.AddWallTouch(); }
        else if (other.gameObject.name.Contains("MazeGoal")) { if (mazeController) mazeController.ReportGoalReached(); }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("MazeWall")) { if (mazeController) mazeController.RemoveWallTouch(); }
    }

    // --------------------- Colisiones Físicas ---------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("MazeWall")) { if (mazeController) mazeController.AddWallTouch(); }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("MazeWall")) { if (mazeController) mazeController.RemoveWallTouch(); }
    }
}
