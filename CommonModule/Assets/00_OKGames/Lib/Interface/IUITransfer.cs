using UnityEngine;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// <see cref="UI"/>への依存性注入のためのデータを移送するためのクラスのインターフェース.
    /// </summary>
    public interface IUITransfer {

        IReadOnlyReactiveProperty<UserData> UserData { get; }

        IResourceStore ResourceStore { get; }

        Language Lang { get; }

        Entity_text TextMaster { get; }

        IAudioPlayer SePlayer { get; }

    }
}
