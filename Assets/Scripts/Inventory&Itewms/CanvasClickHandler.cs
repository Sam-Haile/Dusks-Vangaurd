using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Inventory inventory;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the clicked object is an item slot
        if (eventData.pointerCurrentRaycast.gameObject == null ||
            !eventData.pointerCurrentRaycast.gameObject.CompareTag("ItemSlot"))
        {
            inventory.DeactivateButtons();
        }
    }

    public void OnCanvasClick(BaseEventData baseEventData)
    {
        PointerEventData eventData = baseEventData as PointerEventData;

        if (eventData == null) return;

        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        // Check if the clicked object is an item slot or an empty item slot
        if (clickedObject == null)
        {
            inventory.DeactivateButtons();
            return;
        }

        if (clickedObject.CompareTag("ItemSlot"))
        {
            // Might have some logic here when a valid item slot is clicked.
            return;
        }
    }
}
