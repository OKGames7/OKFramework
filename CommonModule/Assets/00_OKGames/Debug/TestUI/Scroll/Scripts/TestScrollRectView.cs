using OKGamesLib;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesTest {

    /// <summary>
    /// テスト用のスクロールビュークラス.
    /// </summary>
    public class TestScrollRectView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource {

        // スクロールレクト.
        [SerializeField] private LoopScrollRect _scroll;

        // ノードのプレハブ.
        [SerializeField] private GameObject _prefab;


        // リストに表示する全要素数.
        [SerializeField] private int _totalCount = 100;

        // 無限スクロールさせるか.
        [SerializeField] private bool _isInfinitScroll = false;

        private UnityEngine.Pool.ObjectPool<GameObject> _pool;

        /// <summary>
        /// 初期化.
        /// </summary>
        private void Start() {
            _pool = new UnityEngine.Pool.ObjectPool<GameObject>(
                // オブジェクト生成処理.
                () => Instantiate(_prefab),
                // オブジェクトがプールから取得される時の処理.
                o => o.SetActive(true),
                // オブジェクトがプールに戻される時の処理.
                o => {
                    o.transform.SetParent(transform);
                    o.SetActive(false);
                });


            _scroll.prefabSource = this;
            _scroll.dataSource = this;
            // -1を入れると無限スクロール設定になる.
            _scroll.totalCount = _isInfinitScroll ? -1 : _totalCount;
            _scroll.RefillCells();
        }

        /// <summary>
        /// GameObjectが新しく表示のために必要になった時に呼ばれる.
        /// </summary>
        /// <param name="index">取得する要素.</param>
        /// <returns>GameObject.</returns>
        public GameObject GetObject(int index) {
            // プールから取得.
            return _pool.Get();
        }

        /// <summary>
        /// 各要素が表示される時セットアップ処理を書く.
        /// </summary>
        /// <param name="trans">ノードプレハブのTransform.</param>
        /// <param name="index">表示要素の順番.</param>
        public void ProvideData(Transform trans, int index) {
            var dataIndex = _isInfinitScroll ? (int)Mathf.Repeat(index, _totalCount) : index;
            trans.GetChild(0).GetComponent<TextWrapper>().SetText(dataIndex.ToString());
        }

        /// <summary>
        /// GameObjectが不要になった時に呼ばれる
        /// オブジェクトを非表示にしたりプールに帰したりする時の処理を記述する.
        /// </summary>
        /// <param name="trans">returnするオブジェクトのTransform.</param>
        public void ReturnObject(Transform trans) {
            // プールに返却.
            _pool.Release(trans.gameObject);
        }
    }
}
