using System;
using System.Collections.Generic;
using System.Linq;

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
            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if (matchTask != null)
            {
                matchTask.currentAmount += amount;
            }

            task.questData.CheckQuestProgress();
        }
    }
}