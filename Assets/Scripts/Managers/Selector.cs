using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    public GameObject pointerGraphic;
    public float xOffset = 10f; // Adjust this to position the pointer relative to the button

    public void ShowPointer(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData != null)
        {
            pointerGraphic.SetActive(true);

            // Get the button's position
            Vector3 buttonPosition = pointerData.pointerEnter.transform.position;

            // Set the pointer's position to match the button's y-axis
            pointerGraphic.transform.position = new Vector3(pointerGraphic.transform.position.x + xOffset, buttonPosition.y, pointerGraphic.transform.position.z);
        }
    }

    public void HidePointer()
    {
        pointerGraphic.SetActive(false);
    }

}
