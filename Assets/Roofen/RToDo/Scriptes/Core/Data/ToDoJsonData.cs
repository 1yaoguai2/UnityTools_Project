#region

using System;
using System.Collections.Generic;

#endregion

namespace RGame.RToDo
{
    [Serializable]
    public class ToDoJsonData
    {
        public List<TaskJsonData> mTasks = new();
    }

    [Serializable]
    public class TaskJsonData
    {
        public string Description;
        public string Deadline;
        public bool IsCompleted;
    }
}