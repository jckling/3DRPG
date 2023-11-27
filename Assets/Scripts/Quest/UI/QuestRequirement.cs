using TMPro;
using UnityEngine;

public class QuestRequirement : MonoBehaviour
{
    private TextMeshProUGUI requireName;
    private TextMeshProUGUI progressNumber;

    private void Awake()
    {
        requireName = GetComponent<TextMeshProUGUI>();
        progressNumber = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetUpRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = $"{currentAmount}/{amount}";
    }
}