using OKGamesLib;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテスト用のシーンのビュー.
    /// </summary>
    public class TestIAPView : MonoBehaviour {


        /// <summary>
        /// 課金ノードオブジェクトの親オブジェクト.
        /// </summary>
        [SerializeField] private GameObject IAPNodesParent;

        /// <summary>
        /// 課金ノードのリスト.
        /// </summary>
        private List<TestIAPINode> _productNodeList = new List<TestIAPINode>();

        /// <summary>
        /// 課金購入用のノード.
        /// </summary>
        [SerializeField] private GameObject _NodePrefab;

        /// <summary>
        /// リストア処理を手動で行うボタン.
        /// </summary>
        public ButtonWrapper RestoreButton => _restoreButton;
        [SerializeField] private ButtonWrapper _restoreButton;


        /// <summary>
        /// ビューを生成する.
        /// </summary>
        /// <param name="transfer">ノードに必要なパラメータが入ったtransfer.</param>
        public void Create(TestIAPNodeTransfer transfer) {
            // オブジェクト生成
            var obj = Instantiate(_NodePrefab);
            // 座標調整のため親オブジェクト設定.
            obj.transform.SetParent(IAPNodesParent.transform, false);
            // Nodeコンポーネントの設定.
            var node = obj.GetComponent<TestIAPINode>();

            // ノードのセットアップ.
            SetupNode(node, transfer);

            // リストへ追加.
            _productNodeList.Add(node);
        }

        /// <summary>
        /// ノードのセットアップ.
        /// </summary>
        /// <param name="node">ノード.</param>
        /// <param name="transfer">表示に使うデータ群.</param>
        private void SetupNode(TestIAPINode node, TestIAPNodeTransfer transfer) {
            node.SetID(transfer.ID);
            node.SetTitle(transfer.TitleKey);
            node.SetBody(transfer.BodyKey);
            node.SetPrice(transfer.CurrencyCode, transfer.Price);

            // 商品の購入状態に応じてノードのマスクと、ボタン押下時のアクションを処理分けする.
            var state = transfer.PurchaseState;
            node.Button.Button.onClick.RemoveAllListeners();
            if (state == PurchaseState.Purchased) {
                // 新たに購入はできないことを通知する.
                node.Button.SetClickAction(() => { Log.Notice("購入済みのアイテム."); });
                node.SetMask(true);
            } else if (state == PurchaseState.Pending) {
                // 新たに購入はできないことを通知する.
                node.Button.SetClickAction(() => { Log.Notice("Pending中のアイテム."); });
                node.SetMask(true);
            } else {
                // 購入アクションをボタンへ仕込む.
                node.Button.SetClickActionAsync(transfer.ButtonAsync);
                node.SetMask(false);
            }
        }

        public void UpdateNode(string id, TestIAPNodeTransfer transfer) {
            var node = _productNodeList.FirstOrDefault(x => x.ID == id);
            // ノードのセットアップ.
            SetupNode(node, transfer);
        }


        /// <summary>
        /// ビューの削除.
        /// </summary>
        /// <param name="iD">削除するビューのID.</param>
        public void Remove(string iD) {
            var node = _productNodeList.FirstOrDefault(x => x.ID == iD);
            if (node == null) {
                return;
            }

            _productNodeList.Remove(node);
            Destroy(node.gameObject);
        }

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        public void Dispose() {
            _productNodeList.ForEach(x => { x.Dispose(); });
            _productNodeList.Clear();
            _productNodeList = null;

            _NodePrefab = null;
        }
    }
}
