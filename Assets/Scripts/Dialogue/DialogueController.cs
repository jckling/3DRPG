using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
    private bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void Update()
    {
        // right click
        if (canTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateDialogue(currentData.dialoguePieces[0]);
    }
}