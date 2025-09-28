using TMPro;
using UnityEngine;

namespace GamePlayerStats {
    public class StatsController : MonoBehaviour {
        [SerializeField] private int StatsAmount;

        [SerializeField] private TMP_Text StatsPool;
        [SerializeField] private TMP_Text Strength;
        [SerializeField] private TMP_Text Int;
        [SerializeField] private TMP_Text Dex;
        [SerializeField] private TMP_Text Percept;
        [SerializeField] private TMP_Text Mystic;

        [SerializeField] private GameObject ConfirmatiionButton;

        void Start() {
            PlayerStats.OnStrengthChanged += UpdateStrengthUI;
            PlayerStats.OnDexChanged += UpdateIntUI;
            PlayerStats.OnDexChanged += UpdateDexUI;
            PlayerStats.OnPerceptChanged += UpdatePerceptUI;
            PlayerStats.OnMysticChanged += UpdateMysticUI;

            UpdateAllStatsUI();
        }

        void OnDestroy() {
            PlayerStats.OnStrengthChanged -= UpdateStrengthUI;
            PlayerStats.OnDexChanged -= UpdateIntUI;
            PlayerStats.OnDexChanged -= UpdateDexUI;
            PlayerStats.OnPerceptChanged -= UpdatePerceptUI;
            PlayerStats.OnMysticChanged -= UpdateMysticUI;
        }

        private void UpdateStrengthUI(int newValue) {
            if (Strength != null)
                Strength.text = $"Strength: {newValue}";
        }
        private void UpdateIntUI(int newValue) {
            if (Int != null)
                Int.text = $"Int: {newValue}";
        }
        private void UpdateDexUI(int newValue) {
            if (Dex != null)
                Dex.text = $"Dex: {newValue}";
        }
        private void UpdatePerceptUI(int newValue) {
            if (Percept != null)
                Percept.text = $"Percept: {newValue}";
        }
        private void UpdateMysticUI(int newValue) {
            if (Mystic != null)
                Mystic.text = $"Mystic: {newValue}";
        }

        private void UpdateAllStatsUI() {
            UpdateStrengthUI(PlayerStats.Strength);
            UpdateIntUI(PlayerStats.Int);
            UpdateDexUI(PlayerStats.Dex);
            UpdatePerceptUI(PlayerStats.Percept);
            UpdateMysticUI(PlayerStats.Mystic);
            UpdateStatsPoolUI();
        }

        private void ActivateConfirmationButton() {
            if (StatsAmount == 0) ConfirmatiionButton.SetActive(true);
            else ConfirmatiionButton.SetActive(false);
        }

        private void UpdateStatsPoolUI() {
            if (StatsPool != null) {
                StatsPool.text = $"Amount: {StatsAmount}";
            }
        }

        public void IncreaceStrength() {
            if (StatsAmount > 0) {
                StatsAmount--;
                PlayerStats.ChangeStrength(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceInt() { 
            if (StatsAmount > 0) {
                StatsAmount--;
                PlayerStats.ChangeInt(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceDex() { 
            if (StatsAmount > 0) {
                StatsAmount--;
                PlayerStats.ChangeDex(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreacePerceept() { 
            if (StatsAmount > 0) {
                StatsAmount--;
                PlayerStats.ChangePerceept(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceMystic() { 
            if (StatsAmount > 0)  {
                StatsAmount--;
                PlayerStats.ChangeMystic(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }     
        
        public void DecreaceStrength() { 
            if (StatsAmount >= 0) {
                if(PlayerStats.Strength != 0) StatsAmount++;
                PlayerStats.ChangeStrength(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceInt() {
            if(StatsAmount >= 0) {
                if (PlayerStats.Int != 0) StatsAmount++;
                PlayerStats.ChangeInt(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceDex() {
            if(StatsAmount >= 0) {
                if (PlayerStats.Dex != 0) StatsAmount++;
                PlayerStats.ChangeDex(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreacePerceept() {
            if (StatsAmount >= 0) {
                if (PlayerStats.Percept != 0) StatsAmount++;
                PlayerStats.ChangePerceept(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceMystic() {
            if (StatsAmount >= 0) {
                if (PlayerStats.Mystic != 0) StatsAmount++;
                PlayerStats.ChangeMystic(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
    }
}