using System.Collections;
using System.Collections.Generic;
using ECS;
using UnityEngine;

namespace ECS
{
    public class ECSDemoApp : MonoBehaviour
    {
        public ECSGameSystem EcsGameSystem = new ECSGameSystem();
        public GameObject @object;

        void Start()
        {
            var entity = EcsGameSystem.Create<Entity>();
            ECSActor actor = entity.AddComponent<ECSActor>();
            actor.Name = typeof(ECSActor).ToString();
            actor.gameObject = Instantiate(@object);
            actor.transform = actor.gameObject.GetComponent<Transform>();
            entity.AddComponent<ECSInputComponent>();
            entity.AddComponent<ECSMoveComponent>();
            entity.CheckDebugInfo(actor.gameObject);
            Debug.Log(entity.ToString());
        }

        void Update()
        {
            EcsGameSystem.OnUpdate();
        }
    }
}

