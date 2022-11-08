using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// UI全般に関するアクセスポイント.
    /// </summary>
    public class UI : IUI {
        public IFontLoader FontLoader { get; private set; }
        public IButtonAdapter ButtonAdapter { get; private set; }

        public ITextAdapter TextAdapter { get; private set; }

        public void Init() {
            FontLoader = new FontLoader();
            ButtonAdapter = new ButtonAdapter();
            TextAdapter = new TextAdapter();
        }

        public void Inject(IUITransfer transfar) {
            FontLoader.Inject(transfar);
            ButtonAdapter.Inject(transfar);
            TextAdapter.Inject(transfar, FontLoader);
        }

        public void UpdateTexts(Language language) {
            TextAdapter.UpdateTexts(language);
        }
    }
}
