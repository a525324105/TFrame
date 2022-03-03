using UnityEngine;

namespace ECS
{
    public class ECSInputComponent : ECSComponent, IUpdate
    {
        public float Horizontal { private set; get; }
        public float Vertical { private set; get; }
        public void Update()
        {
            Horizontal = Input.GetAxis("Horizontal");

            Vertical = Input.GetAxis("Vertical");
        }
    }
}