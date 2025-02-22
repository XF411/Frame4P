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
    /// ��Ϸ����ģʽ
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
    /// Ӧ�ó�������࣬���������ֹ��������ṩ����ķ��ʡ�
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {

        /// <summary>
        /// ���ú��������ȫ�ֵĿ���һЩ���飬����debug��Ĭ�Ͽ���ȫ����־��release����ر�����Debug��صĴ�ӡ
        /// </summary>
        [SerializeField]
        private EGameBuildMode gameBuildMode;

        /// <summary>
        /// ��ʱ�����ã�UIRootӦ��Ų��UIManager��ʵ�ֹ���
        /// </summary>
        public static Transform UIRoot;
        public EGameBuildMode GameBuildMode 
        {
            get { return gameBuildMode; }
        }

        // �洢Ӧ�ó������������������
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

        // ��־Ӧ�ó����Ƿ���������
        private static bool s_Running;
        public static bool Running => s_Running;

        // �ṩ�Ը��ֹ������ķ���
        public static AssetManager Asset => ManagerBase.GetManager<AssetManager>();
        public static AudioManager Audio => ManagerBase.GetManager<AudioManager>();
        public static CameraManager Camera => ManagerBase.GetManager<CameraManager>();
        public static ConfigManager Config => ManagerBase.GetManager<ConfigManager>();
        public static EventManager Event => ManagerBase.GetManager<EventManager>();
        public static HotUpdateManager HotUpdate => ManagerBase.GetManager<HotUpdateManager>();
        public static NetworkManager Network => ManagerBase.GetManager<NetworkManager>();
        public static SceneManager Scene => ManagerBase.GetManager<SceneManager>();
        public static UIManager UI => ManagerBase.GetManager<UIManager>();

        // ��Ϸ����ʱ�ĳ�ʼ���߼�
        async void Awake()
        {
            //Yoo��ʼ��,�༭�������µĲ���
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
        /// ������Ϸ������������Ϸ��������
        /// ����д���첽����Ϊ���ǽ���Ϸ֮����ʵ�ͻῪʼ����Ĭ����Դ
        /// </summary>
        private static async UniTask EnterGame()
        {
            foreach (var type in gameManagerArray)
            {
                ManagerBase.StartManager(type);
            }

            Logger.Log("��ӡ����Quest", CustomLogType.Quest);
            Logger.Log("��ӡ����UI", CustomLogType.UI);
            Logger.LogBlue("��ӡ����Blue", CustomLogType.Blue);
            Logger.LogRed("��ӡ����Red", CustomLogType.Red);
            Logger.LogYellow("��ӡ����Yellow", CustomLogType.Yellow);
            Logger.Log("���������Ӧ�������������ȸ�����ص��߼������֮������ʽ������Ϸ�ĵ�һ������");
        }



        /// <summary>
        /// �˳���Ϸ��ֹͣ������Ϸ��������
        /// </summary>
        private static void ExitGame()
        {
            foreach (var type in gameManagerArray) 
            {
                ManagerBase.StopManager(type);
            }
        }

        /// <summary>
        ///������Ϸ
        /// </summary>
        public static async void RestartGame()
        {
            ExitGame();
            await EnterGame();
        }

        // ��������Ӧ�ó���
        public static async UniTask RestartApp()
        {
            ExitApp();
            await EnterApp();
        }

        // �ر�Ӧ�ó���
        private static void ExitApp()
        {
            ExitGame();
            foreach (var type in gameManagerArray) 
            {
                ManagerBase.StopManager(type);
            }
            s_Running = false;
        }

        // ������Ϸ����������
        private static async UniTask EnterApp()
        {

            await EnterGame();
        }
    }
}
