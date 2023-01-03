using OKGamesLib;
using UnityEngine;
using UnityEngine.UI;

namespace OKGamesTest {

    /// <summary>
    /// IAPのテスト用のシーンのビュー内に表示するノード.
    /// </summary>
    public class TestIAPINode : MonoBehaviour {

        /// <summary>
        /// タイトルのテキスト.
        /// </summary>
        [SerializeField] private TextWrapper _titleText;

        /// <summary>
        /// 説明文のテキスト.
        /// </summary>
        [SerializeField] private TextWrapper _bodyText;

        /// <summary>
        /// 価格のテキスト.
        /// </summary>
        [SerializeField] private TextWrapper _priceText;

        /// <summary>
        /// 購入できない時に表示するマスク.
        /// </summary>
        [SerializeField] private Image _mask;


        /// <summary>
        /// ボタン.
        /// </summary>
        public ButtonWrapper Button => _button;
        [SerializeField] private ButtonWrapper _button;

        /// <summary>
        /// ID(ProductID).
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// IDを設定する.
        /// </summary>
        /// <param name="id">設定するID.</param>
        public void SetID(string id) {
            ID = id;
        }

        /// <summary>
        /// タイトルを設定する.
        /// </summary>
        /// <param name="key">テキストマスターのキー.</param>
        public void SetTitle(string key) {
            _titleText.SetTextByKey(key);
        }

        /// <summary>
        /// 説明文を設定する.
        /// </summary>
        /// <param name="key">テキストマスターのキー.</param>
        public void SetBody(string key) {
            _bodyText.SetTextByKey(key);
        }

        /// <summary>
        /// 価格を設定する.
        /// </summary>
        /// <param name="currencyCode">通貨単位.</param>
        /// <param name="price">価格.</param>
        public void SetPrice(string currencyCode, string price) {
            _priceText.SetText(string.Format("{0} {1}", currencyCode, price));
        }

        /// <summary>
        /// 購入できない時のマスクの表示、非表示.
        /// </summary>
        /// <param name="isActive">表示するか.</param>
        public void SetMask(bool isActive) {
            _mask.enabled = isActive;
        }

        /// <summary>
        /// 破棄時の処理.
        /// </summary>
        public void Dispose() {
            Destroy(this);
            _titleText = null;
            _bodyText = null;
            _button = null;
        }
    }
}
