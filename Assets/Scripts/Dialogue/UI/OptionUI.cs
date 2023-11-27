using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Button thisButton;
    private DialoguePiece currentPiece;
    private string nextPieceID;


    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        text.text = option.text;
        nextPieceID = option.targetID;
    }

    private void OnOptionClicked()
    {
        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }
        else
        {
            DialogueUI.Instance.UpdateDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}