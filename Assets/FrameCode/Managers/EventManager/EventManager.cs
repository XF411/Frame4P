using Fream4P.Core.Asset;
using System.Collections.Generic;
using YooAsset;

namespace Fream4P.Core.Event 
{
    /// <summary>
    /// ����ί��ʵ�ֵ��¼���������
    /// �йܸ����¼��Ĳ�����
    /// </summary>
    public class EventManager: ManagerBase
    {
        /// <summary>
        /// �����¼�
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
        /// ע����Ϣ����
        /// </summary>
        /// <param name="msgId">��Ϣ����</param>
        /// <param name="handler">�ص�����</param>
        public void RegisterEventMsg(string msgId, CallBackHandler handler, bool isGlobal = false)
        {
            //һ��ע��ʱ�ص����������ܻ���Ч������ж����Կ����Ƴ���
            if (!EventInfo.CheckValid(handler))
            {
                Logger.LogError($" ��Ч�ص�����{handler.Method.Name} ��������ע�ᣡ");
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

        //�Ƴ���Ϣ����
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
        /// ֪ͨ��Ϣȫ��
        /// </summary>
        /// <param name="msgName">��Ϣ����</param>
        /// <param name="data">��Ϣ����</param>
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
        /// �����л�ʱ���ã�
        /// ���������еķ�ȫ���¼�ȫ���Ƴ���
        /// ���ǻ��ǽ����ֶ��Ƴ��¼���
        /// </summary>
        public void OnSceneSwitch()
        {
            foreach (var item in msgMap)
            {
                item.Value.OnSceneSwitch();
            }
        }

        /// <summary>
        /// ������лص���
        /// </summary>
        public void Clear() 
        { 
            msgMap.Clear(); 
        }

    }
}