using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugadortr : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float fuerzaSalto;
    [SerializeField] private Rigidbody2D rb2D;

    private float movimientoHorizontal = 0f;
    private bool estaSaltando = false;

    // Variables para la detección de suelo con BoxCast
    [SerializeField] private Transform groundCheck; // Punto de origen del BoxCast
    [SerializeField] private LayerMask groundLayer; // Capa del suelo
    [SerializeField] private Vector2 boxCastSize = new Vector2(0.5f, 0.1f); // Dimensiones de la caja de detección (ancho, alto)
    [SerializeField] private float boxCastMaxDistance = 0.1f; // Distancia máxima que la caja se proyecta hacia abajo

    private Vector3 velocidad = Vector3.zero; // Para SmoothDamp
    [SerializeField] private InputActionAsset inputActions; // Necesario para el Input System
    private Movimiento MovimientoJugador; // Esta es la clase wrapper generada por el Input System

    private void Awake()
    {
        MovimientoJugador = new Movimiento(); // Instancia la clase wrapper

        if (MovimientoJugador == null)
        {
            Debug.LogError("Error: La instancia de la clase MovimientoJugador es NULL después de instanciación en Awake. Verifica la generación del wrapper C#.");
            return;
        }

        // Suscribirse a los eventos de las acciones.
        MovimientoJugador.MovimientoJugador.Salto.performed += OnJumpPerformed;
        MovimientoJugador.MovimientoJugador.Enable(); // Habilitar la Action Map
    }

    private void OnEnable()
    {
        if (MovimientoJugador != null)
        {
            MovimientoJugador.Enable();
        }
    }

    private void OnDisable()
    {
        if (MovimientoJugador != null)
        {
            MovimientoJugador.MovimientoJugador.Salto.performed -= OnJumpPerformed;
            MovimientoJugador.Disable();
        }
    }

    private void Update()
    {
        if (MovimientoJugador == null)
        {
            return;
        }

        // Leer el valor del input horizontal
        movimientoHorizontal = MovimientoJugador.MovimientoJugador.Horizontal.ReadValue<float>() * velocidadMovimiento;

        // Verificar el estado del suelo
        bool currentIsGrounded = IsGrounded();
        Debug.Log($"Valor actual de IsGrounded(): {currentIsGrounded}");

        if (estaSaltando)
        {
            if (currentIsGrounded) // Solo salta si está en el suelo
            {
                if (rb2D != null)
                {
                    Debug.Log("¡Condición de salto cumplida! (En suelo y salto presionado). Aplicando fuerza.");
                    rb2D.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }
                else
                {
                    Debug.LogError("Rigidbody2D es NULL. Asegúrate de asignarlo en el Inspector.");
                }
                estaSaltando = false; // Resetea la bandera de salto
            }
            else
            {
                // Si intentó saltar pero no estaba en el suelo, resetea la bandera para no intentar de nuevo
                Debug.Log("Intentó saltar, pero el personaje NO está en el suelo. estaSaltando se reinicia.");
                estaSaltando = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb2D == null)
        {
            Debug.LogError("Rigidbody2D es NULL en FixedUpdate. Asegúrate de asignarlo en el Inspector.");
            return;
        }
        Mover(movimientoHorizontal * Time.fixedDeltaTime);
    }

    private void Mover(float mover)
    {
        // rb2D ya se verificó en FixedUpdate antes de llamar a Mover
        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector3.SmoothDamp(rb2D.linearVelocity, velocidadObjetivo, ref velocidad, 0f);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("¡Acción de Salto detectada! EstaSaltando a true.");
        estaSaltando = true;
    }

    // ************************************************************
    // *** NUEVA FUNCIÓN PARA DETECCIÓN DE SUELO CON BOXCAST ***
    // ************************************************************
    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck no está asignado. Asegúrate de asignarlo en el Inspector.");
            return false;
        }

        // Realiza un BoxCast desde el punto 'groundCheck' hacia abajo
        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,         // Origen de la caja
            boxCastSize,                  // Tamaño de la caja (ancho, alto)
            0f,                           // Ángulo de la caja (0 grados, no rotada)
            Vector2.down,                 // Dirección hacia abajo
            boxCastMaxDistance,           // Distancia máxima de la proyección
            groundLayer                   // Capas a detectar
        );

        return hit.collider != null; // Devuelve true si la caja golpeó algo
    }

    // ************************************************************
    // *** NUEVA FUNCIÓN PARA DIBUJAR GIZMOS DEL BOXCAST ***
    // ************************************************************
    void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        // Dibuja la caja de detección en la Scene View
        Gizmos.color = Color.blue; // Color para el BoxCast

        // Calcula los puntos para dibujar la trayectoria de la caja
        Vector2 origin = groundCheck.position;
        Vector2 target = origin + Vector2.down * boxCastMaxDistance;

        // Dibuja la caja de inicio y la caja final
        Gizmos.DrawWireCube(origin, boxCastSize);
        Gizmos.DrawWireCube(target, boxCastSize);

        // Dibuja las líneas que conectan las esquinas de las cajas para mostrar la "trayectoria"
        Vector2 halfSize = boxCastSize / 2f;
        Gizmos.DrawLine(origin + new Vector2(-halfSize.x, halfSize.y), target + new Vector2(-halfSize.x, halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(halfSize.x, halfSize.y), target + new Vector2(halfSize.x, halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(-halfSize.x, -halfSize.y), target + new Vector2(-halfSize.x, -halfSize.y));
        Gizmos.DrawLine(origin + new Vector2(halfSize.x, -halfSize.y), target + new Vector2(halfSize.x, -halfSize.y));
    }
}