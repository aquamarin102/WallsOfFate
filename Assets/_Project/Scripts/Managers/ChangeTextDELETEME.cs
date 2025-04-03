using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Assets._Project.Scripts.TriggerOjects;

namespace Assets._Project.Scripts.Managers
{
    internal class ChangeTextDELETEME : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textField;
        private string _newText = "Узнать у гонца новости";

        private void Update()
        {
            if (DataBetweenLocations.ForgePerfom)
            {
                _textField.text = _newText;
            }
        }
    }
}
