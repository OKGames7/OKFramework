using OKGamesFramework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.U2D;

namespace OKGamesLib {
    /// <summary>
    /// ResourceStoreからリソース情報を収集する.
    /// </summary>
    public class ResourceCollector {

        /// <summary>
        /// 全ての使用リソース情報を集積する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報.</returns>
        public List<ResourceTreeViewItem> CollectAll() {
            if (!IsReady()) {
                return null;
            }

            var items = new List<ResourceTreeViewItem>();
            DoCollectGameObjects(items);
            DoCollectScriptableObjects(items);
            DoCollectSpriteAtlases(items);
            DoCollectAudioClips(items);
            AssignId(items);
            return items;
        }

        /// <summary>
        /// GameObjectの使用リソース情報を集積する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報.</returns>
        public List<ResourceTreeViewItem> CollectGameObjects() {
            if (!IsReady()) {
                Debug.Log("準備不足");
                return null;
            }
            Debug.Log("準備不足ではない");


            var items = new List<ResourceTreeViewItem>();
            DoCollectGameObjects(items);
            AssignId(items);
            return items;
        }

        /// <summary>
        /// ScririptableObjectの使用リソース情報を集積する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報.</returns>
        public List<ResourceTreeViewItem> CollectScriptableObjects() {
            if (!IsReady()) {
                return null;
            }

            var items = new List<ResourceTreeViewItem>();
            DoCollectScriptableObjects(items);
            AssignId(items);
            return items;
        }

        /// <summary>
        /// Spritetの使用リソース情報を集積する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報.</returns>
        public List<ResourceTreeViewItem> CollectSprites() {
            if (!IsReady()) {
                return null;
            }

            var items = new List<ResourceTreeViewItem>();
            DoCollectSpriteAtlases(items);
            AssignId(items);
            return items;
        }

        /// <summary>
        /// AudioClipの使用リソース情報を集積する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報.</returns>
        public List<ResourceTreeViewItem> CollectAudioClips() {
            if (!IsReady()) {
                return null;
            }

            var items = new List<ResourceTreeViewItem>();
            DoCollectAudioClips(items);
            AssignId(items);
            return items;
        }

        /// <summary>
        /// 集積する準備が整っているか.
        /// </summary>
        /// <returns>整っているか.</returns>
        private bool IsReady() {
            return (EditorApplication.isPlaying && OKGames.Context != null);
        }

        /// <summary>
        /// 集積したList用Itemに対しidを割り振る.
        /// </summary>
        /// <param name="itemList">使用リソース情報.</param>
        private void AssignId(List<ResourceTreeViewItem> itemList) {
            for (int i = 0; i < itemList.Count; ++i) {
                itemList[i].id = i;
            }
        }

        /// <summary>
        /// GameObjectの使用リソース情報を集積する実処理.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報の蓄積リスト.</returns>
        private void DoCollectGameObjects(List<ResourceTreeViewItem> itemList) {
            var entries = OKGames.ResourceStore.Registry.GetEntryList().Where(x => {
                return x.Type == typeof(GameObject);
            });

            foreach (var entry in entries) {
                var resource = OKGames.ResourceStore.GetGameObj(entry.Address);
                long memorySize = GetGameObjectMemory(resource);
                itemList.Add(MakeItem(entry, "Prefab", resource, memorySize));
            }
        }

        /// <summary>
        /// ScriptableObjecttの使用リソース情報を集積する実処理.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報の蓄積リスト.</returns>
        private void DoCollectScriptableObjects(List<ResourceTreeViewItem> items) {
            var entries = OKGames.ResourceStore.Registry.GetEntryList().Where(x => {
                return x.Type.IsSubclassOf(typeof(ScriptableObject));
            });

            foreach (var entry in entries) {
                var resource = OKGames.ResourceStore.GetObj<ScriptableObject>(entry.Address);
                long memorySize = GetScriptableObjectMemory(resource);
                items.Add(MakeItem(entry, "Scriptable Object", resource, memorySize));
            }
        }

        /// <summary>
        /// SpriteAtlasの使用リソース情報を集積する実処理.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報の蓄積リスト.</returns>
        private void DoCollectSpriteAtlases(List<ResourceTreeViewItem> items) {
            var entries = OKGames.ResourceStore.Registry.GetEntryList().Where(x => {
                return x.Type == typeof(SpriteAtlas);
            });

            foreach (var entry in entries) {
                var resource = OKGames.ResourceStore.GetSpriteAtlas(entry.Address);
                long memorySize = GetSpriteAtlasTextureMemory(resource);
                items.Add(MakeItem(entry, "Sprite Atlas", resource, memorySize));
            }
        }

        /// <summary>
        /// SudioClipの使用リソース情報を集積する実処理.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報の蓄積リスト.</returns>
        private void DoCollectAudioClips(List<ResourceTreeViewItem> items) {
            var entries = OKGames.ResourceStore.Registry.GetEntryList().Where(x => {
                return x.Type == typeof(AudioClip);
            });

            foreach (var entry in entries) {
                var resource = OKGames.ResourceStore.GetAudio(entry.Address);
                items.Add(MakeItem(entry, "Audio Clip", resource));
            }
        }

        /// <summary>
        /// Addressables GroupのEntryの情報からEditorWindow内のTreeViewで表示するためのItem情報を生成する.
        /// </summary>
        /// <returns>EditorWindowsの表示に使用する情報の蓄積リスト.</returns>
        private ResourceTreeViewItem MakeItem(
            ResourceEntry entry, string category, Object resource,
            long? _memorySize = null
        ) {
            long genericMemorySize = (resource == null) ? -1 : Profiler.GetRuntimeMemorySizeLong(resource);
            long memorySize = _memorySize ?? genericMemorySize;
            return new ResourceTreeViewItem(0) {
                Category = category,
                AssetName = entry.Address,
                RefCount = entry.SceneScopeRefCount,
                GlobalRefCount = entry.GlobalScopeRefCount,
                Loaded = entry.Loaded,
                MemorySize = memorySize,
            };
        }

        /// <summary>
        /// SpriteAtlasの使用メモリ量を計算して返す.
        /// </summary>
        /// <param name="atlas">使用しているアトラス.</param>
        /// <returns>メモリ量.</returns>
        private long GetSpriteAtlasTextureMemory(SpriteAtlas atlas) {
            if (atlas.spriteCount == 0) {
                return 0;
            }

            Sprite[] sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            return Profiler.GetRuntimeMemorySizeLong(sprites[0].texture);
        }

        /// <summary>
        /// GameObjectの使用メモリ量を計算して返す.
        /// </summary>
        /// <param name="obj">使用しているGameObject.</param>
        /// <returns>メモリ量.</returns>
        private int GetGameObjectMemory(GameObject obj) {
            string json = JsonUtility.ToJson(obj);
            Encoding utf8 = Encoding.UTF8;
            return utf8.GetByteCount(json);
        }

        /// <summary>
        /// ScriptableObjectの使用メモリ量を計算して返す.
        /// </summary>
        /// <param name="obj">使用しているScriptableObject.</param>
        /// <returns>メモリ量.</returns>
        private int GetScriptableObjectMemory(ScriptableObject obj) {
            string json = JsonUtility.ToJson(obj);
            Encoding utf8 = Encoding.UTF8;
            return utf8.GetByteCount(json);
        }
    }
}
