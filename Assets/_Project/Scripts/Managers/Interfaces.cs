using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    void Trrigered();

}

// Дополнительный интерфейс для проверки состояния
public interface ICheckableTrigger : ITriggerable
{
    bool IsDone { get; }
}