using UnityEngine;
using System;

namespace Fream4P.Core
{
    [System.Flags]
    public enum CustomLogType
    {
        [Header("�ر�ȫ����ӡ����������;���")]
        CloseWarningAndError = -1,
        [Header("�رմ�ӡ")]
        CloseAll = 0,
        [Header("ֻ��ʾ��ͨ��ɫ��ӡ")]
        Normal = 1 << 0,
        [Header("����������չ����������ֻ��ʾUI��صĴ�ӡ")]
        UI = 1 << 1,
        [Header("��������ֻ��ʾ����ϵͳ��صĴ�ӡ")]
        Quest = 1 << 2,
        [Header("�ٻ�������ֻ��ʾĳ������A�Ĵ�ӡ")]
        LogForA = 1 << 3,
        [Header("��ɫ��ӡ")]
        Red = 1 << 4,
        [Header("��ɫ��ӡ")]
        Green = 1 << 5,
        [Header("��ɫ��ӡ")]
        Blue = 1 << 6,
        [Header("��ɫ��ӡ")]
        Yellow = 1 << 7,
        [Header("��ɫ��ӡ")]
        Orange = 1 << 8,
        [Header("��ȫ����ӡ")]
        All = Normal | UI | Quest | LogForA | Red | Green | Blue | Yellow | Orange
    }

    /// <summary>
    /// ��ӡLog�ĸ����ࣻ
    /// Ĭ�ϵ�Logֻ���ڱ༭������Ч��
    /// </summary>
    public static class Logger
    {

        //Debug.Log�����������ĵģ�
        //׷�����ܵĻ����������������ҵķ������йرյ����д�ӡ
        //���ҵ���Ҷ����˺ܶ��ӡ��ʱ�򣬿�������鿴log����bug������������
        //���Զ����װ�����Logger�࣬���Լ���������չ�ɼ����ܿ���
        //ʹ�����������ڲ�ͬ������¶Դ�ӡ���в�ͬ�Ŀ��ƣ�
        //�������ӡ����Ϸ������ڣ�����ȫ�������ļ���ͳһ���ƿ������߹رմ�ӡ��
        //ȱ���ǲ���˫��Console������ת����Ӧ�����У�


        public static CustomLogType mLogType = CustomLogType.All;

        /// <summary>
        /// ���Logֻ���ڱ༭������Ч��
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (Include(type))
                Debug.Log($"[{DateTime.Now.ToString("mm:ss:fff")}]{message}");
        }

        private static bool Include(CustomLogType state)
        {
            if (mLogType == CustomLogType.CloseWarningAndError)
            {
                return false;
            }
            else if (mLogType == CustomLogType.CloseAll)
            {
                return (state == CustomLogType.CloseWarningAndError);
            }
            return mLogType.HasFlag(state);
        }


        /// <summary>
        /// ���ָ������log�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLog(CustomLogType state = CustomLogType.Normal)
        {
            return Include(state);
        }

        /// <summary>
        /// ���Log_Red�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogRed(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// ���Log_Green�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogGreen(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// ���Log_Yellow�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogYellow(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// ���Log_Orange�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogOrange(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// ���Log_Blue�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogBlue(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// ���WarningAndError�Ƿ��
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogWarningAndError()
        {
            return mLogType != CustomLogType.CloseWarningAndError; //IsOpenLog(CustomLogType.Battle);
        }


        /// <summary>
        /// ��ͨ��Log���Ժ���ɫ��ӡ��
        /// </summary>
        /// <param name="message"></param>
        public static void LogRed(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogRed())
                Log($"<color=red>{message}</color>", type);
        }

        /// <summary>
        /// ��ͨ��Log���Ժ���ɫ��ӡ��
        /// </summary>
        /// <param name="message"></param>
        public static void LogGreen(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogGreen())
                Log($"<color=green>{message}</color>", type);
        }

        public static void LogBlue(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogBlue())
                Log($"<color=#0080FF>{message}</color>", type);
        }

        public static void LogYellow(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogYellow())
                Log($"<color=yellow>{message}</color>", type);
        }

        public static void LogOrange(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogOrange())
                Log($"<color=FF8000>{message}</color>", type);
        }

        /// <summary>
        /// �����ӡ;
        /// ���������ϲ���ӡ��
        /// ���Ǵ��� DEBUG ����£�
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            if (mLogType == CustomLogType.CloseWarningAndError)
                return;
            Debug.LogError($"[{DateTime.Now.ToString("mm:ss:fff")}]{message}");
        }

        public static void LogException(Exception ex)
        {
            Debug.LogException(ex);
        }

        public static void LogWarning(object message)
        {
            if (mLogType == CustomLogType.CloseWarningAndError)
                return;
            Debug.LogWarning($"[{DateTime.Now.ToString("mm:ss:fff")}]{message}");
        }
    }
}