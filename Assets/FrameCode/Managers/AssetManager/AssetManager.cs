using Cysharp.Threading.Tasks;
using Fream4P.DataStructure;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Fream4P.Core.Asset
{
    /// <summary>
    /// 基于YooAsset实现的资源管理
    /// </summary>
    public class AssetManager : ManagerBase
    {
        private const string defaultFolderName = "yoo";
        public static string DefaultFolderName => defaultFolderName;
        private const string packageName = "DefaultPackage";
        public static string PackageName => packageName;

        private ObjectPool<AssetGroup> assetGroupPool;
        private AssetGroup assetGroup;
        private ResourcePackage defaultPackage;

        protected override void OnCreate()
        {
            assetGroupPool = new ObjectPool<AssetGroup>(() => new AssetGroup(), null);
        }

        protected override void OnEnter()
        {
            // 创建默认的资源包
            defaultPackage = YooAssets.GetPackage(packageName);
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(defaultPackage);
            assetGroup = CreateAssetGroup();
        }

        protected override void OnExit()
        {
            ReleaseAssetGroup(assetGroup);
            assetGroup = null;
        }

        protected override void OnDispose()
        {
            assetGroupPool = null;
        }

        public AssetGroup CreateAssetGroup()
        {
            return assetGroupPool.Get();
        }

        public void ReleaseAssetGroup(AssetGroup group)
        {
            if (group != null)
            {
                group.Release();
            }
            assetGroupPool.Release(group);
        }

        public T LoadAsset<T>(string assetName) where T : Object
        {
            return assetGroup.LoadAsset<T>(assetName);
        }

        public void ReleaseAsset(string assetName)
        {
            assetGroup.ReleaseAsset(assetName);
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : Object
        {
            return await assetGroup.LoadAssetAsync<T>(assetName);
        }

        public SceneHandle LoadSceneAsync(string name, LoadSceneMode mode)
        {
            SceneHandle handle = defaultPackage.LoadSceneAsync(name, mode);
            return handle;
        }
    }
}