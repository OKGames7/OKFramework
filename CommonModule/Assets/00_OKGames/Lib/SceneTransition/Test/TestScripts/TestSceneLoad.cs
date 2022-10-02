using OKGamesLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;

// ---------------------------------------------------------
// シーン読み込みの挙動のテスト用.
// ---------------------------------------------------------
public class TestSceneLoad : MonoBehaviour {

    //     [SerializeField] private TextMeshProUGUI argsText = null;
    //     [SerializeField] private TextMeshProUGUI sceneText = null;

    //     private async UniTask Start() {

    //         // await UniTask.WaitUntil(() => GlobalContext.IsInitialized);

    //         // var args = SceneLoader.PreviousSceneArgs as ToTestTransitionSceneArgs;
    //         // if (args == null) {
    //         //     Log.Notice("引数は渡っていない");
    //         // }

    //         string debugText = "";
    //         if (args != null) {
    //             string testText = args.TestText;
    //             GameScene previousScene = args.PreviousGameScene;
    //             GameScene[] previousAdditiveScenes = args.PreviousAdditiveScenes;

    //             debugText = $"Args:\n testText: {testText}\n" + $"previousScene: {previousScene}\n" + "previousAdditiveScene: \n" + string.Join("\n", previousAdditiveScenes);
    //             Log.Notice(debugText);
    //         }
    //         // シーン引数の情報を表示する.
    //         argsText.text = debugText;

    //         List<string> scenesNameList = new List<string>();
    //         // 現在表示しているシーンを全て取得する.
    //         for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++) {
    //             //読み込まれているシーンを取得し、その名前をログに表示
    //             scenesNameList.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
    //         }
    //         sceneText.text += string.Join("\n", scenesNameList.ToArray());

    //         // シーン遷移の状態検知とイベント発行を例で記述.
    //         // シーンのロードが全て完了した際に発行される(フェードが開ける前).
    //         SceneLoader.OnSceneLoaded.Subscribe(x => {
    //             Log.Notice("遷移先に必要なシーンの読み込みが全て終了した");
    //         });
    //         // シーン遷移のトランジションの工程が全て完了した際に発行される.
    //         SceneLoader.OnTransitionFinished.Subscribe(x => {
    //             Log.Notice("トランジションの工程が全て終了した");
    //         });


    //     }

    //     /// <summary>
    //     /// 単純なシーン遷移.
    //     /// </summary>
    //     public void ChangeSimpleTest() {
    //         GameScene toScene = (SceneLoader.CurrentScene == GameScene.TestTransitionScene1) ? GameScene.TestTransitionScene2 : GameScene.TestTransitionScene1;
    //         SceneLoader.LoadScene(toScene);
    //     }

    //     /// <summary>
    //     /// 単純なシーン遷移でエンドを待つか確認.
    //     /// </summary>
    //     public void ChangeSimpleManualEndTest() {
    //         GameScene toScene = (SceneLoader.CurrentScene == GameScene.TestTransitionScene1) ? GameScene.TestTransitionScene2 : GameScene.TestTransitionScene1;
    //         SceneLoader.LoadScene(toScene, null, null, false);

    //         TransitionEndTestTask().Forget();
    //     }
    //     private async UniTask TransitionEndTestTask() {
    //         await UniTask.Delay(3000); //3秒.
    //         SceneLoader.End();
    //     }

    //     /// <summary>
    //     /// 単純なシーンで引数を渡した際のテスト.
    //     /// </summary>
    //     public void ChangeSimpleWithArgsTest() {
    //         GameScene toScene = (SceneLoader.CurrentScene == GameScene.TestTransitionScene1) ? GameScene.TestTransitionScene2 : GameScene.TestTransitionScene1;
    //         GameScene additiveScene = (toScene == GameScene.TestTransitionScene2) ? GameScene.TitleScene : GameScene.TestSaveScene;

    //         var args = new ToTestTransitionSceneArgs("TESTTEXT IS OK", SceneLoader.CurrentScene, new[] { additiveScene });
    //         SceneLoader.LoadScene(toScene, args);
    //     }

    //     /// <summary>
    //     /// Additiveでシーン読み込みするTest.
    //     /// </summary>
    //     public void ChangeAdditiveSceneTest() {
    //         GameScene toScene = (SceneLoader.CurrentScene == GameScene.TestTransitionScene1) ? GameScene.TestTransitionScene2 : GameScene.TestTransitionScene1;
    //         // 追加シーンは読み込まれるかさえ確認すればとりあえずOKなので適当に入れておく.
    //         GameScene additiveScene = (toScene == GameScene.TestTransitionScene2) ? GameScene.TitleScene : GameScene.TestSaveScene;
    //         SceneLoader.LoadScene(toScene, null, new[] { additiveScene });
    //     }

    //     /// <summary>
    //     /// Additiveで読み込むシーン + 前シーンからの引数渡しも含めたテスト.
    //     /// </summary>
    //     public void ChangeAdditiveSceneWithArgsTest() {
    //         GameScene toScene = (SceneLoader.CurrentScene == GameScene.TestTransitionScene1) ? GameScene.TestTransitionScene2 : GameScene.TestTransitionScene1;

    //         // 追加シーンは読み込まれるかさえ確認すればとりあえずOKなので適当に入れておく.
    //         GameScene[] additiveScenes = (toScene == GameScene.TestTransitionScene2) ? new[] { GameScene.TitleScene } : new[] { GameScene.TestSaveScene, GameScene.TitleScene };

    //         var args = new ToTestTransitionSceneArgs(
    //             toScene == GameScene.TestTransitionScene1 ? "TESTTEXT IS FROM1" : "TESTTEXT IS FROM2",
    //             SceneLoader.CurrentScene,
    //             additiveScenes);

    //         SceneLoader.LoadScene(toScene, args, additiveScenes);
    //     }
}
