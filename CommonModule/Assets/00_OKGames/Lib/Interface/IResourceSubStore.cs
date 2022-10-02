using System.Collections.Generic;

namespace OKGamesLib {

    /// <summary>
    /// T型タイプのAddressablesのアセットをキャッシュへ保持/キャッシュから取得するクラスのインターフェース.
    /// </summary>
    public interface IResourceSubStore<T> where T : UnityEngine.Object {

        /// <summary>
        /// キャッシュからアセットを取得する.
        /// </summary>
        /// <param name="assetAddress">取得したいアセットのアドレス.</param>
        /// <returns>アセット.</returns>
        T Get(string assetAddress);

        /// <summary>
        /// キャッシュにアセットが含まれているか.
        /// </summary>
        /// <param name="assetAddress">含まれているか確認したいアセットのアドレス.</param>
        /// <returns>アセットがキャッシュに含まれているかどうあk</returns>
        bool Contains(string assetAddress);

        /// <summary>
        /// Addressables Assetでアセットロード時の処理.
        /// キャッシュへ格納する.
        /// </summary>
        /// <param name="assetAddress">ロードしたアセットのアドレス.</param>
        /// <param name="resource">アセット本体.</param>
        void OnLoad(string assetAddress, T resource);

        /// <summary>
        /// Addressables Assetが破棄されたときの処理.
        /// キャッシュから削除する.
        /// </summary>
        /// <param name="assetAddress">破棄されたアセットのアドレス.</param>
        void OnUnload(string assetAddress);
    }
}
