using OKGamesLib;
using System.Collections.Generic;

namespace OKGamesLib {
    /// <summary>
    /// T型タイプのAddressablesのアセットをキャッシュへ保持/キャッシュから取得する.
    /// </summary>
    public class ResourceSubStore<T> : IResourceSubStore<T> where T : UnityEngine.Object {

        /// <summary>
        /// Addressables Assetで読み込んだアセットは[assetAddress, resource]の本辞書へキャッシュされる.
        /// </summary>
        protected Dictionary<string, T> _resourceDict = new Dictionary<string, T>();

        public T Get(string assetAddress) {
            if (!_resourceDict.ContainsKey(assetAddress)) {
                Log.Error($"[{GetType().Name}] Resource not found : <b>{assetAddress}</b>");
                return null;
            }

            return _resourceDict[assetAddress];
        }

        public bool Contains(string assetAddress) {
            return _resourceDict.ContainsKey(assetAddress);
        }

        public virtual void OnLoad(string assetAddress, T resource) {
            Log.Notice(
                $"[{GetType().Name}] OnLoad : <b>{assetAddress}</b> - {resource.GetType()}",
                null, "3bc29a"
            );
            Add(assetAddress, resource);
        }

        public virtual void OnUnload(string assetAddress) {
            Log.Notice(
             $"[{GetType().Name}] *** OnUnload : <b>{assetAddress}</b>",
             null, "4463c9"
            );
            Remove(assetAddress);
        }

        /// <summary>
        /// キャッシュ用の辞書へAddressablesのアセットアドレスとアセットを格納.
        /// </summary>
        /// <param name="assetAddress">キャッシュ辞書へ加えるアセットのアドレス.</param>
        /// <param name="resource">キャッシュ辞書へ加えるアセットの本体</param>
        private void Add(string assetAddress, T resource) {
            if (_resourceDict.ContainsKey(assetAddress)) {
                return;
            }
            _resourceDict.Add(assetAddress, resource);
        }

        /// <summary>
        /// キャッシュ用の辞書からAddressablesのアセットアドレスとアセットを削除.
        /// </summary>
        /// <param name="assetAddress">キャッシュ辞書から削除するアセットのアドレス.</param>
        private void Remove(string assetAddress) {
            if (!_resourceDict.ContainsKey(assetAddress)) {
                return;
            }
            _resourceDict.Remove(assetAddress);
        }
    }
}
