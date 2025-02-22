using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fream4P.Core.Asset;
using Fream4P.Core.Event;
using Fream4P.Core.Audio;
using Fream4P.Core.Camera;
using Fream4P.Core.Config;
using Fream4P.Core.HotUpdate;
using Fream4P.Core.Network;
using Fream4P.Core.UI;
using YooAsset;

namespace Fream4P.Core
{
    /// <summary>
    /// 游戏发布模式
    /// </summary>
    public enum EGameBuildMode
    {
        Editor,
        MonoDebug,
        IL2CPPDebug,
        MonoRelease,
        IL2CPPRelease,
    }

    /// <summary>
    /// 应用程序的主类，负责管理各种管理器并提供对其的访问。
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {

        /// <summary>
        /// 设置后可以用来全局的控制一些事情，例如debug包默认开启全局日志，release包则关闭所有Debug相关的打印
        /// </summary>
        [SerializeField]
        private EGameBuildMode gameBuildMode;

        /// <summary>
        /// 暂时测试用，UIRoot应该挪到UIManager中实现管理
        /// </summary>
        public static Transform UIRoot;
        public EGameBuildMode GameBuildMode 
        {
            get { return gameBuildMode; }
        }

        // 存储应用程序管理器的类型数组
        private static Type[] gameManagerArray = new Type[]
        {
            typeof(AssetManager),
            typeof(AudioManager),
            typeof(CameraManager),
            typeof(ConfigManager),
            typeof(EventManager),
            typeof(HotUpdateManager),
            typeof(NetworkManager),
            typeof(SceneManager),
            typeof(UIManager),
        };

        // 标志应用程序是否正在运行
        private static bool s_Running;
        public static bool Running => s_Running;

        // 提供对各种管理器的访问
        public static AssetManager Asset => ManagerBase.GetManager<AssetManager>();
        public static AudioManager Audio => ManagerBase.GetManager<AudioManager>();
        public static CameraManager Camera => ManagerBase.GetManager<CameraManager>();
        public static ConfigManager Config => ManagerBase.GetManager<ConfigManager>();
        public static EventManager Event => ManagerBase.GetManager<EventManager>();
        public static HotUpdateManager HotUpdate => ManagerBase.GetManager<HotUpdateManager>();
        public static NetworkManager Network => ManagerBase.GetManager<NetworkManager>();
        public static SceneManager Scene => ManagerBase.GetManager<SceneManager>();
        public static UIManager UI => ManagerBase.GetManager<UIManager>();

        // 游戏启动时的初始化逻辑
        async void Awake()
        {
            //Yoo初始化,编辑器环境下的测试
            YooAssets.Initialize();
            var package = YooAssets.CreatePackage(AssetManager.PackageName);
            var param = new EditorSimulateModeParameters();
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            param.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initializationOperation = package.InitializeAsync(param);
            await initializationOperation.ToUniTask();

            foreach (var type in gameManagerArray) 
            {
                ManagerBase.AddManager(this, type);
            }
            await EnterApp();
        }

        void OnDestroy()
        {
            ExitApp();
            foreach (var type in gameManagerArray) 
            {
                ManagerBase.RemoveManager(type);
            }
        }

        /// <summary>
        /// 进入游戏，启动所有游戏管理器，
        /// 这里写成异步，因为考虑进游戏之后其实就会开始加载默认资源
        /// </summary>
        private static async UniTask EnterGame()
        {
            foreach (var type in gameManagerArray)
            {
                ManagerBase.StartManager(type);
            }

            Logger.Log("打印测试Quest", CustomLogType.Quest);
            Logger.Log("打印测试UI", CustomLogType.UI);
            Logger.LogBlue("打印测试Blue", CustomLogType.Blue);
            Logger.LogRed("打印测试Red", CustomLogType.Red);
            Logger.LogYellow("打印测试Yellow", CustomLogType.Yellow);
            Logger.Log("框架启动，应该在这里启动热更新相关的逻辑，完成之后再正式进入游戏的第一个场景");
        }



        /// <summary>
        /// 退出游戏，停止所有游戏管理器。
        /// </summary>
        private static void ExitGame()
        {
            foreach (var type in gameManagerArray) 
            {
                ManagerBase.StopManager(type);
            }
        }

        /// <summary>
        ///重启游戏
        /// </summary>
        public static async void RestartGame()
        {
            ExitGame();
            await EnterGame();
        }

        // 重新启动应用程序
        public static async UniTask RestartApp()
        {
            ExitApp();
            await EnterApp();
        }

        // 关闭应用程序
        private static void ExitApp()
        {
            ExitGame();
            foreach (var type in gameManagerArray) 
            {
                ManagerBase.StopManager(type);
            }
            s_Running = false;
        }

        // 进入游戏的启动场景
        private static async UniTask EnterApp()
        {

            await EnterGame();
        }
    }
}
