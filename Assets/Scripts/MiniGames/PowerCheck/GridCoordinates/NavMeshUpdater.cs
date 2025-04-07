using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.AI.Navigation.Samples;
using UnityEngine;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    internal class NavMeshUpdater : MonoBehaviour
    {
        private void FixedUpdate()
        {
            GloballyUpdatedNavMeshSurface.RequestNavMeshUpdate();
        }
    }
}
