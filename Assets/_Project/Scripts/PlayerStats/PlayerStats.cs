using System;

namespace GamePlayerStats {
    public static class PlayerStats {
        private static int _strength;
        private static int _int;
        private static int _dex;
        private static int _percept;
        private static int _mystic;

        public static int Strength {
            get => _strength;
            set {
                if (_strength != value) {
                    _strength = Math.Max(value, 0);
                    OnStrengthChanged?.Invoke(_strength);
                }
            }
        }

        public static int Int {
            get => _int;
            set {
                if (_int != value) {
                    _int = Math.Max(value, 0);
                    OnIntChanged?.Invoke(_int);
                }
            }
        }

        public static int Dex {
            get => _dex;
            set {
                if (_dex != value) {
                    _dex = Math.Max(value, 0);
                    OnDexChanged?.Invoke(_dex);
                }
            }
        }

        public static int Percept {
            get => _percept;
            set {
                if (_percept != value) {
                    _percept = Math.Max(value, 0);
                    OnPerceptChanged?.Invoke(_percept);
                }
            }
        }

        public static int Mystic {
            get => _mystic;
            set {
                if (_mystic != value) {
                    _mystic = Math.Max(value, 0);
                    OnMysticChanged?.Invoke(_mystic);
                }
            }
        }

        public static event Action<int> OnStrengthChanged;
        public static event Action<int> OnIntChanged;
        public static event Action<int> OnDexChanged;
        public static event Action<int> OnPerceptChanged;
        public static event Action<int> OnMysticChanged;

        public static void ChangeStrength(int delta) { Strength = Math.Max(Strength + delta, 0); }

        public static void ChangeInt(int delta) { Int = Math.Max(Int + delta, 0); }
        public static void ChangeDex(int delta) { Dex = Math.Max(Dex + delta, 0); }

        public static void ChangePerceept(int delta) { Percept = Math.Max(Percept + delta, 0); }

        public static void ChangeMystic(int delta) { Mystic = Math.Max(Mystic + delta, 0); }
    }
}
