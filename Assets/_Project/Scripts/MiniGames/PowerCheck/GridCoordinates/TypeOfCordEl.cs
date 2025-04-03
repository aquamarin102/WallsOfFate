using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    public enum TypeOfCordEl
    {
        NotOccupied = 0,       // Не занято
        Border = 1,            // Оглушающая мина/игрок
        Heal = 2,              // Хилка
        Damage = 3,            // Урон
        SpeedBuff = 4,         // Скоростной баф
        Path = 5               // Дорога для алгоритмя A*
    }
}
