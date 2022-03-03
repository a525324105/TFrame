using System;
using System.Collections;
using System.Collections.Generic;
using ECS;
using UnityEngine;

namespace ECS
{
    [Serializable]
    public class ECSGameSystem : ECSSystem
    {
        public void OnUpdate()
        {
            Update();
        }
    }
}
