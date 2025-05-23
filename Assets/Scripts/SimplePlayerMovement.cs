using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float speed = 5f; 
    public Rigidbody2D rb;    

    public Joystick joystick; 

   
    void Start()
    {
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("SimplePlayerMovement: Rigidbody2D no encontrado en el jugador. El movimiento físico no funcionará. ¡Añade un Rigidbody2D!");
                enabled = false; 
                return; 
            }
        }

        
        if (joystick == null)
        {
            Debug.LogWarning("SimplePlayerMovement: Joystick no asignado al jugador en el Inspector. El movimiento en móvil podría no funcionar. Usando entrada de teclado como respaldo para PC.");
        }
    }

    
    void FixedUpdate()
    {
        
        if (rb == null) return;

        float moveHorizontal = 0f;

       
        if (joystick != null)
        {
            
            moveHorizontal = joystick.Direction.x; 
        }

        
        if (moveHorizontal == 0f) 
        {
            moveHorizontal = Input.GetAxis("Horizontal"); 
        }

        
        if (rb.bodyType == RigidbodyType2D.Dynamic || rb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.linearVelocity = new Vector2(moveHorizontal * speed, rb.linearVelocity.y);
        }
        
    }

    
}