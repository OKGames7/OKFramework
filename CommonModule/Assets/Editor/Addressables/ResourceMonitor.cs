using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace OKGamesLib {

    /// <summary>
    /// Addressablesで使用中のリソース一覧をモニタリングするエディタ拡張.
    /// </summary>
    public class ResourceMonitor : EditorWindow {

        private ResourceTreeView _treeView;

        /// <summary>
        /// エディタのツリービューのフィールドと相互作業すると更新される状態の情報.
        /// </summary>
        [SerializeField] private TreeViewState _treeViewState;


        /// <summary>
        /// EditorWindowsを表示する.
        /// </summary>
        [MenuItem("o.k.games/05.Addressables/ResourceMonitor", false)]
        private static void ShowWindow() {
            var window = EditorWindow.GetWindow<ResourceMonitor>("Resource");
            window.Show();
        }

        /// <summary>
        /// 活性化されたときの処理.
        /// </summary>
        private void OnEnable() {
            if (_treeViewState == null) {
                _treeViewState = new TreeViewState();
            }
            _treeView = new ResourceTreeView(_treeViewState);
        }

        /// <summary>
        /// UI表示.
        /// </summary>
        private void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All")) {
                _treeView.WatchAll();
            }

            if (GUILayout.Button("Prefab")) {
                Debug.Log("aa");
                _treeView.WatchGameObjects();
            }

            if (GUILayout.Button("Scriptabl Object")) {
                _treeView.WatchScriptableObjects();
            }

            if (GUILayout.Button("Sprite Atlas")) {
                _treeView.WatchSpriteAtlas();
            }

            if (GUILayout.Button("Audio Clip")) {
                _treeView.WatchAudioClips();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label(GetMemoryUsage());

            var treeRect = EditorGUILayout.GetControlRect(new GUILayoutOption[] {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true)
            });

            _treeView.OnGUI(treeRect);
        }

        /// <summary>
        /// 現在の使用メモリを取得する.
        /// </summary>
        /// <returns>メモリ使用量がわかる文字列.</returns>
        private string GetMemoryUsage() {
            float totalMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
            float usedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;
            float memory = _treeView.TotalMemory() / 1024f / 1024f;

            return $"[Total Reserved Memory] : {totalMemory.ToString("0.0")} MB  "
                 + $"[Used] : {usedMemory.ToString("0.0")} MB  "
                 + $"[PJ] : {memory.ToString("0.0")} MB";
        }
    }
}
