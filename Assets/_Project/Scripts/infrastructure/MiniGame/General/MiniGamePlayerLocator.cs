using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.infrastructure
{
    internal class MiniGamePlayerLocator : MonoInstaller
    {
        [Header("For instantiaie prefab")]
        public Transform StartPoint;
        public GameObject Prefab;
        public Transform Parent;
        public Transform CameraTransform;
        [Header("HPBar")]
        public Transform HPBarParent;
        public Slider HPBarSlider;

        public override void InstallBindings()
        {
            BindCameraTransform();
            InstantiateMainCharacter();
        }

        private void InstantiateMainCharacter()
        {
            if (Prefab == null)
            {
                Debug.LogError("Prefab не назначен в инспекторе!", this);
                return;
            }


            PlayerMove playerMove = Container
                .InstantiatePrefabForComponent<PlayerMove>(Prefab, StartPoint.position, Prefab.transform.rotation, Parent);

            Container
                .Bind<PlayerMove>()
                .FromInstance(playerMove)
                .AsSingle();

            // Выполняем инъекцию вручную, так как объект уже был создан
            Container.Inject(playerMove);
        }

        private void BindCameraTransform()
        {
            Container
                .Bind<Transform>()
                .FromInstance(CameraTransform)
                .WhenInjectedInto<PlayerMove>();
        }
    }
}
