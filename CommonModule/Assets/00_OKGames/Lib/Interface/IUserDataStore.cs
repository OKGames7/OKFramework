using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// ローカルセーブデータを扱うStoreクラスのインターフェース.
    /// </summary>
    public interface IUserDataStore {

        /// <summary>
        /// 保存されているユーザーデータ.
        /// </summary>
        IReadOnlyReactiveProperty<UserData> Data { get; }

        /// <summary>
        /// データをセーブする.
        /// </summary>
        /// <param name="lang"></param>
        UniTask Save(Language lang);

        /// <summary>
        /// データをロードする.
        /// </summary>
        void Load();
    }
}
