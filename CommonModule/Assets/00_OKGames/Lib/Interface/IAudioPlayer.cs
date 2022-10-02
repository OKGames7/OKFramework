using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OKGamesLib {

    /// <summary>
    /// bgmやseの再生のクラスのインターフェース.
    /// </summary>
    public interface IAudioPlayer {

        /// <summary>
        /// システムの音量.
        /// </summary>
        float SystemMasterVolume { get; set; }

        /// <summary>
        /// ユーザーが調整できる分の音量.
        /// </summary>
        float UserMasterVolume { get; set; }

        /// <summary>
        /// 外部からの初期化.
        /// </summary>
        /// <param name="gameObject">AudioSourceのPoolで生成するオブジェクトの親とするGameObject.</param>
        /// <param name="numSourcePool">AudioSourceのPool数.</param>
        /// <param name="sceneDirector">ゲームで唯一の<see cref="SceneDirector"/></param>
        /// <param name="resourceStore">AudioClipのアセットのStore.</param>
        void Init(
            GameObject gameObject, int numSourcePool,
            OKGamesFramework.ISceneDirector sceneDirector, IResourceStore resourceStore
        );

        /// <summary>
        /// サウンドの再生.
        /// </summary>
        /// <param name="audioClip">再生するサウンド.</param>
        /// <param name="loop">ループ再生するか.</param>
        /// <param name="mix">他のサウンドと同時再生可能にするか.</param>
        /// <param name="replay">既に同じサウンドがなっている場合、最初から再生するか.</param>
        /// <param name="autoVolume">同じ音を同時再生したときのボリュームの高まりを抑えるか.</param>
        /// <param name="volume">音量.</param>
        /// <param name="pitch">ピッチ数.</param>
        /// <param name="pan">音の左右の定位(0は左右バランス良く.-1が左だけ, 1が右だけ.</param>
        /// <param name="sptatial">3Dサウンドの計算にどの程度影響を及ぼすか(0=2D, 1.0=完全に3D)</param>
        /// <param name="position">3Dサウンドの発生位置.</param>
        void Play(
            AudioClip audioClip, bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
            float sptatial = 0f, Vector3? position = null)
        ;

        /// <summary>
        /// サウンドの再生.
        /// </summary>
        /// <param name="audioPath">再生するサウンドアセットのパス.</param>
        /// <param name="loop">ループ再生するか.</param>
        /// <param name="mix">他のサウンドと同時再生可能にするか.</param>
        /// <param name="replay">既に同じサウンドがなっている場合、最初から再生するか.</param>
        /// <param name="autoVolume">同じ音を同時再生したときのボリュームの高まりを抑えるか.</param>
        /// <param name="volume">音量.</param>
        /// <param name="pitch">ピッチ数.</param>
        /// <param name="pan">音の左右の定位(0は左右バランス良く.-1が左だけ, 1が右だけ.</param>
        /// <param name="sptatial">3Dサウンドの計算にどの程度影響を及ぼすか(0=2D, 1.0=完全に3D)</param>
        /// <param name="position">3Dサウンドの発生位置.</param>
        void Play(
            string audioPath, bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
            float sptatial = 0f, Vector3? position = null
        );

        /// <summary>
        /// サウンドリソースの自動ロード、再生、アンロードまで一括で行う再生処理.
        /// 再生時はリソースのグローバル参照カウントが +1 され、未ロードならロードしてから再生する。
        /// 再生終了時には参照カウントを -1 し、その時点でカウントが 0 ならアンロードを行う。
        ///
        /// シーンを跨いでBGMを再生する場合や、ジュークボックスなどで都度必要なリソースをロードして鳴らすような場合に利用できる.
        /// </summary>
        /// <param name="audioPath">再生するサウンドアセットのパス.</param>
        /// <param name="loop">ループ再生するか.</param>
        /// <param name="mix">他のサウンドと同時再生可能にするか.</param>
        /// <param name="replay">既に同じサウンドがなっている場合、最初から再生するか.</param>
        /// <param name="autoVolume">同じ音を同時再生したときのボリュームの高まりを抑えるか.</param>
        /// <param name="volume">音量.</param>
        /// <param name="pitch">ピッチ数.</param>
        /// <param name="pan">音の左右の定位(0は左右バランス良く.-1が左だけ, 1が右だけ.</param>
        /// <param name="sptatial">3Dサウンドの計算にどの程度影響を及ぼすか(0=2D, 1.0=完全に3D)</param>
        /// <param name="position">3Dサウンドの発生位置.</param>
        UniTask PlayOndemand
        (string audioPath, bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float volume = 1.0f, float pitch = 1.0f, float pan = 0f,
            float sptatial = 0f, Vector3? position = null
        );

        /// <summary>
        /// 指定した<see cref="AudioClip"/>の再生を止める.
        /// 引数にnullを指定すると全サウンドを止める.
        /// </summary>
        /// <param name="audioClip">再生を止めたい<see cref="AudioClip"/>.</param>
        void Stop(AudioClip audioClip = null);

        /// <summary>
        /// 指定したパスの<see cref="AudioClip"/>の再生を止める.
        /// </summary>
        /// <param name="audioPath">再生を止めたい<see cref="AudioClip"/>のアセットパス.</param>
        void Stop(string audioPath);

        /// <summary>
        /// サウンドのフェードイン再生.
        /// </summary>
        /// <param name="audioClip">再生するサウンド.</param>
        /// <param name="fadeTime">フェードインにかける時間.</param>
        /// <param name="volumeFrom">フェード開始時の音量.</param>
        /// <param name="volumeTo">フェード終了時の音量.</param>
        /// <param name="loop">ループ再生するか.</param>
        /// <param name="mix">他のサウンドと同時再生可能にするか.</param>
        /// <param name="replay">既に同じサウンドがなっている場合、最初から再生するか.</param>
        /// <param name="autoVolume">同じ音を同時再生したときのボリュームの高まりを抑えるか.</param>
        /// <param name="pitch">ピッチ数.</param>
        /// <param name="pan">音の左右の定位(0は左右バランス良く.-1が左だけ, 1が右だけ.</param>
        /// <param name="sptatial">3Dサウンドの計算にどの程度影響を及ぼすか(0=2D, 1.0=完全に3D)</param>
        /// <param name="position">3Dサウンドの発生位置.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeIn(
            AudioClip audioClip, float fadeTime, float volumeFrom = 0f, float volumeTo = 1.0f,
            bool? loop = null, bool? mix = null, bool? replay = null,
            bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        );

        /// <summary>
        /// サウンドのフェードアウト停止.
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかける時間.</param>
        /// <param name="audioClip">停止するサウンド.</param>
        /// <param name="volumeFrom">フェード開始時の音量.</param>
        /// <param name="volumeTo">フェード終了時の音量.</param>
        /// <returns>UniTask.</returns>
        UniTask FadeOut(
            float fadeTime, AudioClip audioClip = null, float volumeFrom = 1.0f, float volumeTo = 0f
        );

        /// <summary>
        /// 現在再生しているサウンドをフェードアウトさせて、指定したサウンドをフェードインさせる(サウンドのクロスフェード再生する)
        /// </summary>
        /// <param name="audioPath">フェードインさせるサウンドアセットのパス.</param>
        /// <param name="fadeOutTime">フェードアウトにかける時間.</param>
        /// <param name="fadeInDelay">フェードインの開始を何秒後から開始するか.</param>
        /// <param name="fadeInTime">フェードインし終わるまでの時間</param>
        /// <param name="volumeFrom">フェードイン再生開始時の音量.</param>
        /// <param name="volumeTo">フェードイン再生狩猟時の音量.</param>
        /// <param name="loop">ループ再生するか.</param>
        /// <param name="autoVolume">同じ音を同時再生したときのボリュームの高まりを抑えるか.</param>
        /// <param name="pitch">ピッチ数.</param>
        /// <param name="pan">音の左右の定位(0は左右バランス良く.-1が左だけ, 1が右だけ.</param>
        /// <param name="sptatial">3Dサウンドの計算にどの程度影響を及ぼすか(0=2D, 1.0=完全に3D)</param>
        /// <param name="position">3Dサウンドの発生位置.</param>
        /// <returns></returns>
        UniTask CrossFade(
            string audioPath, float fadeOutTime, float fadeInDelay, float fadeInTime,
            float volumeFrom = 0f, float volumeTo = 1.0f, bool? loop = null,
            bool autoVolume = true, float pitch = 1.0f, float pan = 0f,
            float spatial = 0f, Vector3? position = null
        );

        /// <summary>
        /// マスターボリュームを一時的に絞る.
        /// </summary>
        /// <param name="duckTime">ボリュームを絞る時間.</param>
        /// <param name="volumeScale">ダッキング時の音量倍率.</param>
        /// <param name="fadeOutTime"><See cref="volumeScale"/>へ音を徐々に絞りきるのにかける時間.</param>
        /// <param name="fadeInTime">等倍の音量に徐々に音を戻すのにかける時間.</param>
        /// <returns>UniTask.</returns>
        UniTask Ducking(
            float duckTime, float volumeScale = 0.25f,
            float fadeOutTime = 0.2f, float fadeInTime = 0.4f
        );

        /// <summary>
        /// マスターボリュームをフェードで変更する。
        /// 現状リセット機能は無いので元に戻す際は手動で元の音量に FadeVolume() すること
        /// </summary>
        /// <param name="duration">何秒かけてボリュームを変更するか.</param>
        /// <param name="volumeFrom">開始値.</param>
        /// <param name="volumeTo">終了値.</param>
        /// <param name="ease">変化曲線</param>
        /// <returns>UniTask.</returns>
        UniTask FadeVolume(
            float duration, float volumeFrom, float volumeTo, EasingFunc ease = null
        );

        /// <summary>
        /// サウンドの一時停止.
        /// 引数にnullを送った場合は全てのサウンドの一時停止する.
        /// </summary>
        /// <param name="audioClip">一時停止させる<see cref="AudioClip"/></param>
        void Pause(AudioClip audioClip = null);

        /// <summary>
        /// サウンドの一時停止.
        /// </summary>
        /// <param name="audioPath">一時停止させる<see cref="AudioClip"/>のアセットパス.</param>
        void Pause(string audioPath);

        /// <summary>
        /// 一時停止しているサウンドの再開.
        /// 引数にnullを送った場合は全ての一時停止しているサウンドが再開する.
        /// </summary>
        /// <param name="audioClip">再開させる<see cref="AudioClip"/></param>
        void Resume(AudioClip audioClip = null);

        /// <summary>
        /// 一時停止しているサウンドの再開.
        /// </summary>
        /// <param name="audioPath">再開させる<see cref="AudioClip"/>のアセットパス.</param>
        void Resume(string audioPath);

    }
}
