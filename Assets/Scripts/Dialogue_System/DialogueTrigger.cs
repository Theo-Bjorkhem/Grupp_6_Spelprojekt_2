using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
    public Dialogue dialogue2;

    public void TriggerDialogue2()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue2);
    }
}
