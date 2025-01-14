using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMine : Mine
{
    public HealMine(uint number, float ñooldown, GameObject mine) : base(number, ñooldown, mine) {}

    public void Heal(Player player)
    {
        player.TakeHeal();
    }
}
