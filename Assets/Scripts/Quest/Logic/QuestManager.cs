using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;

        public bool IsStarted
        {
            get => questData.isStarted;
            set => questData.isStarted = value;
        }

        public bool IsCompleted
        {
            get => questData.isCompleted;
            set => questData.isCompleted = value;
        }

        public bool IsFinished
        {
            get => questData.isFinished;
            set => questData.isFinished = value;
        }
    }

    public List<QuestTask> tasks = new List<QuestTask>();

    #region Event Functions

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LoadQuestData();
    }

    #endregion

    public bool HaveQuest(QuestData_SO data)
    {
        return data != null && tasks.Any(task => task.questData.questName == data.questName);
    }

    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(task => task.questData.questName == data.questName);
    }

    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in tasks)
        {
            if (task.IsFinished)
            {
                continue;
            }

            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if (matchTask != null)
            {
                matchTask.currentAmount += amount;
            }

            task.questData.CheckQuestProgress();
        }
    }

    public void SaveQuestData()
    {
        PlayerPrefs.SetInt("QuestCount", tasks.Count);
        for (var i = 0; i < tasks.Count; i++)
        {
            SaveManager.Instance.Save(tasks[i].questData, "task" + i);
        }
    }

    public void LoadQuestData()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");
        for (var i = 0; i < questCount; i++)
        {
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            SaveManager.Instance.Load(newQuest, "task" + i);
            tasks.Add(new QuestTask() { questData = newQuest });
        }
    }
}