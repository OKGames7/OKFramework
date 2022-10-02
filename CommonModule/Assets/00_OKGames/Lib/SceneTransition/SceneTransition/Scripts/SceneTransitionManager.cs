using OKGamesFramework;
using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;
using UnityEngine;
using UnityEngine.SceneManagement;

// ---------------------------------------------------------
// シーン遷移を管理する.
// ---------------------------------------------------------
// public class SceneTransitionManager : MonoBehaviour, INowLoading {

// // フェードのアニメーションを管理するクラス.
// [SerializeField] private CanvasSetter _fadeCanvasSetter;
// [SerializeField] private Fade _fade;

// private float _fadeTime = 0.5f;

// private INowLoading _nowLoading = null;

// // シーン遷移処理を実行中か.
// private bool _isRunning = false;
// public bool IsRunnning { get { return _isRunning; } }

// // トランジションアニメーションを終了させて良いか.
// // (フェードが開くアニメーションを再生して良いか).
// private ReactiveProperty<bool> _canEndTransition = new ReactiveProperty<bool>(false);

// public GameScene CurrentGameScene { get { return _currentGameScene; } }


// //* ↓↓↓シーン遷移中の状態に関する変数↓↓ *//

// // トランジションの一連の流れの終了通知.
// // フェードが開き切ったり、閉じ切ったことを通知する.
// private Subject<Unit> _onTransactionFinishedInternal = new Subject<Unit>();

// // 全シーンのロードが完了したことを通知する.
// private Subject<Unit> _onAllSceneLoaded = new Subject<Unit>();
// public IObservable<Unit> OnSceneLoaded { get { return _onAllSceneLoaded; } }

// // トランジションが終了し、シーンが開始したことを通知する.
// // OnCompletedもセットで発行する.
// public IObservable<Unit> OnTransitionAnimationFinished {
//     get {
//         if (_isRunning) {
//             return _onTransactionAnimationFinishedSubject.FirstOrDefault();
//         } else {
//             // シーン遷移を実行していないなら既値を返却.
//             return Observable.Return(Unit.Default);
//         }
//     }
// }

// // シーン遷移の全ての流れ(フェードイン、シーン切り替え、データ格納、フェードアウト)が終了したことを通知する.
// private Subject<Unit> _onTransactionAnimationFinishedSubject = new Subject<Unit>();

// /// <summary>
// /// Start処理
// /// </summary>
// private void Start() {
//     _canEndTransition.AddTo(this);
//     _onTransactionFinishedInternal.AddTo(this);
//     _onAllSceneLoaded.AddTo(this);
//     _onTransactionAnimationFinishedSubject.AddTo(this);
// }

// // 初期化.
// public void Init(INowLoading nowLoading) {
//     _nowLoading = nowLoading;

//     try {
//         _currentGameScene = (GameScene)Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name, false);
//     }
//     catch {
//         // Debug用のシーンなどで、BuildSettingsに登録されていないシーンの場合.
//         Log.Notice("現在のシーンの取得に失敗しました");
//         // Debugシーンの場合はとりあえずそのシーンで挙動すればOKなので、適当にタイトルシーンを入れておく.
//         _currentGameScene = GameScene.TitleScene;
//     }

//     // トランジションアニメーションが終了したイベントをObservableに変換する.
//     // トランジションの終了を待機してゲームを開始することを想定して
//     // 初期化直後にシーン遷移完了通知を発行する(デバッグで任意のシーンからゲーム開始できるように)
//     _onTransactionFinishedInternal.OnNext(Unit.Default);
//     _onAllSceneLoaded.OnNext(Unit.Default);

// }

// public async UniTask StartTransactionAsync(
//     string nextSceneName,
//     SceneDataArgs args,
//     GameScene[] additiveLoadSenes,
//     bool autoMove) {

//     if (_isRunning) {
//         return;
//     }

//     var ct = this.GetCancellationTokenOnDestroy();
//     await TransitionTask(nextSceneName, args, additiveLoadSenes, autoMove, ct).Forget();
// }

// private async UniTask TransitionTask(
//     string nextSceneName,
//     SceneDataArgs args,
//     GameScene[] additiveLoadSenes,
//     bool autoMove,
//     CancellationToken token) {

//     // 処理開始フラグのセット.
//     _isRunning = true;

//     // トランジションの自動遷移設定.
//     _canEndTransition.Value = autoMove;

//     // フェードイン.
//     _fade.FadeIn(
//         _fadeTime,
//         () => _onTransactionFinishedInternal.OnNext(Unit.Default)
//         ).Forget();

//     // フェードイン完了まで待機.
//     await _onTransactionFinishedInternal.FirstOrDefault();
//     Log.Notice("フェードイン完了");

//     // 前のシーンから受け取った情報を格納.
//     SceneLoader.PreviousSceneArgs = args;

//     // メインシーンを単体で読み込んで待つ.
//     await SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single)
//         .ToUniTask(Progress.Create<float>(x =>
//             _nowLoading.Show(x)),
//             cancellationToken: token);

//     if (additiveLoadSenes != null) {
//         // 追加シーンがある場合は読み込んで全て読み終わるまで待つ.
//         await additiveLoadSenes.Select(scene =>
//         SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive)
//         .AsObservable(Progress.Create<float>(x =>
//             _nowLoading.Show(x))))
//         .WhenAll();
//     }

//     // リソースの解放やGCが効かない場合があるので1フレ待つ.
//     await UniTask.DelayFrame(1, cancellationToken: token);
//     var resourceUnload = Resources.UnloadUnusedAssets();
//     GC.Collect();

//     // リソース解放を待つ.
//     await resourceUnload;

//     // 現在シーン情報を格納.
//     _currentGameScene = nextSceneName;

//     // シーンロードの完了通知を発行.
//     _onAllSceneLoaded.OnNext(Unit.Default);

//     if (!autoMove) {
//         // 自動遷移しない設定の場合はフラグがtrueに変化するまで待機する.
//         await UniTask.WaitUntil(() => _canEndTransition.Value);
//     }
//     _canEndTransition.Value = false;

//     // シーンの読み込みが終わったのでNowLoadingは非表示にする.
//     _nowLoading.Close();

//     // フェードアウト.
//     _fade.FadeOut(
//         _fadeTime,
//         () => _onTransactionFinishedInternal.OnNext(Unit.Default)
//         ).Forget();

//     // フェードアウト完了を待つ.
//     await _onTransactionFinishedInternal.FirstOrDefault();
//     Log.Notice("フェードアウト完了");

//     // トランジションの一連の流れが全て完了したことを通知する.
//     _onTransactionAnimationFinishedSubject.OnNext(Unit.Default);

//     // 終了.
//     _isRunning = false;
// }

// /// <summary>
// /// NowLoadingの表示をする.
// /// </summary>
// void INowLoading.Show(float progress) {
//     _nowLoading.Show(progress);
// }
// /// <summary>
// /// NowLoadingを非表示にする.
// /// </summary>
// void INowLoading.Close() {
//     _nowLoading.Close();
// }


// /// <summary>
// /// トランジションアニメーションを終了させる.
// /// (AutoMove=falseを指定した場合に予備出す必要がある.)
// /// </summary>
// public void End() {
//     _canEndTransition.Value = true;
// }
// }
