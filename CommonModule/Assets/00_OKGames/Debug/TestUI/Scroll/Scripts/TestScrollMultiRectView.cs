using OKGamesLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesTest {

    /// <summary>
    /// テスト用のスクロールマルチビュークラス(ノードの要素プレハブをマルチに持てる).
    /// </summary>
    public class TestScrollMultiRectView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollMultiDataSource {

        // ノードのプレハブ.
        [SerializeField] private GameObject[] _prefabs;
        // Prefabのマッピング.
        private Dictionary<GameObject, int> _prefabIndexMap = new Dictionary<GameObject, int>();

        // プールしているオブジェクト.
        private UnityEngine.Pool.ObjectPool<GameObject>[] _pools;

        // リストに表示する全要素数.
        private readonly int _totalCount = 100;

        // 無限スクロールさせるか.
        [SerializeField] private bool _isInfinitScroll = false;

        /// <summary>
        /// 初期化.
        /// </summary>
        private void Start() {
            _pools = new UnityEngine.Pool.ObjectPool<GameObject>[_prefabs.Length];
            for (var i = 0; i < _prefabs.Length; ++i) {
                var prefab = _prefabs[i];
                var pool = new UnityEngine.Pool.ObjectPool<GameObject>(
                    // オブジェクト生成処理.
                    () => Instantiate(prefab),
                    // オブジェクトがプールから取得される時の処理.
                    o => o.SetActive(true),
                    // オブジェクトがプールに戻される時の処理.
                    o => {
                        o.transform.SetParent(transform);
                        o.SetActive(false);
                    });
                _pools[i] = pool;
            }

            var scrollRect = transform.GetChild(0).GetComponent<LoopScrollRectMulti>();
            scrollRect.prefabSource = this;
            scrollRect.dataSource = this;
            // -1を入れると無限スクロール設定になる.
            scrollRect.totalCount = _isInfinitScroll ? -1 : _totalCount;
            scrollRect.RefillCells();
        }

        /// <summary>
        /// GameObjectが新しく表示のために必要になった時に呼ばれる.
        /// </summary>
        /// <param name="index">取得する要素.</param>
        /// <returns>GameObject.</returns>
        public GameObject GetObject(int index) {
            var dataIndex = _isInfinitScroll ? (int)Mathf.Repeat(index, _totalCount) : index;
            var prefabIndex = dataIndex % _prefabs.Length;
            // 今は要素を_poolsの要素順から取得しているが、動的に変更したければここで選定処理を挟めばOK.
            var instance = _pools[prefabIndex].Get();
            _prefabIndexMap.Add(instance, prefabIndex);
            return instance;
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
            // instanceに応じたプールを取得し返却する.
            var instance = trans.gameObject;
            var prefabIndex = _prefabIndexMap[instance];
            _prefabIndexMap.Remove(instance);
            _pools[prefabIndex].Release(instance);
        }
    }
}
