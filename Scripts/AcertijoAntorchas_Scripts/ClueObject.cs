using UnityEngine;
using TMPro; 

public class ClueObject : MonoBehaviour
{
    public int clueValue; 
    public bool canBeCollectedMultipleTimes = false; 
   

    private bool hasBeenCollected = false;
    private SpriteRenderer spriteRenderer; 
    private Collider2D objectCollider;    

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
      
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!hasBeenCollected || canBeCollectedMultipleTimes)
            {
                if (GameProgressManager.Instance != null)
                {
                    GameProgressManager.Instance.AddClue(clueValue);
                    Debug.Log("Jugador recolectó pista: " + clueValue + " desde el objeto: " + gameObject.name);
                    hasBeenCollected = true;

                    
                    //if (spriteRenderer != null)
                   // {
                   //     spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // 50% de transparencia
                  //  }
                    
                    if (objectCollider != null)
                    {
                        objectCollider.enabled = false;
                    }
                    

                }
                else
                {
                    Debug.LogError("GameProgressManager no encontrado al intentar recolectar pista: " + clueValue);
                }
            }
        }
    }
}