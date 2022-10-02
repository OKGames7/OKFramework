using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    ///
    /// </summary>
    public class UI : IUI {
        public IFontLoader FontLoader { get; private set; }
        public IButtonWatcher ButtonWatcher { get; private set; }

        public void Init() {
            FontLoader = new FontLoader();
            ButtonWatcher = new ButtonWatcher();
        }
    }
}
