using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public static class QuestCollection
    {
        private static List<Quest> QuestList = new List<Quest>();

        public static Quest GetQuestById(int id) => QuestList.FirstOrDefault(q => q.Id == id);
        public static Quest GetFirstNotDoneQuest() =>
            QuestList.FirstOrDefault(q => !q.IsDone);
        public static void AddQuest(Quest quest) => QuestList.Add(quest);
        public static void ClearQuests() => QuestList.Clear();
        public static List<Quest> GetAllQuests() => new List<Quest>(QuestList);

    }

    public class Quest
    {
        public int Id;
        public string QuestInfo;
        public bool IsDone;

        public Quest(int id, string questInfo, bool isDone)
        {
            Id = id;
            QuestInfo = questInfo;
            IsDone = isDone;
        }

        public void ChangeDone(bool isDone) => IsDone = isDone;
    }
}
