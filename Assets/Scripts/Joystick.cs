using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;          

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias UI")]
    public Image joystickBackground; 
    public Image joystickKnob;       

    [Header("Propiedades del Joystick")]
    public float joystickRange = 75f; 

    public Vector2 Direction { get; private set; } 

    public Vector2 initialKnobPosition; 

    void Start()
    {
        if (joystickBackground == null || joystickKnob == null)
        {
            Debug.LogError("Joystick: ¡Faltan asignaciones de UI en el Inspector!");
            enabled = false; 
            return;
        }
        initialKnobPosition = joystickKnob.rectTransform.anchoredPosition;
        Direction = Vector2.zero; 
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        
        OnDrag(eventData);
    }

   
    public void OnDrag(PointerEventData eventData)
    {
        if (joystickKnob == null) return;

        Vector2 touchPosition;
        
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.rectTransform,
            eventData.position,
            eventData.pressEventCamera, 
            out touchPosition
        );

        
        Vector2 clampedPosition = Vector2.ClampMagnitude(touchPosition, joystickRange);

        joystickKnob.rectTransform.anchoredPosition = clampedPosition; 

        
        Direction = clampedPosition.normalized;
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (joystickKnob != null)
        {
            joystickKnob.rectTransform.anchoredPosition = initialKnobPosition;
        }
        Direction = Vector2.zero;
    }
}