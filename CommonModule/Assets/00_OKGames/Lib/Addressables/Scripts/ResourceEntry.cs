using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OKGamesLib {
    /// <summary>
    /// Addressablesグループで登録するEntryの個情報.
    /// </summary>
    public class ResourceEntry {
        /// <summary>
        /// アセットのアドレス.
        /// </summary>
        public string Address = string.Empty;

        /// <summary>
        /// シーン内で参照されている数.
        /// </summary>
        public int SceneScopeRefCount = 0;

        /// <summary>
        /// ゲーム全体で参照されている数.
        /// </summary>
        public int GlobalScopeRefCount = 0;

        /// <summary>
        /// シーン内+ゲーム全体を合わせた参照の合計数.
        /// </summary>
        public int RefCount => SceneScopeRefCount + GlobalScopeRefCount;

        /// <summary>
        /// ロード済みか.
        /// </summary>
        public bool Loaded = false;

        /// <summary>
        /// 型情報.
        /// </summary>
        public Type Type;

        /// <summary>
        /// Addressableロード時のhander(成功や失敗、結果(リソース)などの情報)を保持する.
        /// </summary>
        public AsyncOperationHandle handle;
    }
}
