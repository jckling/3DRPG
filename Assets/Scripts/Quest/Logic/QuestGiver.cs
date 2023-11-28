using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    private DialogueController dialogueController;
    private QuestData_SO currentQuest;

    public DialogueData_SO startDialogue;
    public DialogueData_SO progressDialogue;
    public DialogueData_SO completeDialogue;
    public DialogueData_SO finishDialogue;

    #region Quest State

    private bool IsStarted => QuestManager.Instance.HaveQuest(currentQuest) &&
                              QuestManager.Instance.GetTask(currentQuest).IsStarted;

    private bool IsCompleted => QuestManager.Instance.HaveQuest(currentQuest) &&
                                QuestManager.Instance.GetTask(currentQuest).IsCompleted;

    private bool IsFinished => QuestManager.Instance.HaveQuest(currentQuest) &&
                               QuestManager.Instance.GetTask(currentQuest).IsFinished;

    #endregion

    #region Event Functions

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
    }

    private void Start()
    {
        dialogueController.currentData = startDialogue;
        currentQuest = dialogueController.currentData.GetQuest();
    }

    private void Update()
    {
        if (IsStarted)
        {
            dialogueController.currentData = IsCompleted ? completeDialogue : progressDialogue;
        }

        if (IsFinished)
        {
            dialogueController.currentData = finishDialogue;
        }
    }

    #endregion
}