#if DEVELOPMENT || DEBUG_SIMPLEPROFILE_UI
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace OKGamesLib {

    /// <summary>
    /// プロファイルに最低限必要な数値を取得し表示する.
    /// </summary>
    public class SimpleProfiler : MonoBehaviour {

        /// <summary>
        /// プロファイラ情報を表示するテキスト.
        /// </summary>
        [SerializeField] protected TextWrapper text = null;

        /// <summary>
        /// 表示更新する時間間隔.
        /// </summary>
        [SerializeField] protected float updateInterval = 1.0f;

        /// <summary>
        /// 現在経過フレーム.
        /// </summary>
        private int _frameCount;

        /// <summary>
        ///　更新までの残り時間.
        /// </summary>
        private float _timeToUpdate;

        /// <summary>
        /// 更新までにかかった経過時間.
        /// </summary>
        private float _passedTime;

        private StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 毎フレーム行う処理.
        /// </summary>
        private void Update() {
            ++_frameCount;
            _timeToUpdate -= Time.deltaTime;
            _passedTime += Time.deltaTime;

            if (_timeToUpdate > 0) {
                return;
            }

            _timeToUpdate += updateInterval;

            var fps = _frameCount / _passedTime;
            _frameCount = 0;
            _passedTime = 0;

            var totalMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
            var usedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;
            var unusedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / 1024f / 1024f;

            Display(fps.ToString("0.0"), totalMemory.ToString("0.0"), usedMemory.ToString("0.0"), unusedMemory.ToString("0.0"));
        }

        /// <summary>
        /// プロファイル情報を表示する.
        /// </summary>
        /// <param name="fps">fps値.</param>
        /// <param name="totalMemory">使用できる合計メモリ量.</param>
        /// <param name="usedMemory">現在使用しているメモリ量.</param>
        /// <param name="unusedMemory">未使用のメモリ量.</param>
        protected virtual void Display(string fps, string totalMemory, string usedMemory, string unusedMemory) {
            _sb.Clear();
            _sb.Append($"        [FPS] {fps} \n");
            _sb.Append($"[Memory] {totalMemory} MB\n");
            _sb.Append($"      - Used {usedMemory} MB\n");
            text.SetText(_sb.ToString());
        }
    }
}
#endif
