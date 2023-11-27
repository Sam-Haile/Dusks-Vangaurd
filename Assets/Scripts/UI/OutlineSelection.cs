using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{

    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    public Unit selectedUnit; // This will hold the selected Unit
    public bool canSelect;

    void Update()
    {
        if (canSelect)
        {

            // Highlight
            if (highlight != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = false;
                highlight = null;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) 
            {
                highlight = raycastHit.transform;
                if (highlight.gameObject.GetComponent<Unit>() != null && highlight != selection)
                {
                    if (highlight.gameObject.GetComponent<Outline>() != null)
                    {
                        highlight.gameObject.GetComponent<Outline>().enabled = true;
                    }
                    else
                    {
                        Outline outline = highlight.gameObject.AddComponent<Outline>();
                        outline.enabled = true;
                        highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.blue;
                        highlight.gameObject.GetComponent<Outline>().OutlineWidth = 1.0f;
                    }
                }
                else
                {
                    highlight = null;
                }
            }

            // Selection
            if (Input.GetMouseButtonDown(0))
            {
                if (highlight)
                {
                    if (selection != null)
                    {
                        selection.gameObject.GetComponent<Outline>().enabled = false;
                    }

                    selection = raycastHit.transform;
                    selectedUnit = selection.gameObject.GetComponent<Unit>(); // Here you get the Unit component
                }
                else
                {
                    if (selection)
                    {
                        selection.gameObject.GetComponent<Outline>().enabled = false;
                        selection = null;
                    }
                }
            }

        }

    }
}
