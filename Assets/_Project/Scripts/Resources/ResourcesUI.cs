using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameResources
{
    [RequireComponent(typeof(ResourcesUI))]
    public class ResourcesUI : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text foodText;
        [SerializeField] private TMP_Text satisfactionText;
        [SerializeField] private TMP_Text strengthText;

        private int lastGold;
        private int lastFood;
        private int lastSatisfaction;
        private int lastStrength;

        private void Start()
        {
            // Первоначальное обновление UI
            UpdateAllResources(true);
        }

        private void Update()
        {
            DialogueManager dialogeManager = DialogueManager.GetInstance();
            int gold = ((Ink.Runtime.IntValue)dialogeManager.GetVariablesState("Gold")).value;
            int food = ((Ink.Runtime.IntValue)dialogeManager.GetVariablesState("Food")).value;
            int peopleSatisfaction = ((Ink.Runtime.IntValue)dialogeManager.GetVariablesState("PeopleSatisfaction")).value;
            int castleStrength = ((Ink.Runtime.IntValue)dialogeManager.GetVariablesState("CastleStrength")).value;

            if (!dialogeManager.DialogueIsPlaying)
            {
                GameResources.ChangeGold(gold);
                GameResources.ChangeFood(food);
                GameResources.ChangePeopleSatisfaction(peopleSatisfaction);
                GameResources.ChangeCastleStrength(castleStrength);
                dialogeManager.SetVariableState("Gold", 0);
                dialogeManager.SetVariableState("Food", 0);
                dialogeManager.SetVariableState("PeopleSatisfaction", 0);
                dialogeManager.SetVariableState("CastleStrength", 0);

            }
            // Проверяем изменения каждый кадр
            UpdateAllResources();
        }

        private void UpdateAllResources(bool forceUpdate = false)
        {
            UpdateResource(ref lastGold, GameResources.Gold, goldText, forceUpdate);
            UpdateResource(ref lastFood, GameResources.Food, foodText, forceUpdate);
            UpdateResource(ref lastSatisfaction,
                          GameResources.PeopleSatisfaction,
                          satisfactionText,
                          forceUpdate);
            UpdateResource(ref lastStrength,
                          GameResources.CastleStrength,
                          strengthText,
                          forceUpdate);
        }

        private void UpdateResource(ref int lastValue,
                                  int currentValue,
                                  TMP_Text textField,
                                  bool forceUpdate)
        {
            if (textField == null) return;

            if (forceUpdate || lastValue != currentValue)
            {
                textField.text = currentValue.ToString();
                lastValue = currentValue;
            }
        }

        // Валидация в редакторе
        private void OnValidate()
        {
            if (goldText == null)
                Debug.LogWarning("Gold Text not assigned!", this);
            if (foodText == null)
                Debug.LogWarning("Food Text not assigned!", this);
            if (satisfactionText == null)
                Debug.LogWarning("Satisfaction Text not assigned!", this);
            if (strengthText == null)
                Debug.LogWarning("Strength Text not assigned!", this);
        }
    }
}
