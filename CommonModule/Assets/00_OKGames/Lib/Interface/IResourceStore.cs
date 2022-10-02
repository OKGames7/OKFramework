using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace OKGamesLib {

    /// <summary>
    /// Addressablesのリソースを生成/保持しているRegistryに対してリソースの取得や解放を行うクラス.
    /// <see cref="ResouceRegistry"/>
    /// </summary>
    public interface IResourceStore {

        ResourceRegistry Registry { get; }

        /// <summary>
        /// <see cref="registry"/>へAddressables GroupのEntryを追加する.
        /// </summary>
        /// <param name="assetAddresses">追加するEntryの元となるAddressablesのアドレス.</param>
        void Retain(params string[] assetAddresses);

        /// <summary>
        /// <see cref="registry"/>へAddressables GroupのEntryを追加する.
        /// 参照はglobalスコープで設定する.
        /// </summary>
        /// <param name="assetAddresses">追加するEntryの元となるAddressablesのアドレス.</param>
        void RetainGlobal(params string[] assetAddresses);

        /// <summary>
        /// <see cref="registry"/>へAddressables GroupのEntryを追加し、Assetの生成、キャッシュ用辞書への登録まで自動で行う.
        /// 参照はglobalスコープで設定する。
        /// </summary>
        /// <param name="assetAddresses">追加するEntryの元となるAddressablesのアドレス.</param>
        /// <returns>UniTask</returns>
        UniTask RetainGlobalWithAutoLoad(params string[] assetAddresses);


        /// <summary>
        /// <see cref="registry"/>からAddressables GroupのEntryの参照カウントを減らす.
        /// </summary>
        /// <param name="assetAddresses">参照カウントを減らすEntryの元となるAddressablesのアドレス.</param>
        void Release(params string[] assetAddresses);

        /// <summary>
        /// <see cref="registry"/>からAddressables GroupのEntryの参照カウントを減らす.
        /// 参照はglobalスコープで設定する.
        /// </summary>
        /// <param name="assetAddresses">参照カウントを減らすEntryの元となるAddressablesのアドレス.</param>
        void ReleaseGlobal(params string[] assetAddresses);

        /// <summary>
        /// <see cref="registry"/>の全てのAddressables GroupのEntryの参照カウントを減らす.
        /// </summary>
        void ReleaseAllSceneScoped();

        /// <summary>
        /// <see cref="registry"/>の全てのAddressables GroupのEntryのグローバル参照カウントを減らす.
        /// </summary>
        void ReleaseAllGlobalScoped();

        /// <summary>
        /// <see cref="registry"/>のEntryから参照カウンタを減らし、Addressables.Releaseし、~Storeのキャッシュを消し、RegistryもUnmarkするまで自動で行う.
        /// 参照はglobalスコープで設定する。
        /// </summary>
        /// <param name="assetAddresses">参照カウントを減らすEntryの元となるAddressablesのアドレス.</param>
        void ReleaseGlobalWithAutoUnLoad(params string[] assetAddresses);

        /// <summary>
        /// キャッシュに含まれているか.
        /// </summary>
        /// <param name="assetAddress">キャッシュに含まれているか確認したいオブジェクト.</param>
        /// <returns>含まれていればtrue.</returns>
        bool Contains(string assetAddress);

        /// <summary>
        /// 指定したアドレスでGameObjetを取得する.
        /// </summary>
        /// <param name="assetAddress">取得したいGameObjectのアドレス.</param>
        /// <returns>GameObject</returns>
        GameObject GetGameObj(string assetAddress);

        /// <summary>
        /// 指定したアドレスでScriptableObjectを継承しているT型のObjectを取得する.
        /// </summary>
        /// <typeparam name="T">ScriptableObject型を継承していること.</typeparam>
        /// <param name="assetAddress">取得したいT型のオブジェクトのアドレス</param>
        /// <returns>T</returns>
        T GetObj<T>(string assetAddress) where T : ScriptableObject;

        /// <summary>
        /// 指定したアドレスでAudioClipを取得する.
        /// </summary>
        /// <param name="assetAddress">取得したいAudioClipのアドレス.</param>
        /// <returns>AudioClip</returns>
        AudioClip GetAudio(string assetAddress);

        /// <summary>
        /// Retainしていなくても自分でRetainGlobalWithAutoLoad後、Get関数を呼ぶ.
        /// </summary>
        /// <param name="assetAddresses">Retain、GetしたいAudioClipのアドレス.</param>
        /// <returns></returns>
        UniTask<AudioClip> GetAudioOndemand(string assetAddresses);

        /// <summary>
        /// 指定したアドレスでSpriteAtlasを取得する.
        /// </summary>
        /// <param name="assetAddress">取得したいSpriteAtlasのアドレス.</param>
        /// <returns>SpriteAtlas</returns>
        SpriteAtlas GetSpriteAtlas(string assetAddress);

        /// <summary>
        /// 指定した名前でSpriteを取得する.
        /// </summary>
        /// <param name="spriteName">取得したいSpriteの名前.</param>
        /// <returns>Sprite</returns>
        Sprite GetSprite(string spriteName);

        /// <summary>
        /// Retain系統の関数を読んだ時に貯めたassetAddressのアセット内容を全てAddresssables.LoadAsyncする.
        /// RegistryのMarkLoadedと~Store系にキャッシュも登録する.
        /// </summary>
        /// <returns>UniTask</returns>
        UniTask Load();

        /// <summary>
        /// Release系統の関数を読んだ時に貯めたassetAddressのアセット内容を全てAddresssables.Releaseする.
        /// ~Store系のキャッシュを削除し、RegistryのUnmarkLoadedもする.
        /// </summary>
        void Unload();

    }
}
