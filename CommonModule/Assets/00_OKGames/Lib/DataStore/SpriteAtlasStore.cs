using OKGamesLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace OKGamesLib {

    /// <summary>
    /// ロードしたAddressables AssetのSprite Atlasを保持する.
    /// 各SpriteはGetSpriteで取得する.
    /// </summary>
    public class SpriteAtlasStore : ResourceSubStore<SpriteAtlas> {
        /// <summary>
        /// Atlas内にあるSpriteを個別管理するための辞書.
        /// </summary>
        Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();

        /// <summary>
        /// Spriteを取得する.
        /// </summary>
        /// <param name="spriteName">取得するスプライト名.</param>
        /// <returns>キャッシュのスプライト.</returns>
        public Sprite GetSprite(string spriteName) {
            Sprite sprite;
            if (!_spriteDict.TryGetValue(spriteName, out sprite)) {
                Log.Error($"[SpriteAtlasStore] Sprite <b>{spriteName}</b> not found.");
                return null;
            }
            return sprite;
        }

        /// <summary>
        /// Atlas内のSpriteを分解して、Sprite用のキャッシュへ追加する.
        /// </summary>
        /// <inheritdoc/>
        public override void OnLoad(string assetAddress, SpriteAtlas resource) {
            base.OnLoad(assetAddress, resource);
            RegisterSprite(resource);
        }

        /// <summary>
        /// Atlas内のSpriteを分解して、Sprite用のキャッシュに対しても削除する.
        /// </summary>
        /// <inheritdoc/>
        public override void OnUnload(string assetAddress) {
            UnregisterSprites(Get(assetAddress));
            base.OnUnload(assetAddress);
        }

        /// <summary>
        /// Atlas内のSpriteをキャッシュ用辞書へ登録する.
        /// </summary>
        /// <param name="atlas">登録元となるアトラス.</param>
        private void RegisterSprite(SpriteAtlas atlas) {
            ForEachSprite(atlas, (spriteName, sprite) => {
                _spriteDict.Add(spriteName, sprite);
            });
        }

        /// <summary>
        /// キャッシュ用辞書からAtlas内のSpriteと同じ要素を削除する.
        /// </summary>
        /// <param name="atlas"></param>
        private void UnregisterSprites(SpriteAtlas atlas) {
            ForEachSprite(atlas, (spriteName, sprite) => {
                _spriteDict.Remove(spriteName);
            });
        }

        /// <summary>
        /// Atlas内の各Spriteに対して/用いて何か処理させたい場合に使用する関数.
        /// </summary>
        /// <param name="atlas">元となるアトラス.</param>
        /// <param name="action">各スプライトに対して/用いて行う処理.</param>
        private void ForEachSprite(SpriteAtlas atlas, Action<string, Sprite> action) {
            Sprite[] spriteInAtlas = new Sprite[atlas.spriteCount];

            atlas.GetSprites(spriteInAtlas);

            foreach (var sprite in spriteInAtlas) {
                string spriteName = sprite.name.Substring(0, sprite.name.Length - "(Clone)".Length);
                action(spriteName, sprite);
            }
        }
    }
}
