#region

using System;

#endregion

namespace RGame.RToDo
{
    [Serializable]
    public class TaskData
    {
        public string Description;
        public bool IsCompleted;
        public DateTime Deadline;
    }
}