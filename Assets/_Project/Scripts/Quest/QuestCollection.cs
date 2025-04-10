using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    [System.Serializable]
    public struct QuestResources
    {
        public int Gold;
        public int Food;
        public int PeopleSatisfaction;
        public int CastleStrength;
    }

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
        public QuestResources RewardResources;
        public Quest(int id, string questInfo, bool isDone, QuestResources resources)
        {
            Id = id;
            QuestInfo = questInfo;
            IsDone = isDone;
            RewardResources = resources;
        }

        public void ChangeDone(bool isDone)
        {
            if (isDone && !IsDone) 
            {
                GameResources.Resources.ChangeGold(RewardResources.Gold);
                GameResources.Resources.ChangeFood(RewardResources.Food);
                GameResources.Resources.ChangePeopleSatisfaction(RewardResources.PeopleSatisfaction);
                GameResources.Resources.ChangeCastleStrength(RewardResources.CastleStrength);
            }
            IsDone = isDone;
        }
    }
}
