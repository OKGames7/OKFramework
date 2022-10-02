using OKGamesLib;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OKGamesLib {
    /// <summary>
    /// Addressablesのリソースのアドレスと参照カウントを保持する.
    /// </summary>
    public class ResourceRegistry {

        /// <summary>
        /// Addressables Groupで登録されているEntryを格納する辞書.
        /// </summary>
        private Dictionary<string, ResourceEntry> _entries = new Dictionary<string, ResourceEntry>();


        /// <summary>
        /// <see cref="_entries"/>の要素を全て削除する.
        /// </summary>
        public void Clear() {
            _entries.Clear();
        }

        /// <summary>
        /// <see cref="_entries"/>に要素を追加し、参照カウンタも加算する.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        /// <param name="isGlobalScope">現在シーン以外からも参照されるか.</param>
        public void Retain(string assetAddress, bool isGlobalScope = false) {
            if (!_entries.ContainsKey(assetAddress)) {
                _entries.Add(assetAddress, new ResourceEntry() { Address = assetAddress });
            }

            if (isGlobalScope) {
                _entries[assetAddress].GlobalScopeRefCount += 1;
            } else {
                _entries[assetAddress].SceneScopeRefCount += 1;
            }
        }

        /// <summary>
        /// <see cref="_entries"/>の参照カウンタも減算する.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        /// <param name="isGlobalScope">現在シーン以外からも参照されるか.</param>
        public void Release(string assetAddress, bool isGlobalScope = false) {
            if (!ValidateKey(assetAddress)) {
                return;
            }

            if (isGlobalScope) {
                _entries[assetAddress].GlobalScopeRefCount -= 1;
            } else {
                _entries[assetAddress].SceneScopeRefCount -= 1;
            }
        }

        /// <summary>
        /// <see cref="_entries"/>の全ての要素に対してglobalかsceneかどちらか選択した一方の参照カウンタを1減算する.
        /// </summary>
        /// <param name="isGlobalScope">現在シーン以外からも参照されるか</param>
        public void ReleaseAll(bool isGlobalScope = false) {
            foreach (var entry in _entries.Values) {
                if (isGlobalScope) {
                    if (entry.GlobalScopeRefCount > 0) {
                        entry.GlobalScopeRefCount -= 1;
                    }
                } else {
                    if (entry.SceneScopeRefCount > 0) {
                        entry.SceneScopeRefCount -= 1;
                    }
                }
            }
        }

        /// <summary>
        /// ロード済みをマーキングする.
        /// リソースの型とアンロード用のハンドラもここで保持する.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        /// <param name="resource">アセット自体.</param>
        /// <param name="handle">AddressablesのResouce読み込み時のhande.</param>
        public void MarkLoaded(string assetAddress, UnityEngine.Object resource, AsyncOperationHandle handle) {
            if (!ValidateKey(assetAddress)) {
                return;
            }

            var entry = _entries[assetAddress];
            entry.Loaded = true;
            entry.Type = resource.GetType();
            entry.handle = handle;
        }

        /// <summary>
        /// ロード済みのマーキングを解除する.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        public void UnmarkLoaded(string assetAddress) {
            if (!ValidateKey(assetAddress)) {
                return;
            }

            var entry = _entries[assetAddress];
            entry.Loaded = false;
            entry.Type = null;
        }

        /// <summary>
        /// 指定したアセットのアドレスで<see cref="_entries"/>のvalueを取得.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        /// <returns>Addressables GroupのEntry情報を格納したクラス.</returns>
        public ResourceEntry GetEntry(string assetAddress) {
            ResourceEntry entry;
            if (!_entries.TryGetValue(assetAddress, out entry)) {
                Log.Error($"[ResourceRegistry] Entry not found : {assetAddress}");
                return null;
            }

            return entry;
        }

        /// <summary>
        ///  <see cref="_entries"/>のvalueの全要素をリストにして取得.
        /// </summary>
        /// <returns>Addressables GroupのEntry情報を格納したクラスのリスト.</returns>
        public List<ResourceEntry> GetEntryList() {
            return _entries.Values.Where(x => x.Type != null).ToList();
        }

        /// <summary>
        /// 指定したアドレスのアセットが参照されているか.
        /// </summary>
        /// <param name="assetAddress"></param>
        /// <returns></returns>
        public bool IsReferenced(string assetAddress) {
            return GetEntry(assetAddress).RefCount > 0;
        }

        /// <summary>
        /// ロードが必要なリソース(参照カウント1以上で未ロードのリソース)が1つでも残っていたらtrue.
        /// </summary>
        /// <returns>ロードが必要かどうか.</returns>
        public bool ShouldLoadAny() {
            foreach (var entry in _entries.Values) {
                if (!entry.Loaded && entry.RefCount > 0) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ロードが必要なリソース(参照カウント1以上で未ロードのリソース)のアドレスのリストを取得する.
        /// </summary>
        /// <returns>ロードが必要なアドレスのリスト.</returns>
        public List<string> GetAddressListToLoad() {
            return _entries.Values.Where(x => !x.Loaded && x.RefCount > 0)
                    .Select(x => x.Address)
                    .ToList();
        }

        /// <summary>
        /// アンロードが必要なリソース(ロード済みで参照カウントが0未満のリソース)のアドレスのリストを取得する.
        /// </summary>
        /// <returns>アンロードが必要なアドレスのリスト.</returns>
        public List<string> GetAddressListToUnload() {
            return _entries.Values.Where(x => x.Loaded && x.RefCount <= 0)
                    .Select(x => x.Address)
                    .ToList();
        }

        /// <summary>
        /// バリデーション.
        /// </summary>
        /// <param name="assetAddress">アセットのアドレス.</param>
        /// <returns><see cref="_entries"/>に指定アセットアドレスが含まれるか.</returns>
        private bool ValidateKey(string assetAddress) {
            if (!_entries.ContainsKey(assetAddress)) {
                Log.Error($"[ResourceRegistry] Key not found : {assetAddress}");
                return false;
            }

            return true;
        }
    }
}
