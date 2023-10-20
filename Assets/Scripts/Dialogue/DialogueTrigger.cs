using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    public Dialogue dialogue;
    public DialogueManager manager;
    public bool isTalking;

    private void Start()
    {
        isTalking = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.F) && !manager.isActive)
            {
                isTalking = true;
                manager.StartDialogue(dialogue);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTalking = false;
            manager.EndDialogue();
        }
    }

}