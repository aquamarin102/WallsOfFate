using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMine : Mine
{
    public DamageMine(uint number, float ñooldown, GameObject mine) : base(number, ñooldown, mine) {}

    public void Damage(Player player1, Player player2)
    {
        player1.TakeDamage(player2.GetDamage());
    }
}
