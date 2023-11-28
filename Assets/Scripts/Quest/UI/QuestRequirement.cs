using TMPro;
using UnityEngine;

public class QuestRequirement : MonoBehaviour
{
    private TextMeshProUGUI requireName;
    private TextMeshProUGUI progressNumber;

    #region Event Functions

    private void Awake()
    {
        requireName = GetComponent<TextMeshProUGUI>();
        progressNumber = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    #endregion

    public void SetUpRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = $"{currentAmount}/{amount}";
    }

    public void SetUpRequirement(string name, int amount)
    {
        requireName.text = name;
        requireName.color = Color.gray;

        progressNumber.text = $"<s>{amount}/{amount}</s>";
        progressNumber.color = Color.gray;
    }
}