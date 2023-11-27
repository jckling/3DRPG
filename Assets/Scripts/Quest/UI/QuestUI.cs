using TMPro;
using UnityEngine;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Basic UI Elements")] public GameObject questPanel;
    public ItemTooltip tooltip;
    private bool isOpen;

    [Header("Quest")] public RectTransform questListContent;
    public QuestButton questButton;
    public TextMeshProUGUI questContentDescription;

    [Header("Requirement")] public RectTransform requireList;
    public QuestRequirement requirement;

    [Header("Reward")] public RectTransform rewardList;
    public ItemUI reward;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContentDescription.text = string.Empty;

            SetUpQuestList();

            if (!isOpen)
            {
                tooltip.gameObject.SetActive(false);
            }
        }
    }

    private void SetUpQuestList()
    {
        // Destroy Objects
        foreach (Transform questButton in questListContent)
        {
            Destroy(questButton.gameObject);
        }

        foreach (Transform requirement in requireList)
        {
            Destroy(requirement.gameObject);
        }

        foreach (Transform reward in rewardList)
        {
            Destroy(reward.gameObject);
        }

        // Load Quests
        foreach (var task in QuestManager.Instance.tasks)
        {
            var newTask = Instantiate(questButton, questListContent);
            newTask.SetUpQuestButton(task.questData);
        }
    }

    public void SetUpRequireList(QuestData_SO questData)
    {
        questContentDescription.text = questData.description;

        foreach (Transform requirement in requireList)
        {
            Destroy(requirement.gameObject);
        }

        foreach (var require in questData.questRequires)
        {
            var newRequire = Instantiate(requirement, requireList);
            newRequire.SetUpRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetUpRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(reward, rewardList);
        item.SetUpItemUI(itemData, amount);
    }
}