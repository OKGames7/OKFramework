using OKGamesLib;
using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// ユーザーデータのアクセスポイント.
    /// </summary>
    public class UserDataStore : IUserDataStore {

        /// <summary>
        ///  ローカル保存する際のkey.
        /// </summary>
        private readonly string _userDataKey = "userData";

        /// <summary>
        /// ローカルユーザーの実データ
        /// </summary>
        public IReadOnlyReactiveProperty<UserData> Data => _data;
        private readonly ReactiveProperty<UserData> _data = new ReactiveProperty<UserData>();


        /// <summary>
        /// 保存する.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async UniTask Save(Language lang) {
            var newData = new UserData();
            newData.CurrentLanguage = lang;

            if (_data.Value == null) {
                // 比較用にストレージのデータを取得しキャッシュに格納する.
                Load();
            }

            if (!Equals(_data.Value, newData)) {
                await StorageUtility.SaveAsync(_userDataKey, newData);

                _data.Value = newData;
            }
        }

        /// <summary>
        /// ストレージのユーザーデータを取得しキャッシュへ格納する.
        /// </summary>
        public void Load() {
            Log.Notice("ユーザーデータのロード処理開始");
            if (_data.Value != null) {
                return;
            }

            if (StorageUtility.Exists(_userDataKey)) {
                var data = StorageUtility.Load(_userDataKey) as UserData;
                _data.Value = data;
            } else {
                Log.Notice("ローカルストレージにユーザーデータがまだ存在しない.");
                // コンストラクタで初期値を持ったデータの生成.
                _data.Value = new UserData();
            }
        }

        /// <summary>
        /// すでに保存している分と更新しようとしている分の内容比較.
        /// </summary>
        /// <param name="a">すでに保存しているデータ.</param>
        /// <param name="b">これから更新をかけたいデータ.</param>
        /// <returns>同等ならtrue.</returns>
        private static bool Equals(UserData a, UserData b) {
            if (a == b) {
                return true;
            }

            if ((a == null) || (b == null)) {
                return false;
            }

            return (a.CurrentLanguage == b.CurrentLanguage);
        }
    }
}
