using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// EditorWindow内に表示するAddressablesのリソース使用状況を表示するためのツリービュー.
    /// </summary>
    public class ResourceTreeView : TreeView {

        private List<ResourceTreeViewItem> _itemList = new List<ResourceTreeViewItem>();
        private ResourceCollector _collector = new ResourceCollector();

        /// <summary>
        /// <see cref="ResourceTreeViewItem"/>の合計メモリ量の取得.
        /// </summary>
        /// <returns>合計メモリ量</returns>
        public long TotalMemory() {
            long memorySum = 0;
            _itemList.ForEach(item => memorySum += item.MemorySize);
            return memorySum;
        }

        /// <summary>
        /// 全てAddressablesの使用アセットの情報を集積する.
        /// </summary>
        public void WatchAll() {
            var items = _collector.CollectAll();
            if (items == null) {
                return;
            }
            _itemList = items;
            ReloadAndSort();
        }

        /// <summary>
        /// Addressablesの使用リソースのうち、GameObjectの使用アセット情報を集積する.
        /// </summary>
        public void WatchGameObjects() {
            var items = _collector.CollectGameObjects();
            if (items == null) {
                Debug.Log("null");
                return;
            }
            Debug.Log("nullではない");

            _itemList = items;
            ReloadAndSort();
        }

        /// <summary>
        /// Addressablesの使用リソースのうち、ScriptableObjectの使用アセット情報を集積する.
        /// </summary>
        public void WatchScriptableObjects() {
            var items = _collector.CollectScriptableObjects();
            if (items == null) {
                return;
            }

            _itemList = items;
            ReloadAndSort();
        }

        /// <summary>
        /// Addressablesの使用リソースのうち、SpriteAtlasの使用アセット情報を集積する.
        /// </summary>
        public void WatchSpriteAtlas() {
            var items = _collector.CollectSprites();
            if (items == null) {
                return;
            }

            _itemList = items;
            ReloadAndSort();
        }

        /// <summary>
        /// Addressablesの使用リソースのうち、AudioClipの使用アセット情報を集積する.
        /// </summary>
        public void WatchAudioClips() {
            var items = _collector.CollectAudioClips();
            if (items == null) {
                return;
            }
            _itemList = items;
            ReloadAndSort();
        }

        /// <summary>
        /// ヘッダーのカラム情報.
        /// </summary>
        private static readonly MultiColumnHeaderState.Column[] headerColumns = new[] {
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Category"),   width = 15 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Id"),         width =  5 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Asset Name"), width = 20 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Ref"),        width =  5 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("gRef"),       width =  5 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Loaded"),     width =  7 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Memory"),     width = 10 },
            new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Info"),       width = 15 },
        };

        /// <summary>
        /// コントラクタ.
        /// </summary>
        /// <param name="state">新規生成した<see cref="TreeViewState"/>.</param>
        public ResourceTreeView(TreeViewState state) : this(state, new MultiColumnHeader(new MultiColumnHeaderState(headerColumns))) {
        }

        /// <summary>
        /// コントラクタ.
        /// </summary>
        /// <param name="state">新規生成した<see cref="TreeViewState"/>.</param>
        /// <param name="header">EditorWindowsで表示するカラム情報を持つヘッダ.</param>
        private ResourceTreeView(TreeViewState state, MultiColumnHeader header) : base(state, header) {
            rowHeight = 20;
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            header.sortingChanged += OnSortingChanged;
            header.ResizeToFit();
            Reload();
        }

        /// <summary>
        ///  RootItem情報の生成.
        /// </summary>
        /// <returns>TreeViewItem</returns>
        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem {
                id = 0,
                depth = -1,
                displayName = "Root"
            };

            if (!EditorApplication.isPlaying) {
                var emptyItem = new List<TreeViewItem>();
                SetupParentsAndChildrenFromDepths(root, emptyItem);
                return root;
            }

            root.children = _itemList.Cast<TreeViewItem>().ToList();
            return root;
        }

        /// <summary>
        /// レコードのUI表示.
        /// </summary>
        /// <param name="args"></param>
        protected override void RowGUI(RowGUIArgs args) {
            var item = args.item as ResourceTreeViewItem;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i) {
                var rect = args.GetCellRect(i);
                var colIndex = args.GetColumn(i);
                var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
                labelStyle.alignment = TextAnchor.MiddleLeft;

                switch (colIndex) {
                    case 0: EditorGUI.LabelField(rect, item.Category, labelStyle); break;
                    case 1: EditorGUI.LabelField(rect, item.id.ToString(), labelStyle); break;
                    case 2: EditorGUI.LabelField(rect, item.AssetName, labelStyle); break;
                    case 3: EditorGUI.LabelField(rect, item.RefCount.ToString(), labelStyle); break;
                    case 4: EditorGUI.LabelField(rect, item.GlobalRefCount.ToString(), labelStyle); break;
                    case 5:
                        string loadState = item.Loaded ? "Yes" : "No";
                        EditorGUI.LabelField(rect, loadState, labelStyle);
                        break;
                    case 6:
                        string memorySize = item.MemorySize > 0
                            ? (item.MemorySize / 1024f / 1024f).ToString("0.000") + " MB"
                            : "-";
                        EditorGUI.LabelField(rect, memorySize, labelStyle);
                        break;
                    case 7: EditorGUI.LabelField(rect, item.Info, labelStyle); break;
                }
            }
        }

        /// <summary>
        /// ViewItemのリストの表示をソートする時に呼び出す処理.
        /// </summary>
        /// <param name="multiColumnHeader">多重カラムを含むヘッダの情報.</param>
        private void OnSortingChanged(MultiColumnHeader multiColumnHeader) {
            int index = multiColumnHeader.sortedColumnIndex;
            if (index == -1) {
                return;
            }

            bool isAsc = multiColumnHeader.IsSortedAscending(index);

            var items = rootItem.children.Cast<ResourceTreeViewItem>();
            IOrderedEnumerable<ResourceTreeViewItem> orderedItems = null;
            switch (index) {
                case 0:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.Category)
                        : items.OrderByDescending(item => item.Category);
                    break;
                case 1:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.id)
                        : items.OrderByDescending(item => item.id);
                    break;
                case 2:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.AssetName)
                        : items.OrderByDescending(item => item.AssetName);
                    break;
                case 3:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.RefCount)
                        : items.OrderByDescending(item => item.RefCount);
                    break;
                case 4:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.GlobalRefCount)
                        : items.OrderByDescending(item => item.GlobalRefCount);
                    break;
                case 5:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.Loaded)
                        : items.OrderByDescending(item => item.Loaded);
                    break;
                case 6:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.MemorySize)
                        : items.OrderByDescending(item => item.MemorySize);
                    break;
                case 7:
                    orderedItems = isAsc
                        ? items.OrderBy(item => item.Info)
                        : items.OrderByDescending(item => item.Info);
                    break;
            }

            _itemList = orderedItems.ToList();
            rootItem.children = _itemList.Cast<TreeViewItem>().ToList();
            BuildRows(rootItem);
        }

        /// <summary>
        /// TreeViewをリロードしソートをかける.
        /// </summary>
        private void ReloadAndSort() {
            var currentSelected = state.selectedIDs;
            Reload();
            OnSortingChanged(multiColumnHeader);
            state.selectedIDs = currentSelected;
        }
    }
}
