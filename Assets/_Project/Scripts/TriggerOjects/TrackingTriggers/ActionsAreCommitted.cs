using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActionsAreCommitted : MonoBehaviour
{
    private ICheckableTrigger _checkableTrigger;
    public bool IsDone { get; private set; }

    private void Awake()
    {
        _checkableTrigger = GetComponent<ICheckableTrigger>();
        if (_checkableTrigger == null)
        {
            Debug.LogError($"No component implementing ICheckableTrigger found on {gameObject.name}", this);
        }
    }

    private void Update()
    {
        if (_checkableTrigger != null)
        {
            IsDone = _checkableTrigger.IsDone;
        }
    }
}
