using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fream4P.DataStructure
{
    #region 使用示例
    /// <summary>
    /// 对象池使用示例
    /// </summary>

    // 创建一个包含 Vector3 实例的 ObjectPool 实例。
    //ObjectPool<Vector3> objectPool = new ObjectPool<Vector3>(() => new Vector3(), null, null);
    // 从池中获取对象。
    //Vector3 obj = objectPool.Get();
    // 将对象释放回池中。
    //objectPool.Release(obj);
    // 清空池。
    //objectPool.Clear();
    #endregion


    /// <summary>
    /// 泛型对象池,使用时声明一个池本身作为对象管理，
    /// 创建对象池时需要至少传入一个创建所需对象的函数，
    /// 可以自由选择是否传入释放对象时要执行的函数
    /// </summary>
    public class ObjectPool<T>
    {
        /// <summary>
        /// 管理对象的栈，先进后出，如果栈为空的时候取对象就会执行对象创建委托
        /// </summary>
        private Stack<T> objStack = new();

        /// <summary>
        /// 创建对象实例的委托，函数的返回值类型在创建Pool对象时就确定了
        /// </summary>
        private Func<T> createFunciton;

        /// <summary>
        /// 释放对象实例时执行的委托,需要传入的参数在创建Pool对象时就确定了
        /// </summary>
        private Action<T> onReleaseAction;

        /// <summary>
        /// 对象池中所有对象的总数
        /// </summary>
        public int AllObjCount { get; private set; }

        /// <summary>
        /// 对象池中活动物体的总数
        /// </summary>
        public int EnableObjCount { get { return AllObjCount - DisableObjCount; } }

        /// <summary>
        /// 对象池中非活动物体的总数
        /// </summary>
        public int DisableObjCount { get { return objStack.Count; } }

        /// <summary>
        /// 构造函数,创建一个新的对象池
        /// </summary>
        /// <param name="create">创建对象的函数</param>
        /// <param name="actionOnRelease">释放对象实例时执行的函数,可不传</param>
        public ObjectPool(Func<T> create, Action<T> onActionRelease = null)
        {
            createFunciton = create;
            onReleaseAction = onActionRelease;
        }

        /// <summary>
        /// 可以预先创建指定数量的资源存入对象池中管理,
        /// 例如加载界面的逻辑中提前预创建资源.
        /// 避免游戏流程中的一个个创建
        /// </summary>
        public void PreCreat(int preNum) 
        {
            if (preNum <= 0 || objStack.Count >= preNum) 
            {
                return;
            }
            for (int i = 0; i < preNum; i++) 
            {
                T obj = createFunciton();
                objStack.Push(obj);
                AllObjCount++;
            }
        }


        /// <summary>
        ///从池中获取对象
        /// </summary>
        /// <returns>获取到的对象实例。</returns>
        public T Get()
        {
            T obj;
            
            //如果没有足够的对象实例，先创建一个新对象。
            if (objStack.Count == 0)
            {
                obj = createFunciton();
                AllObjCount++;
            }
            else
            {
                //数量足够，从栈中取出一个对象复用
                obj = objStack.Pop();
            }
            return obj;
        }

        /// <summary>
        /// 将对象实例释放回池中。
        /// </summary>
        public void Release(T obj)
        {
            //检查释放的对象是否已在栈顶。如果是，则输出错误信息。
            if (objStack.Count > 0 && ReferenceEquals(objStack.Peek(), obj)) 
            {
                Debug.LogError("当前释放的对象已在对象池中");
            }   
            //如果有释放对象时的委托，则执行该委托。
            if (onReleaseAction != null)
            {
                onReleaseAction(obj);
            }
            //将对象实例推入栈中，以便重复使用。
            objStack.Push(obj);
        }

        /// <summary>
        /// 清空对象池，释放所有对象实例。
        /// </summary>
        public void Clear()
        {
            objStack.Clear();
            AllObjCount = 0;
        }
    }
}
