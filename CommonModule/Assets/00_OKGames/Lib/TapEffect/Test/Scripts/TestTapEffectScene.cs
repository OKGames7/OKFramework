using UnityEngine;
using TMPro;

// ---------------------------------------------------------
// タップエフェクトの挙動テスト用
// ---------------------------------------------------------
public class TestTapEffectScene : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI text = null;

    /// <summary>
    /// ゲームをポーズした時もタップエフェクトが出るかチェックする.
    /// timescaleを0にするやり方だと0にしているときにタップしてもタップエフェクトは出ない。
    /// TODO: ポーズしているかプレイしているかのSubjectを発行してポーズさせる必要があるオブジェクトには別途ポーズ時の挙動を仕込む形にしたほうが良いかもしれない。
    /// </summary>
    public void PoseGame() {
        Time.timeScale = 0f;
        text.text = "State: POSE";
    }

    /// <summary>
    /// ゲームプレイの再開.
    /// </summary>
    public void PlayGame() {
        Time.timeScale = 1.0f;
        text.text = "State: PLAY";
    }
}
