using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace OKGamesLib {

    /// <summary>
    /// シーン内の全テキストの状況を監視する.
    /// </summary>
    public class TextWatcher : ITextWatcher {

        public List<TextWrapper> WrapperList => _wrapperList;

        /// 現在シーン上に存在する<see cref = "TextWrapper"/>のリスト(DontDestoryなものも含む).
        private List<TextWrapper> _wrapperList = new List<TextWrapper>();

        public void Add(TextWrapper wrapper) {
            _wrapperList.Add(wrapper);
        }

        public void Remove(TextWrapper wrapper) {
            _wrapperList.Remove(wrapper);
        }

        public async UniTask UpdateTexts(Language language) {
            var tasks = new List<UniTask>();
            for (int i = 0; i < _wrapperList.Count; ++i) {
                // 言語設定
                _wrapperList[i].SetLanguage(language);
                // セットアップ.
                tasks.Add(_wrapperList[i].Setup());
            }

            // 全ての要素がセットアップ終わるまで待つ.
            await UniTask.WhenAll(tasks);

            for (int i = 0; i < _wrapperList.Count; ++i) {
                _wrapperList[i].ShowByKey();
            }
        }
    }
}
