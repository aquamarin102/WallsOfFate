using System;

namespace GamePlayerStats {
    public static class PlayerStats {
        public static int Strength {get; set; }
        public static int Dex { get; set; }
        public static int Percept { get; set; }
        public static int Mystic { get; set; }

        public static void ChangeStrength(int delta) { Strength = Math.Max(Strength + delta, 0); }

        public static void ChangeDex(int delta) { Dex = Math.Max(Dex + delta, 0); }

        public static void ChangePerceept(int delta) { Percept = Math.Max(Percept + delta, 0); }

        public static void ChangeMystic(int delta) { Mystic = Math.Max(Mystic + delta, 0); }
    }
}
