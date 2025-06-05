using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDetect : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    CharacterController characterController;
    void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("down");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            characterController.IsMouseOverCharacter = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("up");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            characterController.IsMouseOverCharacter = false;
        }
    }

}
