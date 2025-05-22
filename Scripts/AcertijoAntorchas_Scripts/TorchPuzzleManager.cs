using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TorchPuzzleManager : MonoBehaviour
{
    
    private List<int> correctSequence = new List<int>();

    
    public List<TorchSwitch> allSwitchesInPuzzle;
    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleFailed;
    [Header("Transición de Escena")]
    public string nextSceneAfterPuzzle = "EndGameScene";

    private List<int> playerSequence = new List<int>();
    private int currentStep = 0;
    private bool puzzleIsComplete = false;
    public Button interactionButton; 
    private TorchSwitch activeTorch = null; 

    void Start()
    {
        
        if (GameProgressManager.Instance != null)
        {
            correctSequence = GameProgressManager.Instance.GetCollectedClues();

            if (correctSequence.Count == 0)
            {
                Debug.LogError("¡TorchPuzzleManager: No se han recogido pistas del GameProgressManager! La secuencia correcta está vacía. " +
                               "Asegúrate de 'recoger' pistas (ej: presionando los botones en la escena de recolección) antes de llegar a este puzzle.");

               
            }
            else
            {
               
                string seqStr = "TorchPuzzleManager: Secuencia correcta cargada desde GameProgressManager: ";
                foreach (int num in correctSequence)
                {
                    seqStr += num + " ";
                }
                Debug.Log(seqStr);
            }
        }
        else
        {
            Debug.LogError("TorchPuzzleManager: GameProgressManager.Instance no encontrado. " +
                           "Asegúrate de que exista un GameObject con el script GameProgressManager en la escena y que se haya inicializado (y tenga DontDestroyOnLoad).");

            
        }
       

        if (allSwitchesInPuzzle == null || allSwitchesInPuzzle.Count == 0)
        {
            Debug.LogWarning("TorchPuzzleManager: No hay antorchas asignadas en la lista 'All Switches In Puzzle' en el Inspector.");
        }
        if (interactionButton != null)
        {
            interactionButton.gameObject.SetActive(false); 
                                                           
                                                           
            interactionButton.onClick.RemoveAllListeners();
            interactionButton.onClick.AddListener(OnInteractionButtonClicked);
        }
        else
        {
            Debug.LogError("TorchPuzzleManager: ¡El 'Interaction Button' no está asignado en el Inspector! La interacción no funcionará.");
        }




        ResetPuzzle();

    }
    public void OnInteractionButtonClicked()
    {
        if (activeTorch != null) 
        {
            activeTorch.ActivateSwitchFromButton();
            
        }
        else
        {
            Debug.LogWarning("TorchPuzzleManager: Botón de interacción presionado, pero no hay antorcha cerca para activar.");
        }
    }
    public void SetActiveTorch(TorchSwitch torch)
    {
        
        activeTorch = torch; 
        if (interactionButton != null)
        {
            interactionButton.gameObject.SetActive(true); 
        }
    }
    public void ClearActiveTorch()
    {
        
        activeTorch = null; 
        if (interactionButton != null)
        {
            interactionButton.gameObject.SetActive(false);
        }
    }
    public bool IsPuzzleComplete()
    {
        return puzzleIsComplete;
    }

    public void SwitchActivated(TorchSwitch activatedSwitch)
    {
        if (puzzleIsComplete) 
        {
            return;
        }

        if (activatedSwitch.IsActivated() && playerSequence.Contains(activatedSwitch.GetId()))
        {
            
            Debug.Log("TorchPuzzleManager: La antorcha " + activatedSwitch.GetId() + " ya estaba activada en la secuencia actual.");
            return;
        }

       
        if (!activatedSwitch.IsActivated())
        {
            activatedSwitch.TurnOn(); 
        }
        playerSequence.Add(activatedSwitch.GetId());

       
        if (correctSequence.Count > 0 && currentStep < correctSequence.Count && activatedSwitch.GetId() == correctSequence[currentStep])
        {
            currentStep++;
            
            if (currentStep == correctSequence.Count) 
            {
                PuzzleSolved();
            }
        }
        else
        {
          
            Debug.Log("TorchPuzzleManager: Secuencia incorrecta. Antorcha activada: " + activatedSwitch.GetId() +
                      (correctSequence.Count > 0 && currentStep < correctSequence.Count ? ". Se esperaba: " + correctSequence[currentStep] : ". (Secuencia correcta no disponible o ya superada)") +
                      ". Reiniciando puzzle.");
            if (onPuzzleFailed != null)
            {
                onPuzzleFailed.Invoke(); 
            }
            ResetPuzzle();
        }
    }

    void PuzzleSolved()
    {
        Debug.Log("¡TorchPuzzleManager: Puzzle Resuelto!");
        puzzleIsComplete = true;

       
        foreach (TorchSwitch ts in allSwitchesInPuzzle)
        {
            if (ts != null)
            {
                var collider = ts.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        if(onPuzzleSolved != null)
        {
            onPuzzleSolved.Invoke(); 
        }
        ClearActiveTorch();
        if (!string.IsNullOrEmpty(nextSceneAfterPuzzle))
        {
            if (LoadingScreenManager.Instance != null) 
            {
                LoadingScreenManager.Instance.StartLoadingScene("LoadingScene", nextSceneAfterPuzzle); 
            }
            else
            {
                Debug.LogError("[TPM] LoadingScreenManager.Instance no encontrado. Fallback a carga directa.");
                SceneManager.LoadScene(nextSceneAfterPuzzle); 
            }
        }
        else
        {
            Debug.LogWarning("TorchPuzzleManager: 'Next Scene After Puzzle' no definido. El juego termina aquí.");
        }

    }
    private System.Collections.IEnumerator CallLoadingManagerAfterDelay(string name)
    {
        yield return null; 
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartLoadingScene("LoadingScene", nextSceneAfterPuzzle);
        }
        else
        {
            Debug.LogError("LoadingScreenManager no encontrado. ¡No se pudo iniciar la carga asíncrona! Asegúrate de que existe en LoadingScene.");
            SceneManager.LoadScene(name); 
        }
    }
    public void ResetPuzzle()
    {
        Debug.Log("TorchPuzzleManager: Reiniciando estado del puzzle de antorchas.");
        playerSequence.Clear();
        currentStep = 0;
        puzzleIsComplete = false; 

        foreach (TorchSwitch ts in allSwitchesInPuzzle)
        {
            if (ts != null)
            {
                ts.TurnOff(); 
                var collider = ts.GetComponent<Collider2D>(); 
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
        }
         ClearActiveTorch();
    }

}