using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using YooAsset;
using System.Threading;
using Fream4P.DataStructure;

namespace Fream4P.Core.Asset
{
    /// <summary>
    /// 资源组类，用于管理资源的加载和释放，
    /// 解释一下为什么要有这么一层封装，
    /// AssetHandle 是 YooAsset 提供的资源加载句柄，负责资源的加载、引用计数和释放
    /// AssetEntry 将 AssetHandle 和对应的资源名称封装在一起，形成一个独立的资源条目。
    /// 通过这种封装，AssetGroup 可以更方便地管理资源的生命周期（如加载、缓存和释放），而无需直接依赖 AssetHandle 的具体实现
    /// 如果说在未来有出现其他更好的资源管理方案，或者想要换回直接从Resourse或者AB包加载资源，
    /// 利用AssetGroup来加载或者释放资源的代码都不需要变更，只需要修改AssetGroup和AssetEntry的实现即可
    /// </summary>
    public class AssetGroup 
    {
        /// <summary>
        /// 资源条目类，用于表示单个资源条目。
        /// </summary>
        internal class AssetEntry 
        {
            // 资源对象池
            private static ObjectPool<AssetEntry> entryPool = new(() => new AssetEntry(), null); 

            public string assetName;
            public AssetHandle handle;

            public static AssetEntry Create()
            {
                return entryPool.Get();
            }

            /// <summary>
            /// 回收资源
            /// </summary>
            public static void Recycle(AssetEntry item)
            {
                item.Release();
                entryPool.Release(item);
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Release()
            {
                handle?.Release();
                handle = null;
                assetName = null;
            }
        }

        private readonly Dictionary<string, LinkedList<AssetEntry>> cacheAsset; // 缓存的资源
        private readonly ResourcePackage assetPackage; // 资源包

        internal AssetGroup()
        {
            cacheAsset = new Dictionary<string, LinkedList<AssetEntry>>();
            assetPackage = YooAssets.GetPackage("DefaultPackage"); // 获取默认资源包
        } 

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <returns>加载的资源实例</returns>
        public T LoadAsset<T>(string assetName) where T : Object
        {
            if(!TryGetAssetFromPool(assetName, out T asset))
            {
                AssetHandle handle = assetPackage.LoadAssetSync<T>(assetName);
                var entry = AddEntry(assetName, handle);
                asset = entry.handle.GetAssetObject<T>();
            }
            return asset;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <returns>加载的资源实例</returns>
        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : Object
        {
            if (!TryGetAssetFromPool(assetName, out T asset))
            {
                var handle = assetPackage.LoadAssetAsync<T>(assetName);
                bool cancelOrFailed = await handle.ToUniTask().AttachExternalCancellation(CancellationToken.None)
                    .SuppressCancellationThrow();

                if (cancelOrFailed)
                {
                    return null;
                }
                var entry = AddEntry(assetName, handle);
                asset = entry.handle.GetAssetObject<T>();
            }
            return asset;
        }

        /// <summary>
        /// 尝试从缓存池中获取资源。
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>是否成功从缓存池中获取资源</returns>
        private bool TryGetAssetFromPool<T>(string assetName, out T asset) where T : Object
        {
            asset = null;
            if(string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("资源加载错误, 传入的assetName为null");
                return true;
            }
            if(!cacheAsset.TryGetValue(assetName, out LinkedList<AssetEntry> linked))
                return false;
            LinkedListNode<AssetEntry> node = linked.First;
            AssetEntry entry = node.Value; 
            asset = entry.handle.GetAssetObject<T>();
            return true;
        }

        /// <summary>
        /// 添加资源到缓存池
        /// </summary>
        /// <returns>添加的资源条目</returns>
        private AssetEntry AddEntry(string assetName, AssetHandle handle)
        {
            AssetEntry entry = AssetEntry.Create();
            entry.assetName = assetName;
            entry.handle = handle;
            if(cacheAsset.TryGetValue(assetName, out LinkedList<AssetEntry> linked))
                linked.AddLast(entry);
            else
            {
                linked = new LinkedList<AssetEntry>();
                linked.AddLast(entry);
                cacheAsset.Add(assetName, linked);
            }
            return entry;
        }

        /// <summary>
        /// 释放指定名称的资源
        /// </summary>
        public void ReleaseAsset(string assetName)
        {
            if(!cacheAsset.TryGetValue(assetName, out LinkedList<AssetEntry> linked))
                return;
            LinkedListNode<AssetEntry> node = linked.First;
            AssetEntry entry = node.Value; 
            AssetEntry.Recycle(entry);
            linked.RemoveFirst();
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        internal void Release()
        {
            foreach (var assetName in cacheAsset.Keys)
            {
                if(!cacheAsset.TryGetValue(assetName, out LinkedList<AssetEntry> linked))
                    continue;
                var enumerator = linked.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AssetEntry.Recycle(enumerator.Current);
                }
            }
            cacheAsset.Clear();
        }
    }
}
