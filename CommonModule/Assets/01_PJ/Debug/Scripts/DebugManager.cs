using OKGamesLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------------------------------------------
// ゲーム全体のデバッグ機能を制御するマネージャー.
// ---------------------------------------------------------
public class DebugManager : MonoBehaviour {
    [SerializeField] private CanvasSetter _canvasSetter = null;

    /// <summary>
    /// キャンバスで描画するためのカメラをセットする.
    /// </summary>
    public void SetCanvasCamera(Camera camera) {
        _canvasSetter.SetCamera(camera);
    }
}
