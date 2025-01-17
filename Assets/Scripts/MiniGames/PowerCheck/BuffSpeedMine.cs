using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BuffSpeedMine : Mine
{
    private float SpeedBuff;
    private float BuffCooldown;
    private int TimeBeforeExplosion;
    private float MaxRadius;

    public BuffSpeedMine(uint number, float сooldown, GameObject mine, float speedbuff, float buffcooldown, int timebeforeexplosion, float radius) : base(number, сooldown, mine)
    {
        this.SpeedBuff = speedbuff;
        this.BuffCooldown = buffcooldown;
        this.TimeBeforeExplosion = timebeforeexplosion;
        this.MaxRadius = radius;
    }

    public float GetSpeedBuff()
    {
        return this.SpeedBuff;
    }
    public float GetBuffCooldown()
    {
        return this.BuffCooldown;
    }
    public int GetTimeBeforeExplosion()
    {
        return this.TimeBeforeExplosion;
    }

   public async Task BuffSpeed(Player player)
{
    player.TakeSpeedboost(this.SpeedBuff); // Применяем начальный бафф
    await Task.Delay((int)(this.BuffCooldown * 1000)); // Ждём указанное время (в миллисекундах)
    player.TakeSpeedboost(1 / this.SpeedBuff); // Убираем бафф
}

    public async Task BuffSpeedList(List<Player> players)
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                await BuffSpeed(player);
            }
        }
    }

    public List<Player> FindDistanceToMine(params GameObject[] positions)
    {
        Vector3 minePosition = this.MineGameObject.transform.position;
        List<Player> closeObjects = new List<Player>();

        foreach (var obj in positions)
        {
            if (obj != null) // Проверяем, что объект не null
            {
                float distance = Vector3.Distance(minePosition, obj.transform.position);
                if (distance <= this.MaxRadius)
                {
                    Player objChar = obj.GetComponent<Player>();
                    if(objChar != null) closeObjects.Add(objChar);
                }
                Debug.Log($"Distance to {obj.name}: {distance}");
            }
            else
            {
                Debug.LogWarning("One of the passed GameObjects is null.");
            }
        }

        return closeObjects;
    }  

}
