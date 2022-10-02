using OKGamesFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace OKGamesLib {

    /// <summary>
    /// お気に入り登録する際のアセット情報.
    /// </summary>
    [System.Serializable]
    public class AssetInfo {
        public string Guid;
        public string Path;
        public string Name;
        public string Type;
    }

    /// <summary>
    /// お気に入り登録するアセットの情報のリスト.
    /// </summary>
    [System.Serializable]
    public class AssetInfoList {
        public List<AssetInfo> InfoList = new List<AssetInfo>();
    }

    /// <summary>
    /// 任意のファイルを選択した状態でお気に入り登録すると、ウィンドウに該当項目が登録される。
    /// その後、その項目を押すとProjectViewで該当ファイルにすぐ飛べたり、シーンなどであればすぐに開いたりすることができる.
    /// </summary>
    public class FavoritesWindow : EditorWindow {

        /// <summary>
        /// お気に入り登録が多くなってきたらスクロールできるようにする.
        /// </summary>
        private Vector2 _scrollView;

        /// <summary>
        /// 最後に開いたアセットがあればその項目を目立たせるために記憶しておく.
        /// </summary>
        private AssetInfo _lastOpenedAsset;

        /// <summary>
        /// お気に入り登録しているアセット情報のキャッシュ.
        /// </summary>
        [SerializeField]
        private static AssetInfoList _assetListCache = null;
        private static AssetInfoList _assetList {
            get {
                if (_assetListCache == null) {
                    _assetListCache = LoadPrefs();
                }
                return _assetListCache;
            }
        }

        /// <summary>
        /// エディタ拡張したウィンドウを表示する.
        /// </summary>
        [MenuItem("o.k.games/99.UnityEditor/FavoritesWindow")]
        private static void ShowWindow() {
            GetWindow<FavoritesWindow>("★ Favorites");
        }

        /// <summary>
        /// お気に入り登録しているアセットはEditorPrefsに登録している
        /// そのためのセーブ、取得キー.
        /// </summary>
        /// <returns>キー情報.</returns>
        private static string PrefsKey() {
            return $"{Application.productName}-Favs";
        }

        /// <summary>
        /// 現在のお気に入り登録の情報をローカルセーブする.
        /// </summary>
        private static void SavePrefs() {
            string prefsJson = JsonUtility.ToJson(_assetList);
            EditorPrefs.SetString(PrefsKey(), prefsJson);
        }

        /// <summary>
        /// EditorPrefsからお気に入り情報リストの情報を読み込む.
        /// </summary>
        /// <returns>お気に入り情報リスト.</returns>
        private static AssetInfoList LoadPrefs() {
            Log.Notice("Loading Favorites Prefs...");

            string prefsKey = PrefsKey();
            if (!EditorPrefs.HasKey(prefsKey)) {
                return new AssetInfoList();
            }

            string prefsJson = EditorPrefs.GetString(prefsKey);
            var assets = JsonUtility.FromJson<AssetInfoList>(prefsJson);
            if (assets == null) {
                Log.Error("Error Favorites Prefs Load");
                return new AssetInfoList();
            }

            return assets;
        }

        /// <summary>
        /// UI表示する.
        /// </summary>
        private void OnGUI() {
            GUILayout.BeginHorizontal();
            {
                var content = new GUIContent("* Fav", "Bookmark selected asset");
                if (GUILayout.Button(content, GUILayout.Width(100), GUILayout.Height(40))) {
                    BookmarkAsset();
                }

                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("▼ Sort by Type", GUILayout.MaxWidth(200))) {
                        SortByType();
                        SavePrefs();
                    }

                    if (GUILayout.Button("▼ Sort by Name", GUILayout.MaxWidth(200))) {
                        SortByName();
                        SavePrefs();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            _scrollView = GUILayout.BeginScrollView(_scrollView);
            {
                foreach (var info in _assetList.InfoList) {
                    GUILayout.BeginHorizontal();
                    {
                        bool isCanceled = DrawAssetRow(info);
                        if (isCanceled) {
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// ウィンドウにアセット項目を表示する.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool DrawAssetRow(AssetInfo info) {
            bool isCanceled = false;
            {
                var content = new GUIContent(" ◎ ", "Highlight asset");
                if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.Height(32))) {
                    HighlightAsset(info);
                }
            }
            {
                DrawAssetItemButton(info);
            }
            {
                var content = new GUIContent("X", "Remove from Favs");
                if (GUILayout.Button(content, GUILayout.ExpandWidth(false))) {
                    RemoveAsset(info);
                    isCanceled = true;
                }
            }
            return isCanceled;
        }

        /// <summary>
        /// アセットアイコンとアセット名が一つになっているボタンの表示.
        /// </summary>
        /// <param name="info"></param>
        private void DrawAssetItemButton(AssetInfo info) {
            var content = new GUIContent($" {info.Name}", AssetDatabase.GetCachedIcon(info.Path));
            var style = GUI.skin.button;
            var originalAlignment = style.alignment;
            var originalFontStyle = style.fontStyle;
            var originalTextColor = style.normal.textColor;
            style.alignment = TextAnchor.MiddleLeft;

            if (info == _lastOpenedAsset) {
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.yellow;
            }

            float width = position.width - 70f;
            if (GUILayout.Button(content, style, GUILayout.MaxWidth(width), GUILayout.Height(32))) {
                OpenAsset(info);
            }

            style.alignment = originalAlignment;
            style.fontStyle = originalFontStyle;
            style.normal.textColor = originalTextColor;
        }

        /// <summary>
        /// ProjectViewで選択中のアセットをお気に入り登録する.
        /// </summary>
        private void BookmarkAsset() {
            foreach (string assetGuid in Selection.assetGUIDs) {
                if (_assetList.InfoList.Exists(x => x.Guid == assetGuid)) {
                    continue;
                }

                var info = new AssetInfo();
                info.Guid = assetGuid;
                info.Path = AssetDatabase.GUIDToAssetPath(assetGuid);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
                info.Name = asset.name;
                info.Type = asset.GetType().ToString();
                _assetList.InfoList.Add(info);
            }
            SavePrefs();
        }

        /// <summary>
        /// お気に入り登録から指定したアセットを削除する.
        /// </summary>
        /// <param name="info">お気に入り削除するアセット.</param>
        private void RemoveAsset(AssetInfo info) {
            _assetList.InfoList.Remove(info);
            SavePrefs();
        }

        /// <summary>
        /// 指定したアセットの項目でProjectView上をハイライトする(表示をそのアセットに飛ばす).
        /// </summary>
        /// <param name="info"></param>
        private void HighlightAsset(AssetInfo info) {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
            EditorGUIUtility.PingObject(asset);
        }

        /// <summary>
        /// 指定したアセットを開く.
        /// </summary>
        /// <param name="info"></param>
        private void OpenAsset(AssetInfo info) {
            if (info.Type != "UnityEditor.DefaultAsset") {
                // 最後に開いたアセットがなければ↑のタイプになる.
                _lastOpenedAsset = info;
            }

            if (Path.GetExtension(info.Path).Equals(".unity")) {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    EditorSceneManager.OpenScene(info.Path, OpenSceneMode.Single);
                }
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
            AssetDatabase.OpenAsset(asset);
        }

        /// <summary>
        /// お気に入りリストを型でソートする.
        /// </summary>
        private void SortByType() {
            SortByName();
            _assetList.InfoList.Sort((a, b) => {
                return a.Type.CompareTo(b.Type);
            });
        }

        /// <summary>
        /// お気に入りリストを名前でソートする.
        /// </summary>
        private void SortByName() {
            _assetList.InfoList.Sort((a, b) => {
                return a.Name.CompareTo(b.Name);
            });
        }
    }
}
