using OKGamesFramework;
using OKGamesLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJ {

    /// <summary>
    /// OKGamesの機能を中継するBridge.
    /// </summary>
    public class FWBridge {

        public ISceneDirector SceneDirector => OKGames.SceneDirector;

        public IResourceStore ResourceStore => OKGames.ResourceStore;

        public IAudioPlayer BgmPlayer => OKGames.BgmPlayer;

        public IAudioPlayer SePlayer => OKGames.SePlayer;




        public IAdmob Admob => OKGames.Ads;

        public BootType GetBootType() {
            if (!(OKGames.IsInit && OKGames.IsPJRoutine)) {
                return BootType.NotReady;
            } else if (OKGames.IsInit && !OKGames.IsPJRoutine) {
                return BootType.CommonModuleDebug;
            } else {
                return BootType.PJRoutine;
            }
        }
    }
}
