using System;
using System.Collections.Generic;
using System.Threading.Tasks;


/* Author - AlexTang
 * Date   - 2022/3/3
 * E-- Entity 实体，本质上是存放组件的容器
 * C -- Component 组件，游戏所需的所有数据结构
 * S -- System 系统，根据组件数据处理逻辑状态的管理器
 * Component 组件只能存放数据，不能实现任何处理状态相关的函数
 * System系统不可以自己去记录维护任何状态
 * 说的通俗点，就是组件放数据，系统来处理。这么做的好处，就是为了尽可能地让数据与逻辑进行解耦
 * 一个良好的数据结构设计，也会以增加CPU缓存命中的形式来提升性能表现
 *
 * 举个例子，常见的组件包括而不仅限于:
 * 渲染组件 ：模型的顶点、材质等数据，保证我们能正确地渲染到world中
 * 位置组件 ：记录着实体在这个world的真实位置
 * 特效组件 ：不同的时机，可能会需要播放不同的粒子特效以增强视觉感受
 */

namespace ECS
{
    /// <summary>
    /// ECS系统 管理Entity、ECSComponent复用对象池
    /// </summary>
    [Serializable]
    public class ECSSystem : IDisposable
    {
        private static ECSSystem instance = new ECSSystem();
        public static ECSSystem Instance => instance;
        private readonly Dictionary<int,Stack<ECSObject>> m_ObjectPool = new Dictionary<int, Stack<ECSObject>>();
        internal readonly ArrayPool<Entity> Entities = new ArrayPool<Entity>();
        private bool m_IsDispose = false;

        public void AddEntity(Entity entity)
        {
            entity.System = this;
            entity.Awake();
            Entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);
        }

        /// <summary>
        /// Get Object From ECSSystem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : ECSObject, new()
        {
            int type = typeof(T).GetHashCode();
            if (m_ObjectPool.TryGetValue(type,out Stack<ECSObject> stack))
            {
                if (stack.Count > 0)
                {
                    return (T) stack.Pop();
                }
                goto Instantiate;
            }
            stack = new Stack<ECSObject>();
            m_ObjectPool.Add(type,stack);
            Instantiate: T ecsObject = new T();
            return ecsObject;
        }

        /// <summary>
        /// Push Object To ECSSystem
        /// </summary>
        public void Push(ECSObject ecsObject)
        {
            int type = ecsObject.HashCode;

            if (m_ObjectPool.TryGetValue(type,out Stack<ECSObject> stack))
            {
                stack.Push(ecsObject);
                return;
            }
            stack = new Stack<ECSObject>();
            m_ObjectPool.Add(type,stack);
            stack.Push(ecsObject);
        }

        public T Create<T>() where T : Entity, new()
        {
            T entity = Get<T>();
            AddEntity(entity);
            return entity;
        }

        public T Create<T>(T entity) where T : Entity, new()
        {
            AddEntity(entity);
            return entity;
        }

        /// <summary>
        /// 更新ECS系统
        /// </summary>
        /// <param name="worker">线程池是否并行</param>
        public void Update(bool worker = false)
        {
            Run(worker);
        }

        /// <summary>
        /// 运行ECS系统
        /// </summary>
        /// <param name="worker">线程池是否并行</param>
        public void Run(bool worker = false)
        {
            int count = Entities.Count;
            if (!worker)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!Entities.Buckets[i])
                    {
                        continue;
                    }

                    if (!Entities[i].CanUpdate)
                    {
                        continue;
                    }
                    Entities[i].Execute();
                }
            }
            else
            {
                Parallel.For(0, count, i =>
                {
                    if (!Entities.Buckets[i])
                    {
                        return;
                    }

                    if (!Entities[i].CanUpdate)
                    {
                        return;
                    }
                    Entities[i].Execute();
                });
            }
        }

        public void Dispose()
        {
            if (m_IsDispose)
            {
                return;
            }
            m_IsDispose = true;
        }

        public T FindObjectOfType<T>() where T : ECSObject
        {
            Type type = typeof(T);
            var elements = Entities.ToArray();
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].GetType() == type)
                {
                    return elements[i] as T;
                }

                for (int j = 0; j < elements[i].Components.Count; j++)
                {
                    if (elements[i].Components[j].GetType() == type)
                    {
                        return elements[i].Components[j] as T;
                    }
                }
            }
            return null;
        }

        public T[] FindObjectsOfType<T>() where T : ECSObject
        {
            Type type = typeof(T);
            var entities = Entities.ToArray();
            List<T> elements = new List<T>();
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].GetType() == type)
                {
                    elements.Add(entities[i] as T);
                }
                for (int n = 0; n < entities[i].Components.Count; n++)
                {
                    if (entities[i].Components[n].GetType() == type)
                    {
                        elements.Add(entities[i].Components[n] as T);
                    }
                }
            }
            return elements.ToArray();
        }
    }
}

