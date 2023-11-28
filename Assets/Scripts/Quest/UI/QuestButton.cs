using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public QuestData_SO currentData;

    #region Event Functions

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    #endregion

    private void UpdateQuestContent()
    {
        QuestUI.Instance.SetUpRequireList(currentData);

        foreach (Transform item in QuestUI.Instance.rewardList)
        {
            Destroy(item.gameObject);
        }

        foreach (var reward in currentData.rewards)
        {
            QuestUI.Instance.SetUpRewardItem(reward.itemData, reward.amount);
        }
    }

    public void SetUpQuestButton(QuestData_SO questData)
    {
        currentData = questData;
        if (questData.isCompleted)
        {
            questName.text = questData.questName + " (Completed)";
        }
        else
        {
            questName.text = questData.questName;
        }
    }
}