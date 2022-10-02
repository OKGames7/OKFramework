
using UnityEditor.IMGUI.Controls;

namespace OKGamesLib {

    /// <summary>
    /// EditorWindow内に表示するAddressablesのリソース使用状況を表示ツリービュー内で使用するデータを所持したItem.
    /// </summary>
    public class ResourceTreeViewItem : TreeViewItem {

        /// <summary>
        /// 型のカテゴリ(ex: GameObject, AudioClip..).
        /// </summary>
        public string Category;

        /// <summary>
        /// アセット名.
        /// </summary>
        public string AssetName;

        /// <summary>
        /// シーン内での参照数.
        /// </summary>
        public int RefCount;

        /// <summary>
        /// グローバルスコープでの参照数.
        /// </summary>
        public int GlobalRefCount;

        /// <summary>
        /// ロード済みの状態か.
        /// </summary>
        public bool Loaded;

        /// <summary>
        /// メモリ量.
        /// </summary>
        public long MemorySize;

        /// <summary>
        /// 備考情報.
        /// </summary>
        public string Info;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="id">レコードの特定用.</param>
        public ResourceTreeViewItem(int id) : base(id) { }
    }
}
