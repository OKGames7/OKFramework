using OKGamesLib;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;


/// <summary>
/// ButtonWrapperの機能テスト用.
/// </summary>
public class TestButtonScene : MonoBehaviour {

    [SerializeField] private ButtonWrapper _wrapperTestAction = null;
    [SerializeField] private ButtonWrapper _wrapperTestFunction = null;

    private void Start() {
        TestAction();
        TestFunc();
    }

    //  ・ボタン押下時のActionが動くか.
    //　・同時押し防止が効くか(TestFuncでまだ処理を待っている最中にこちらのボタンをおして無視されればOK)
    private void TestAction() {
        Action<string> testAction = (string str) => {
            Log.Notice(str);
        };
        _wrapperTestAction.SetClickAction(testAction, "test");
    }

    //　・ボタン押下時のFuncが動くか
    //　・連打防止が効くかFuncでテスト(ボタンを押してから３秒以内にボタンを押しても反応しなくて3秒以降に押したらまた反応するようになればOK)
    private void TestFunc() {
        Func<int, UniTask> testFunc = async (int delay) => {
            Log.Notice($"非同期のテスト。処理が終わるまで待つ.処理フラグ:{_wrapperTestFunction.IsRunningProcess}");
            await UniTask.Delay(delay * 1000);
            Log.Notice($"処理終了した。処理フラグ:{_wrapperTestFunction.IsRunningProcess}");
        };
        _wrapperTestFunction.SetClickActionAsync(testFunc, 3);
    }
}
