using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    // La velocidad a la que se mueve este fondo en relación con la cámara.
    // Valores más bajos (ej: 0.1) para fondos lejanos (efecto paralaje).
    // Valores más altos (ej: 0.8) para fondos más cercanos.
    public float scrollSpeed = 0.5f;

    private float backgroundWidth;

    // La Transform de la cámara principal. La usamos para saber cuánto se ha movido la cámara.
    public Transform cameraTransform;

    // La posición anterior de la cámara. La necesitamos para calcular el movimiento.
    private Vector3 lastCameraPosition;

    void Start()
    {
       
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            
            backgroundWidth = spriteRenderer.bounds.size.x;
            Debug.Log($"[LoopingBackground] Ancho calculado para {gameObject.name}: {backgroundWidth} unidades.");
        }
        else
        {
            Debug.LogError($"[LoopingBackground] ERROR: {gameObject.name} no tiene un SpriteRenderer. No se puede calcular el ancho del fondo. Desactivando script.");
            enabled = false; 
            return;
        }

        
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                Debug.LogWarning($"[LoopingBackground] 'Camera Transform' no asignado para {gameObject.name}. Usando la cámara principal.");
            }
            else
            {
                Debug.LogError($"[LoopingBackground] ERROR: No se asignó 'Camera Transform' para {gameObject.name} ni se encontró la cámara principal. El fondo no se moverá. Desactivando script.");
                enabled = false;
                return;
            }
        }

        
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        
        if (!enabled || cameraTransform == null) return;

       
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;

       
        transform.position += new Vector3(deltaX * scrollSpeed, 0, 0);

        
        if (transform.position.x < cameraTransform.position.x - backgroundWidth)
        {
            
            transform.position = new Vector3(transform.position.x + backgroundWidth * 5, transform.position.y, transform.position.z);
            Debug.Log($"[LoopingBackground] {gameObject.name} reposicionado para loop. Nueva X: {transform.position.x}");
        }
       
        else if (transform.position.x > cameraTransform.position.x + backgroundWidth)
        {
             transform.position = new Vector3(transform.position.x - backgroundWidth * 5, transform.position.y, transform.position.z);
             Debug.Log($"[LoopingBackground] {gameObject.name} reposicionado para loop a la izquierda. Nueva X: {transform.position.x}");
        }
   
        lastCameraPosition = cameraTransform.position;
    }
}