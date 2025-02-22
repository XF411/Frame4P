using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Fream4P.Core;

namespace Fream4P.Core.Event
{
    public delegate void CallBackHandler(string msgId, object data);

    /// <summary>
    /// 单个事件信息
    /// 为了确保事件的回调能正确注册、移除、执行，因此进行再次封装；
    /// </summary>
    public sealed class EventInfo
    {
        /// <summary>
        /// 当前所管理的回调；
        /// 如果是全局回调则看情况移除；
        /// </summary>
        private Dictionary<string, CallBackHandler> CallBackDict = new Dictionary<string, CallBackHandler>();
        private List<CallBackHandler> CallBackList = new List<CallBackHandler>();//非全局事件
        private List<CallBackHandler> GlobalCallBackList = new List<CallBackHandler>();//全局事件；

        /// <summary>
        /// 为了在不同的场景中能够正确管理和区分全局回调和非全局回调
        /// 记录哪些回调是全局回调，以便在执行和移除回调时进行区分和处理
        /// 在Add时，如果添加的回调是全局回调，会将其键值添加到 GlobalCallStringList 中
        /// Invoke的时候，遍历 GlobalCallStringList 中的键值，执行对应的全局回调
        /// 在OnSceneSwitch时，场景切换时会移除所有非全局回调，通过 GlobalCallStringList 来排除全局回调
        /// </summary>
        private List<string> GlobalCallStringList = new List<string>();

        public void Add(CallBackHandler handler, bool isGlobal)
        {
            string key = GetKey(handler);

            CallBackHandler oldHanlder;
            if (CallBackDict.TryGetValue(key, out oldHanlder))
            {
                CallBackList.Remove(oldHanlder);
                GlobalCallBackList.Remove(oldHanlder);
                //已经注册了此方法；这里这替换
                CallBackDict[key] = handler;
                Logger.LogError($"[{key}] : 已经被注册过了！Valid:{CheckValid(handler)} ; 原事件被替换！");
            }
            else
                CallBackDict.Add(key, handler);

            if (isGlobal)
            {
                GlobalCallBackList.Add(handler);
                if (!GlobalCallStringList.Contains(key))
                    GlobalCallStringList.Add(key);
            }
            else
                CallBackList.Add(handler);
        }

        public void Remove(CallBackHandler handler)
        {
            string key = GetKey(handler);
            //这个删除是从外部调用的，无论是不是全局的，都要删除；
            CallBackDict.Remove(key);
            CallBackList.Remove(handler);
            GlobalCallStringList.Remove(key);
            GlobalCallBackList.Remove(handler);
        }

        public void Invoke(string msgID, object param)
        {
            //全局事件；
            int length = GlobalCallStringList.Count;
            CallBackHandler callBack;
            for (int i = length - 1; i >= 0; i--)
            {
                callBack = GlobalCallBackList[i];
                Inovke(msgID, callBack, param);
            }
            //非全局事件；
            length = CallBackList.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                if (i < 0 || i >= CallBackList.Count)
                    continue;

                callBack = CallBackList[i];
                Inovke(msgID, callBack, param);
            }
        }

        private void Inovke(string msgID, CallBackHandler callBack, object param)
        {
            if (CheckValid(callBack))
            {
                //有时候会有报错，还未清楚原因，加个try容错
                try
                {
                    callBack.Invoke(msgID, param);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"==>事件[{msgID}]出错：{ex.Message}<==");
                    Logger.LogError($"{callBack.Method} | {callBack.Target}");
                    Logger.LogError(ex.Source);
                    Logger.LogError(ex.StackTrace);
                    Logger.LogError(ex.InnerException);
                    Logger.LogError($"==>事件[{msgID}]出错日志打印完成<==");
                }
            }
            else 
            {
                Logger.LogError($"无效回调：{GetKey(callBack)}");
            }
        }

        public void OnSceneSwitch()
        {
            //场景切换时，移除所有的非全局回调；
            var list = CallBackDict.Keys.ToList();

            //排除全局；
            int length = GlobalCallStringList.Count;
            for (int i = 0; i < length; i++)
            {
                var key = GlobalCallStringList[i];
                list.Remove(key);
            }

            //移除；
            length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var key = list[i];
                CallBackDict.Remove(key);
            }
            CallBackList.Clear();
        }

        /// <summary>
        /// 获取回调的键值；
        /// 类名+方法名；
        /// </summary>
        public static string GetKey(CallBackHandler handler)
        {
            var method = handler.Method;

            if (method.IsStatic)
                return $"{method.DeclaringType}.{method.Name}";

            //如果不是静态类，不同的实例视为不同的类；
            var target = handler.Target;
            string objKey;
            if (target == null)
                objKey = "Null";
            else
                objKey = target.GetHashCode().ToString();

            return $"{method.DeclaringType}[{objKey}].{method.Name}";
        }

        /// <summary>
        /// 检查一个方法是否是有效方法；
        /// </summary>
        /// <param name="hanlder"></param>
        /// <returns></returns>
        public static bool CheckValid(CallBackHandler handler)
        {
            if (handler == null)
                return false;

            var method = handler.Method;

            //静态方法委托，一直可以生效；
            if (method.IsStatic)
                return true;

            //非静态方法，如果原来的类对象为空，视为无效方法；
            var target = handler.Target;
            if (target == null)
                return false;

            return true;
        }

    }

}