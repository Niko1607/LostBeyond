using UnityEngine;
using UnityEngine.UI; // O using TMPro; si usas botones TextMeshPro
using UnityEngine.SceneManagement; // Para cambiar de escena

public class RecoleccionPistas_UI_Manager : MonoBehaviour
{
    // Asigna estos botones desde el Inspector
    public Button botonPista5;
    public Button botonPista9;
    public Button botonPista7;
    public Button botonIrAPuzzle;

    void Start()
    {
       
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ClearClues();
        }

        if (botonPista5 != null) botonPista5.onClick.AddListener(() => AddSpecificClue(5));
        if (botonPista9 != null) botonPista9.onClick.AddListener(() => AddSpecificClue(9));
        if (botonPista7 != null) botonPista7.onClick.AddListener(() => AddSpecificClue(7));

        if (botonIrAPuzzle != null) botonIrAPuzzle.onClick.AddListener(GoToPuzzleScene);
    }

    void AddSpecificClue(int clueValue)
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.AddClue(clueValue);
            Debug.Log("Botón presionado, añadiendo pista: " + clueValue);
           
        }
        else
        {
            Debug.LogError("GameProgressManager no encontrado!");
        }
    }

    void GoToPuzzleScene()
    {
        
        SceneManager.LoadScene("Escena_ResolucionAntorchas");
    }
}