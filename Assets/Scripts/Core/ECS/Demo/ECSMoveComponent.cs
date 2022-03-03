using UnityEngine;

namespace ECS
{
    public class ECSMoveComponent : ECSComponent, IUpdate
    {
        public ECSInputComponent EcsInputComponent;

        public ECSActor EcsActor;

        public float Speed = 5f;
        public override void Awake()
        {
            base.Awake();

            EcsInputComponent = Entity.GetComponent<ECSInputComponent>();
            
            EcsActor = Entity.GetComponent<ECSActor>();
        }

        public void Update()
        {
            if (EcsInputComponent == null || EcsActor == null)
            {
                return;
            }

            EcsActor.transform.Translate(EcsInputComponent.Horizontal * Time.deltaTime * Speed, EcsInputComponent.Vertical * Time.deltaTime * Speed, 0);
        }
    }
}
