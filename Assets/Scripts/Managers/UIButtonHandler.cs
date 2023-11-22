using UnityEngine;
using UnityEngine.EventSystems; // Required for event handling

public class UIButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject[] uiElement; // The UI element to show/hide
    private bool isClicked = false; // Flag to track if the button has been clicked

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show UI on hover
        foreach (var item in uiElement)
            item.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide UI on hover out only if it hasn't been clicked
        if (!isClicked)
        {
            foreach (var item in uiElement)
                item.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Toggle the clicked state
        isClicked = true;

        // Optionally hide or show the UI based on the new state
        foreach (var item in uiElement)
            item.SetActive(isClicked);
    }

    // Call this method to manually reset the clicked state, e.g., when another UI element is interacted with
    public void ResetClickState()
    {
        isClicked = false;
        foreach (var item in uiElement)
            item.SetActive(false);
    }
}
