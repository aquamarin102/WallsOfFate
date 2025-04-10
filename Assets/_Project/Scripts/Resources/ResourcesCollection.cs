using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameResources
{
    public static class Resources
    {
        public static int Gold { get; private set; }
        public static int Food { get; private set; }
        public static int PeopleSatisfaction { get; private set; }
        public static int CastleStrength { get; private set; }
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
