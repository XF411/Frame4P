using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Fream4P.Core;

namespace Fream4P.Core.Event
{
    public delegate void CallBackHandler(string msgId, object data);

    /// <summary>
    /// �����¼���Ϣ
    /// Ϊ��ȷ���¼��Ļص�����ȷע�ᡢ�Ƴ���ִ�У���˽����ٴη�װ��
    /// </summary>
    public sealed class EventInfo
    {
        /// <summary>
        /// ��ǰ������Ļص���
        /// �����ȫ�ֻص�������Ƴ���
        /// </summary>
        private Dictionary<string, CallBackHandler> CallBackDict = new Dictionary<string, CallBackHandler>();
        private List<CallBackHandler> CallBackList = new List<CallBackHandler>();//��ȫ���¼�
        private List<CallBackHandler> GlobalCallBackList = new List<CallBackHandler>();//ȫ���¼���

        /// <summary>
        /// Ϊ���ڲ�ͬ�ĳ������ܹ���ȷ���������ȫ�ֻص��ͷ�ȫ�ֻص�
        /// ��¼��Щ�ص���ȫ�ֻص����Ա���ִ�к��Ƴ��ص�ʱ�������ֺʹ���
        /// ��Addʱ�������ӵĻص���ȫ�ֻص����Ὣ���ֵ��ӵ� GlobalCallStringList ��
        /// Invoke��ʱ�򣬱��� GlobalCallStringList �еļ�ֵ��ִ�ж�Ӧ��ȫ�ֻص�
        /// ��OnSceneSwitchʱ�������л�ʱ���Ƴ����з�ȫ�ֻص���ͨ�� GlobalCallStringList ���ų�ȫ�ֻص�
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
                //�Ѿ�ע���˴˷������������滻
                CallBackDict[key] = handler;
                Logger.LogError($"[{key}] : �Ѿ���ע����ˣ�Valid:{CheckValid(handler)} ; ԭ�¼����滻��");
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
            //���ɾ���Ǵ��ⲿ���õģ������ǲ���ȫ�ֵģ���Ҫɾ����
            CallBackDict.Remove(key);
            CallBackList.Remove(handler);
            GlobalCallStringList.Remove(key);
            GlobalCallBackList.Remove(handler);
        }

        public void Invoke(string msgID, object param)
        {
            //ȫ���¼���
            int length = GlobalCallStringList.Count;
            CallBackHandler callBack;
            for (int i = length - 1; i >= 0; i--)
            {
                callBack = GlobalCallBackList[i];
                Inovke(msgID, callBack, param);
            }
            //��ȫ���¼���
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
                //��ʱ����б�����δ���ԭ�򣬼Ӹ�try�ݴ�
                try
                {
                    callBack.Invoke(msgID, param);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"==>�¼�[{msgID}]����{ex.Message}<==");
                    Logger.LogError($"{callBack.Method} | {callBack.Target}");
                    Logger.LogError(ex.Source);
                    Logger.LogError(ex.StackTrace);
                    Logger.LogError(ex.InnerException);
                    Logger.LogError($"==>�¼�[{msgID}]������־��ӡ���<==");
                }
            }
            else 
            {
                Logger.LogError($"��Ч�ص���{GetKey(callBack)}");
            }
        }

        public void OnSceneSwitch()
        {
            //�����л�ʱ���Ƴ����еķ�ȫ�ֻص���
            var list = CallBackDict.Keys.ToList();

            //�ų�ȫ�֣�
            int length = GlobalCallStringList.Count;
            for (int i = 0; i < length; i++)
            {
                var key = GlobalCallStringList[i];
                list.Remove(key);
            }

            //�Ƴ���
            length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var key = list[i];
                CallBackDict.Remove(key);
            }
            CallBackList.Clear();
        }

        /// <summary>
        /// ��ȡ�ص��ļ�ֵ��
        /// ����+��������
        /// </summary>
        public static string GetKey(CallBackHandler handler)
        {
            var method = handler.Method;

            if (method.IsStatic)
                return $"{method.DeclaringType}.{method.Name}";

            //������Ǿ�̬�࣬��ͬ��ʵ����Ϊ��ͬ���ࣻ
            var target = handler.Target;
            string objKey;
            if (target == null)
                objKey = "Null";
            else
                objKey = target.GetHashCode().ToString();

            return $"{method.DeclaringType}[{objKey}].{method.Name}";
        }

        /// <summary>
        /// ���һ�������Ƿ�����Ч������
        /// </summary>
        /// <param name="hanlder"></param>
        /// <returns></returns>
        public static bool CheckValid(CallBackHandler handler)
        {
            if (handler == null)
                return false;

            var method = handler.Method;

            //��̬����ί�У�һֱ������Ч��
            if (method.IsStatic)
                return true;

            //�Ǿ�̬���������ԭ���������Ϊ�գ���Ϊ��Ч������
            var target = handler.Target;
            if (target == null)
                return false;

            return true;
        }

    }

}