using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangePortal : MonoBehaviour
{
    public string targetSceneName = "Escena_ResolucionAntorchas";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[SCP] Jugador entró al portal. Solicitando carga de " + targetSceneName);

            if (LoadingScreenManager.Instance != null)
            {
              LoadingScreenManager.Instance.StartLoadingScene("LoadingScene", targetSceneName); 
            }
            else
            {
                Debug.LogError("[SCP] LoadingScreenManager.Instance no encontrado. No se puede iniciar la carga de la escena de carga. Cargando directamente.");
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}