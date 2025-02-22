using UnityEngine;
using System;

namespace Fream4P.Core
{
    [System.Flags]
    public enum CustomLogType
    {
        [Header("关闭全部打印，包括报错和警告")]
        CloseWarningAndError = -1,
        [Header("关闭打印")]
        CloseAll = 0,
        [Header("只显示普通白色打印")]
        Normal = 1 << 0,
        [Header("可以自行扩展，比如这里只显示UI相关的打印")]
        UI = 1 << 1,
        [Header("或者这里只显示任务系统相关的打印")]
        Quest = 1 << 2,
        [Header("再或者这里只显示某开发者A的打印")]
        LogForA = 1 << 3,
        [Header("红色打印")]
        Red = 1 << 4,
        [Header("绿色打印")]
        Green = 1 << 5,
        [Header("蓝色打印")]
        Blue = 1 << 6,
        [Header("黄色打印")]
        Yellow = 1 << 7,
        [Header("橙色打印")]
        Orange = 1 << 8,
        [Header("打开全部打印")]
        All = Normal | UI | Quest | LogForA | Red | Green | Blue | Yellow | Orange
    }

    /// <summary>
    /// 打印Log的辅助类；
    /// 默认的Log只会在编辑器下生效；
    /// </summary>
    public static class Logger
    {

        //Debug.Log是有性能消耗的，
        //追求性能的话，最好能在面向玩家的发布包中关闭掉所有打印
        //而且当大家都加了很多打印的时候，开发中想查看log调试bug反而成了难事
        //所以额外封装了这个Logger类，可以继续自由扩展可加上总开关
        //使用这个类可以在不同的情况下对打印进行不同的控制；
        //用这个打印在游戏的主入口，或者全局配置文件中统一控制开启或者关闭打印；
        //缺点是不能双击Console窗口跳转到对应代码行；


        public static CustomLogType mLogType = CustomLogType.All;

        /// <summary>
        /// 这个Log只会在编辑器下生效；
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
        /// 检查指定类型log是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLog(CustomLogType state = CustomLogType.Normal)
        {
            return Include(state);
        }

        /// <summary>
        /// 检查Log_Red是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogRed(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// 检查Log_Green是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogGreen(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// 检查Log_Yellow是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogYellow(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// 检查Log_Orange是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogOrange(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// 检查Log_Blue是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogBlue(CustomLogType type = CustomLogType.Normal)
        {
            return Include(type);
        }

        /// <summary>
        /// 检查WarningAndError是否打开
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsOpenLogWarningAndError()
        {
            return mLogType != CustomLogType.CloseWarningAndError; //IsOpenLog(CustomLogType.Battle);
        }


        /// <summary>
        /// 普通的Log，以红颜色打印；
        /// </summary>
        /// <param name="message"></param>
        public static void LogRed(object message, CustomLogType type = CustomLogType.Normal)
        {
            if (IsOpenLogRed())
                Log($"<color=red>{message}</color>", type);
        }

        /// <summary>
        /// 普通的Log，以红颜色打印；
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
        /// 错误打印;
        /// 这个在真机上不打印；
        /// 除非处于 DEBUG 情况下；
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