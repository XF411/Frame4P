using Fream4P.Core.Asset;
using System.Collections.Generic;
using YooAsset;

namespace Fream4P.Core.Event 
{
    /// <summary>
    /// 基于委托实现的事件管理器；
    /// 托管各种事件的操作；
    /// </summary>
    public class EventManager: ManagerBase
    {
        /// <summary>
        /// 所有事件
        /// </summary>
        private Dictionary<string, EventInfo> msgMap;


        protected override void OnCreate()
        {
            msgMap = new Dictionary<string, EventInfo>();
        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override void OnDispose()
        {

        }


        /// <summary>
        /// 注册消息监听
        /// </summary>
        /// <param name="msgId">消息名称</param>
        /// <param name="handler">回调函数</param>
        public void RegisterEventMsg(string msgId, CallBackHandler handler, bool isGlobal = false)
        {
            //一般注册时回调几乎不可能会无效，这个判定可以考虑移除；
            if (!EventInfo.CheckValid(handler))
            {
                Logger.LogError($" 无效回调：【{handler.Method.Name} 】，跳过注册！");
                return;
            }

            if (null == msgMap)
                msgMap = new Dictionary<string, EventInfo>();

            EventInfo Info;
            if (!msgMap.TryGetValue(msgId, out Info))
            {
                Info = new EventInfo();
                msgMap.Add(msgId, Info);
            }
            Info.Add(handler, isGlobal);
        }

        //移除消息监听
        public void RemoveEventMsg(string msgName, CallBackHandler handler)
        {
            if (msgMap == null || handler == null) 
            {
                return;
            }
            EventInfo Info;
            if (msgMap.TryGetValue(msgName, out Info)) 
            {
                Info.Remove(handler);
            }
                
        }

        /// <summary>
        /// 通知消息全局
        /// </summary>
        /// <param name="msgName">消息名称</param>
        /// <param name="data">消息数据</param>
        public void PostNotification(string msgName, object data = null)
        {
            if (msgMap == null)
                return;

            EventInfo Info;
            if (msgMap.TryGetValue(msgName, out Info))
            {
                Info.Invoke(msgName, data);
            }

        }

        /// <summary>
        /// 场景切换时调用；
        /// 这个会把所有的非全局事件全部移除；
        /// 但是还是建议手动移除事件；
        /// </summary>
        public void OnSceneSwitch()
        {
            foreach (var item in msgMap)
            {
                item.Value.OnSceneSwitch();
            }
        }

        /// <summary>
        /// 清除所有回调；
        /// </summary>
        public void Clear() 
        { 
            msgMap.Clear(); 
        }

    }
}