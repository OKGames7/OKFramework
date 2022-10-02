
namespace OKGamesLib {

    /// <summary>
    /// UnityのLayerに関する拡張クラス.
    /// </summary>
    public class LayerExtension {

        /// <summary>
        /// LayerNameからLayerMask値を取得する.
        /// </summary>
        /// <param name="name">レイヤー名.</param>
        public static LayerMasks.LayerMasksEnum GetLayerMask(LayerName.LayerNameEnum name) {
            LayerMasks.LayerMasksEnum mask;
            switch (name) {
                case LayerName.LayerNameEnum.Default:
                    mask = LayerMasks.LayerMasksEnum.Default;
                    break;
                case LayerName.LayerNameEnum.TransparentFX:
                    mask = LayerMasks.LayerMasksEnum.TransparentFX;
                    break;
                case LayerName.LayerNameEnum.Ignore_Raycast:
                    mask = LayerMasks.LayerMasksEnum.Ignore_Raycast;
                    break;
                case LayerName.LayerNameEnum.Water:
                    mask = LayerMasks.LayerMasksEnum.Water;
                    break;
                case LayerName.LayerNameEnum.UI:
                    mask = LayerMasks.LayerMasksEnum.UI;
                    break;
                case LayerName.LayerNameEnum.TransitionBoard:
                    mask = LayerMasks.LayerMasksEnum.TransitionBoard;
                    break;
                case LayerName.LayerNameEnum.NowLoading:
                    mask = LayerMasks.LayerMasksEnum.NowLoading;
                    break;
                case LayerName.LayerNameEnum.Character:
                    mask = LayerMasks.LayerMasksEnum.Character;
                    break;
                case LayerName.LayerNameEnum.TapEffect:
                    mask = LayerMasks.LayerMasksEnum.TapEffect;
                    break;
                case LayerName.LayerNameEnum.Debug:
                    mask = LayerMasks.LayerMasksEnum.Debug;
                    break;
                default:
                    Log.Error("対応していないレイヤーです.");
                    mask = LayerMasks.LayerMasksEnum.Default;
                    break;

            }
            return mask;
        }
    }
}
