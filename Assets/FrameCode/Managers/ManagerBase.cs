using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fream4P.Core
{
    /// <summary>
    /// ManagerBase抽象类是所有管理器的基类，定义了管理器的生命周期
    /// </summary>
    public abstract class ManagerBase
    {
        /// <summary>
        /// 存储所有Manager的字典
        /// </summary>
        private static Dictionary<Type, ManagerBase> ManagersDict = new();

        /// <summary>
        /// 游戏主入口对象
        /// </summary>
        protected GameLauncher gameLauncher { get; private set; }

        /// <summary>
        /// 管理器是否启用的标志
        /// </summary>
        private bool enable;

        /// <summary>
        /// 获取管理器是否启用的属性。
        /// </summary>
        public bool Enable => enable;

        /// <summary>
        /// 获取指定类型的管理器实例。
        /// </summary>
        /// <typeparam name="T">要获取的管理器类型。</typeparam>
        /// <returns>指定类型的管理器实例。</returns>
        public static T GetManager<T>() where T : ManagerBase
        {
            var type = typeof(T);
            if (ManagersDict.TryGetValue(type, out ManagerBase manager))
            {
                return manager as T;
            }
            return null;
        }

        /// <summary>
        /// 添加指定类型的管理器到应用程序。
        /// </summary>
        internal static ManagerBase AddManager(GameLauncher game, Type type)
        {
            var manager = (ManagerBase)Activator.CreateInstance(type);
            ManagersDict.Add(type, manager);
            manager.Init(game);
            return manager;
        }

        /// <summary>
        /// 启动指定类型的管理器。
        /// </summary>
        internal static void StartManager(Type type)
        {
            if (!ManagersDict.TryGetValue(type, out ManagerBase manager))
                return;
            manager.Enter();
        }

        /// <summary>
        /// 停止指定类型的管理器。
        /// </summary>
        internal static void StopManager(Type type)
        {
            try
            {
                if (!ManagersDict.TryGetValue(type, out ManagerBase manager)) 
                {
                    return;
                }
                manager.Exit();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        /// <summary>
        /// 移除指定类型的管理器。
        /// </summary>
        internal static void RemoveManager(Type type)
        {
            if (!ManagersDict.TryGetValue(type, out ManagerBase manager))
                return;
            if (manager.Enable)
            {
                manager.Exit();
            }
            manager.Dispose();
            ManagersDict.Remove(type);
        }

        /// <summary>
        /// 初始化管理器。
        /// </summary>
        internal void Init(GameLauncher game)
        {
            gameLauncher = game;
            OnCreate();
        }

        /// <summary>
        /// 进入管理器。
        /// </summary>
        internal void Enter()
        {
            enable = true;
            OnEnter();
        }

        /// <summary>
        /// 退出管理器。
        /// </summary>
        internal void Exit()
        {
            OnExit();
            enable = false;
        }

        /// <summary>
        /// 释放管理器。
        /// </summary>
        internal void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// 当管理器创建时调用的方法。
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// 当管理器进入时调用的方法。
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 当管理器退出时调用的方法。
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// 当管理器释放时调用的方法。
        /// </summary>
        protected virtual void OnDispose() { }
    }
}
