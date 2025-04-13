using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameResources
{
    public static class GameResources
    {
        public static int Gold { get; set; }
        public static int Food { get; set; }
        public static int PeopleSatisfaction { get; set; }
        public static int CastleStrength { get; set; }
        public static void ChangeGold(int delta)
        {
            Gold = Math.Max(Gold + delta, 0);
        }

        public static void ChangeFood(int delta)
        {
            Food = Math.Max(Food + delta, 0);
        }

        public static void ChangePeopleSatisfaction(int delta)
        {
            PeopleSatisfaction = Math.Max(PeopleSatisfaction + delta, 0);
        }

        public static void ChangeCastleStrength(int delta)
        {
            CastleStrength = Math.Max(CastleStrength + delta, 0);
        }
    }    
}
