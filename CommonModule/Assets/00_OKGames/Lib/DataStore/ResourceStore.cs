using OKGamesLib;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace OKGamesLib {

    /// <summary>
    /// Addressablesのアセットを管理する大元のクラス.
    /// </summary>
    /// <remarks>
    /// 使い方
    /// ・外部クラスから、読み込みたいアセットをRetain or RetainGlobal or RetainGlobalWithAutoLoadする.
    /// 　- RetainGlobalWithAutoLoadは、RegistryへのEntryの追加、Assetの生成、キャッシュ用辞書への登録までをすぐに行う.
    /// ・Retain or RetainGlobalをした場合はその後、Load関数を外部から読んだタイミングでそれらのアセット取得する.
    /// ・以降は、Get~でアセットを取得することができる
    ///
    /// ※メモリリークするので、アセットは不要になれば破棄を忘れないように(以下手順).
    /// ・アセットが不要になれば範囲に合わせてRelease or ReleaseGlobal or ReleaseGlobalWithAutoUnload or ReleaseAllSceneScoped or ReleaseAllGlobalScopedを呼ぶ.
    /// 　- ReleaseGlobalWithAutoUnloadは、Registryから参照を減らして、Addressables.Releseして、~Storeのキャッシュを消して、RegistryもUnmarkにする.
    /// ・他のメソッドの場合は、その後のUnload関数を呼んだタイミングで解放される.
    /// </remarks>
    public class ResourceStore : IResourceStore {

        public ResourceRegistry Registry => _registry;
        private ResourceRegistry _registry = new ResourceRegistry();

        private GameObjectStore _gameObjStore = new GameObjectStore();
        private ScriptableObjectStore _scriptableObjStore = new ScriptableObjectStore();
        private AudioClipStore _audioStore = new AudioClipStore();
        private SpriteAtlasStore _spriteStore = new SpriteAtlasStore();

        /// <summary>
        /// アセットの読み込み中か.
        /// </summary>
        private bool _isLoading = false;

        /// <summary>
        /// アセット読み込み失敗時のリトライのトライ数.
        /// </summary>
        private int _loadRetryCount = 0;

        public void Retain(params string[] assetAddresses) {
            foreach (var address in assetAddresses) {
                _registry.Retain(address);
            }
        }

        public void RetainGlobal(params string[] assetAddresses) {
            foreach (var address in assetAddresses) {
                _registry.Retain(address, isGlobalScope: true);
            }
        }

        public async UniTask RetainGlobalWithAutoLoad(params string[] assetAddresses) {
            RetainGlobal(assetAddresses);
            await LoadMulti(assetAddresses);
        }

        public void Release(params string[] assetAddresses) {
            foreach (var address in assetAddresses) {
                _registry.Release(address);
            }
        }

        public void ReleaseGlobal(params string[] assetAddresses) {
            foreach (var address in assetAddresses) {
                _registry.Release(address, isGlobalScope: true);
            }
        }

        public void ReleaseGlobalWithAutoUnLoad(params string[] assetAddresses) {
            ReleaseGlobal(assetAddresses);
            UnloadMulti(assetAddresses);
        }

        public void ReleaseAllSceneScoped() {
            _registry.ReleaseAll();
        }

        public void ReleaseAllGlobalScoped() {
            _registry.ReleaseAll(isGlobalScope: true);
        }

        public bool Contains(string assetAddress) {
            return _gameObjStore.Contains(assetAddress)
                || _scriptableObjStore.Contains(assetAddress)
                || _audioStore.Contains(assetAddress)
                || _spriteStore.Contains(assetAddress);
        }

        public GameObject GetGameObj(string assetAddress) {
            return _gameObjStore.Get(assetAddress);
        }

        public T GetObj<T>(string assetAddress) where T : ScriptableObject {
            var obj = _scriptableObjStore.Get(assetAddress);
            if (!obj is T) {
                Log.Error($"[ResourceStore] ScriptableObject cast error : <{assetAddress}>");
                return null;
            }

            return (T)obj;
        }

        public AudioClip GetAudio(string assetAddress) {
            return _audioStore.Get(assetAddress);
        }

        public async UniTask<AudioClip> GetAudioOndemand(string assetAddress) {
            if (!_audioStore.Contains(assetAddress)) {
                await RetainGlobalWithAutoLoad(assetAddress);
            }
            return _audioStore.Get(assetAddress);
        }

        public SpriteAtlas GetSpriteAtlas(string assetAddress) {
            return _spriteStore.Get(assetAddress);
        }

        public Sprite GetSprite(string spriteName) {
            return _spriteStore.GetSprite(spriteName);
        }

        public async UniTask Load() {
            if (_isLoading) {
                Log.Notice("[ResourceStore] Loading is already running.");
                return;
            }

            _isLoading = true;

            var addresses = _registry.GetAddressListToLoad();
            await LoadMulti(addresses);
            _isLoading = false;

            if (_registry.ShouldLoadAny()) {
                ++_loadRetryCount;
                if (_loadRetryCount > 99) {
                    return;
                }
                await Load();
                --_loadRetryCount;

            }
        }

        private async UniTask LoadMulti(IEnumerable<string> addresses) {
            var tasks = addresses.Select(address => LoadSingle(address));
            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadSingle(string assetAddress) {
            if (!_registry.IsReferenced(assetAddress)) {
                return;
            }

            var asyncOpHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetAddress);
            var resource = await asyncOpHandle.Task;

            if (asyncOpHandle.Status != AsyncOperationStatus.Succeeded) {
                Log.Error($"[ResourceStore] Load Error : <b>{assetAddress}</b>");
                return;
            }

            _registry.MarkLoaded(assetAddress, resource, asyncOpHandle);
            OnLoadResource(assetAddress, resource);
        }

        private void OnLoadResource(string assetAddress, UnityEngine.Object resource) {
            switch (resource) {
                case GameObject gameObj:
                    _gameObjStore.OnLoad(assetAddress, gameObj);
                    break;
                case ScriptableObject scriptableObj:
                    _scriptableObjStore.OnLoad(assetAddress, scriptableObj);
                    break;
                case AudioClip audioClip:
                    _audioStore.OnLoad(assetAddress, audioClip);
                    break;
                case SpriteAtlas spriteAtlas:
                    _spriteStore.OnLoad(assetAddress, spriteAtlas);
                    break;
                default:
                    Log.Warning($"[ResourceStore] Unsupported asset type : {assetAddress} - {resource.GetType()}");
                    break;
            }
        }

        public void Unload() {
            var addresses = _registry.GetAddressListToUnload();
            UnloadMulti(addresses);
        }

        private void UnloadMulti(IEnumerable<string> addresses) {
            foreach (var address in addresses) {
                UnloadSingle(address);
            }
        }

        private void UnloadSingle(string assetAddress) {
            if (_registry.IsReferenced(assetAddress)) {
                return;
            }

            var entry = _registry.GetEntry(assetAddress);
            Addressables.Release(entry.handle);
            OnUnloadResource(entry);
            _registry.UnmarkLoaded(assetAddress);
        }

        private void OnUnloadResource(ResourceEntry entry) {
            if (entry.Type == typeof(GameObject)) {
                _gameObjStore.OnUnload(entry.Address);
            } else if (entry.Type.IsSubclassOf(typeof(ScriptableObject))) {
                _scriptableObjStore.OnUnload(entry.Address);
            } else if (entry.Type == typeof(AudioClip)) {
                _audioStore.OnUnload(entry.Address);
            } else if (entry.Type == typeof(SpriteAtlas)) {
                _spriteStore.OnUnload(entry.Address);
            } else {
                Log.Warning($"[ResourceStore] Unsupported asset type : {entry.Address} - {entry.Type}");
            }
        }
    }
}
