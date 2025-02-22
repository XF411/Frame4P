using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fream4P.Core
{
    /// <summary>
    /// ManagerBase�����������й������Ļ��࣬�����˹���������������
    /// </summary>
    public abstract class ManagerBase
    {
        /// <summary>
        /// �洢����Manager���ֵ�
        /// </summary>
        private static Dictionary<Type, ManagerBase> ManagersDict = new();

        /// <summary>
        /// ��Ϸ����ڶ���
        /// </summary>
        protected GameLauncher gameLauncher { get; private set; }

        /// <summary>
        /// �������Ƿ����õı�־
        /// </summary>
        private bool enable;

        /// <summary>
        /// ��ȡ�������Ƿ����õ����ԡ�
        /// </summary>
        public bool Enable => enable;

        /// <summary>
        /// ��ȡָ�����͵Ĺ�����ʵ����
        /// </summary>
        /// <typeparam name="T">Ҫ��ȡ�Ĺ��������͡�</typeparam>
        /// <returns>ָ�����͵Ĺ�����ʵ����</returns>
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
        /// ���ָ�����͵Ĺ�������Ӧ�ó���
        /// </summary>
        internal static ManagerBase AddManager(GameLauncher game, Type type)
        {
            var manager = (ManagerBase)Activator.CreateInstance(type);
            ManagersDict.Add(type, manager);
            manager.Init(game);
            return manager;
        }

        /// <summary>
        /// ����ָ�����͵Ĺ�������
        /// </summary>
        internal static void StartManager(Type type)
        {
            if (!ManagersDict.TryGetValue(type, out ManagerBase manager))
                return;
            manager.Enter();
        }

        /// <summary>
        /// ָֹͣ�����͵Ĺ�������
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
        /// �Ƴ�ָ�����͵Ĺ�������
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
        /// ��ʼ����������
        /// </summary>
        internal void Init(GameLauncher game)
        {
            gameLauncher = game;
            OnCreate();
        }

        /// <summary>
        /// �����������
        /// </summary>
        internal void Enter()
        {
            enable = true;
            OnEnter();
        }

        /// <summary>
        /// �˳���������
        /// </summary>
        internal void Exit()
        {
            OnExit();
            enable = false;
        }

        /// <summary>
        /// �ͷŹ�������
        /// </summary>
        internal void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// ������������ʱ���õķ�����
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// ������������ʱ���õķ�����
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// ���������˳�ʱ���õķ�����
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// ���������ͷ�ʱ���õķ�����
        /// </summary>
        protected virtual void OnDispose() { }
    }
}
