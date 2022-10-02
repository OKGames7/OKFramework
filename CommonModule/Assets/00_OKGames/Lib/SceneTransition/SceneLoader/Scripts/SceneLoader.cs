using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // シーン遷移の機能や読み込んだシーンデータを保持しておくクラス.
    // ---------------------------------------------------------
    public static class SceneLoader {

        // TODO ISceneContextの中にargsを入れるようにする.
        // if (args == null) {
        //     // 引き継ぐデータの指定がなければシーン情報のみ格納しておく.
        //     args = new DefaultSceneDataArgs(_transitionManager.CurrentGameScene, additiveLoadScenes);
        // }



        // await _transitionManager.StartTransactionAsync();


    }




    // public static void LoadScene(GameScene scene,
    //     SceneDataArgs args = null,
    //     GameScene[] additiveLoadScenes = null,
    //     bool autoMove = true) {

    //     if (args == null) {
    //         // 引き継ぐデータの指定がなければシーン情報のみ格納しておく.
    //         args = new DefaultSceneDataArgs(_transitionManager.CurrentGameScene, additiveLoadScenes);
    //     }

    //     _transitionManager.StartTransactionAsync(scene, args, additiveLoadScenes, autoMove).Forget();

    // }

    // }
}
