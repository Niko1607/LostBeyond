using UnityEngine;
using System.Collections.Generic;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance; 

    public List<int> collectedClueNumbers = new List<int>(); 

    void Awake()
    {
        Debug.Log("GameProgressManager Awake() INICIADO en la escena: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameProgressManager INSTANCE CREADA Y NO SE DESTRUIRÁ."); 
        }
        else
        {
            Debug.LogWarning("GameProgressManager: Ya existe una instancia. Destruyendo este duplicado."); 
            Destroy(gameObject);
        }
    }
    public void AddClue(int clueNumber)
    {
        if (!collectedClueNumbers.Contains(clueNumber)) 
        {
            collectedClueNumbers.Add(clueNumber);
            Debug.Log("Pista añadida: " + clueNumber);
        }
    }

    public List<int> GetCollectedClues()
    {
        return new List<int>(collectedClueNumbers); 
    }

    public void ClearClues()
    {
        collectedClueNumbers.Clear();
        Debug.Log("Pistas borradas.");
    }
}