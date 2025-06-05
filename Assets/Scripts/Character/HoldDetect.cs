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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            characterController.IsMouseOverCharacter = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            characterController.IsMouseOverCharacter = false;
        }
    }

}
