using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Dialogue Elements")] public Image icon;
    public Text text;
    public Button nextButton;
    public GameObject dialoguePanel;

    [Header("Option Elements")] public RectTransform optionPanel;
    public OptionUI optionPrefab;

    [Header("Dialogue Data")] public DialogueData_SO currentData;
    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    private void ContinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)
        {
            UpdateDialogue(currentData.dialoguePieces[currentIndex]);
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void UpdateDialogueData(DialogueData_SO dialogueData)
    {
        currentData = dialogueData;
        currentIndex = 0;
    }

    public void UpdateDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;

        // icon
        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }

        // text
        text.text = "";
        text.DOText(piece.text, 1f);

        // next button
        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextButton.interactable = true;
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.interactable = false;
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        // option
        CreateOptions(piece);
    }

    private void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)
        {
            for (var i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        foreach (var dialogueOption in piece.options)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, dialogueOption);
        }
    }
}