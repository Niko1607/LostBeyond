using UnityEngine;


public class TorchSwitch : MonoBehaviour
{
    public int switchId;         
    public Sprite spriteOff;      
    public Sprite spriteOn;       

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    private TorchPuzzleManager puzzleManager;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer no encontrado en el GameObject: " + gameObject.name + ". ¡Asegúrate de que este GameObject tenga un SpriteRenderer!");
        }

        
        if (spriteOff == null)
        {
            Debug.LogError("Sprite 'Off' no asignado en el Inspector para la antorcha: " + gameObject.name + ". Por favor, asígnalo.");
        }
        if (spriteOn == null)
        {
            Debug.LogError("Sprite 'On' no asignado en el Inspector para la antorcha: " + gameObject.name + ". Por favor, asígnalo.");
        }

        
        puzzleManager = FindFirstObjectByType<TorchPuzzleManager>();
        if (puzzleManager == null)
        {
            
            Debug.LogWarning("TorchPuzzleManager no encontrado AÚN en la escena por la antorcha " + gameObject.name + ". Se espera que esté disponible más tarde.");
        }

        TurnOff(); 
    }

    public void TurnOn()
    {
        if (spriteRenderer != null && spriteOn != null)
        {
            spriteRenderer.sprite = spriteOn; 
        }
        isActivated = true;
        Debug.Log("Antorcha " + gameObject.name + " (ID: " + switchId + ") ENCENDIDA");
    }

    public void TurnOff()
    {
        if (spriteRenderer != null && spriteOff != null)
        {
            spriteRenderer.sprite = spriteOff; 
        }
        isActivated = false;
        Debug.Log("Antorcha " + gameObject.name + " (ID: " + switchId + ") APAGADA");
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public int GetId()
    {
        return switchId;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && puzzleManager != null)
        {
            Debug.Log("Jugador entró en el área del switch: " + switchId);
            
            if (!puzzleManager.IsPuzzleComplete() && !IsActivated())
            {
              
                puzzleManager.SetActiveTorch(this);
            }
            else
            {
                Debug.Log("Antorcha " + switchId + " ya activada o puzzle completo. No mostrar botón.");
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && puzzleManager != null)
        {
            Debug.Log("Jugador salió del área del switch: " + switchId);
            
            puzzleManager.ClearActiveTorch();
        }
    }
   
    public void ActivateSwitchFromButton()
    {
        if (puzzleManager != null)
        {
            
            puzzleManager.SwitchActivated(this);
          
            puzzleManager.ClearActiveTorch();
        }
        else
        {
            Debug.LogError("PuzzleManager no asignado al intentar activar switch desde botón.");
        }
    }


}