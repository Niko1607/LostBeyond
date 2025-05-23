using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using TMPro; 

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; 

    
    private GameObject loadingScreenPanel;
    private TextMeshProUGUI loadingText;    
    private TextMeshProUGUI clickToContinueText; 

    private string sceneToLoadName; 
    private AsyncOperation loadingOperation; 


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[LSM] Awake: Instance asignada y DontDestroyOnLoad.");
           
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Debug.LogWarning("[LSM] Awake: Ya existe una instancia, destruyendo duplicado.");
            Destroy(gameObject);
        }
    }
    public void StartLoadingScene(string loadingTransitionSceneName, string finalTargetSceneName)
    {
        Debug.Log("[LSM] StartLoadingScene: Solicitud de carga recibida. Transición a: " + loadingTransitionSceneName + ", Escena final: " + finalTargetSceneName);
        sceneToLoadName = finalTargetSceneName; 

        
        SceneManager.LoadScene(loadingTransitionSceneName);
    }
    void OnDestroy()
    {
       
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

   
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        if (scene.name == "LoadingScene")
        {
            Debug.Log("[LSM] OnSceneLoaded: ¡LoadingScene ha sido cargada! Iniciando búsqueda de elementos de UI.");
            if (loadingScreenPanel != null)
            {
                loadingScreenPanel.SetActive(true); 
                                                    
            }
            if (loadingText != null)
            {
                loadingText.text = "Cargando..."; 
                loadingText.color = Color.white;  
                loadingText.gameObject.SetActive(true); 
            }
            if (clickToContinueText != null)
            {
                clickToContinueText.gameObject.SetActive(false); 
                clickToContinueText.color = Color.white;
            }

            loadingScreenPanel = null;
            loadingText = null;
            clickToContinueText = null;

           
            GameObject foundCanvas = GameObject.Find("LoadingCanvas");
            if (foundCanvas != null)
            {
                loadingScreenPanel = foundCanvas;
                Debug.Log("[LSM] Encontrado LoadingCanvas.");

                Transform foundLoadingTextTransform = foundCanvas.transform.Find("LoadingText");
                if (foundLoadingTextTransform != null)
                {
                    loadingText = foundLoadingTextTransform.GetComponent<TextMeshProUGUI>();
                    if (loadingText != null) Debug.Log("[LSM] Encontrado LoadingText.");
                    else Debug.LogError("[LSM] ERROR: LoadingText GameObject encontrado, pero no tiene componente TextMeshProUGUI.");
                }
                else
                {
                    Debug.LogError("[LSM] ERROR: GameObject 'LoadingText' NO encontrado como hijo directo de LoadingCanvas. Revisa el nombre y la jerarquía.");
                }

                Transform foundClickToContinueTextTransform = foundCanvas.transform.Find("ClickToContinueText");
                if (foundClickToContinueTextTransform != null)
                {
                    clickToContinueText = foundClickToContinueTextTransform.GetComponent<TextMeshProUGUI>();
                    if (clickToContinueText != null) Debug.Log("[LSM] Encontrado ClickToContinueText.");
                    else Debug.LogError("[LSM] ERROR: ClickToContinueText GameObject encontrado, pero no tiene componente TextMeshProUGUI.");
                }
                else
                {
                    Debug.LogError("[LSM] ERROR: GameObject 'ClickToContinueText' NO encontrado como hijo directo de LoadingCanvas. Revisa el nombre y la jerarquía.");
                }
            }
            else
            {
                Debug.LogError("[LSM] ERROR: GameObject 'LoadingCanvas' NO encontrado en LoadingScene. Revisa el nombre.");
            }
            
            if (loadingScreenPanel != null)
            {
                loadingScreenPanel.SetActive(true); 
            }
            if (loadingText != null)
            {
                loadingText.text = "Cargando...";
                loadingText.gameObject.SetActive(true); 
            }
            if (clickToContinueText != null)
            {
                clickToContinueText.gameObject.SetActive(false);
            }

            if (loadingScreenPanel != null && !string.IsNullOrEmpty(sceneToLoadName)) 
            {
                StartCoroutine(LoadSceneAsyncCoroutine());
                Debug.Log("[LSM] Iniciando corrutina de carga asíncrona.");
            }
            else
            {
                Debug.LogError("[LSM] No se puede iniciar la carga asíncrona: loadingScreenPanel no encontrado o sceneToLoadName vacío.");
            }
        }
       
    }

   
    void Start()
    {
        // Debug.Log("[LSM] Start: Componente iniciado."); 
        
    }


    private System.Collections.IEnumerator LoadSceneAsyncCoroutine()
    {
        Debug.Log("[LSM] LoadSceneAsyncCoroutine: Iniciando carga de " + sceneToLoadName);
        loadingOperation = SceneManager.LoadSceneAsync(sceneToLoadName);

        if (loadingOperation == null)
        {
            Debug.LogError("[LSM] ERROR: loadingOperation es NULL después de LoadSceneAsync. Nombre de escena podría ser inválido: " + sceneToLoadName);
            yield break;
        }

        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
          

            if (loadingOperation.progress >= 0.9f)
            {
                Debug.Log("[LSM] Progreso >= 0.9f. Escena casi lista.");
                if (loadingText != null) loadingText.text = "¡Carga completa!";
                if (clickToContinueText != null) clickToContinueText.gameObject.SetActive(true);
                break;
            }
            yield return null;
        }

        Debug.Log("[LSM] Corrutina de carga finalizada. allowSceneActivation es " + loadingOperation.allowSceneActivation);
    }

    public void ActivateLoadedScene()
    {
        Debug.Log("[LSM] ActivateLoadedScene: Llamado. Progreso: " + (loadingOperation != null ? loadingOperation.progress.ToString() : "N/A"));
        if (loadingOperation != null && loadingOperation.progress >= 0.9f)
        {
            loadingOperation.allowSceneActivation = true;
            Debug.Log("[LSM] Escena activada.");
            if (loadingScreenPanel != null)
            {
                loadingScreenPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("[LSM] No se puede activar la escena aún (loadingOperation es null o progreso < 0.9).");
        }
    }
}