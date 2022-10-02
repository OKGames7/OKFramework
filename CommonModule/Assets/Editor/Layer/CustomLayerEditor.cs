using OKGamesLib;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ---------------------------------------------------------
// レイヤー設定のEditor拡張クラス.
// ---------------------------------------------------------
public class CustomLayerEditor : MonoBehaviour {

    // レイヤーの種類.
    private enum LayerType {
        CameraLayer,
        CanvasLayer,
    }

    /// <summary>
    /// LayerConstで定義しているレイヤー情報を全てタグマネージャーに書き込む.
    /// </summary>
    [MenuItem("o.k.games/01.Layer/SetLayers", false)]
    private static void SetLayers() {
        SetCameraLayers();
        SetCanvasLayer();
    }

    /// <summary>
    /// LayerConst.CanvasLayerで定義しているレイヤー情報をタグマネージャーに書き込む.
    /// </summary>
    [MenuItem("o.k.games/01.Layer/SetCameraLayer", false)]
    private static void SetCameraLayers() {
        // // Unity内でレイヤー管理時に使われるプロパティの取得.
        // var tagManager = GetTagManager();
        // var layersProperty = tagManager.FindProperty("layers");

        // // 上書き設定したいレイヤーの内容.
        // // Mask~のLayerは内部計算用でTag & Settings側へは登録しないので含めないする.
        // List<LayerMask.aaa> layerList = new List<LayerMask.aaa>(
        //     Enum.GetValues(typeof(LayerMask.aaa)).Cast<LayerMask.aaa>().ToList()
        // );

        // if (SetLayerPropertyToTagManager(LayerType.CameraLayer, layersProperty, layerList)) {
        //     // 設定の反映.
        //     tagManager.ApplyModifiedProperties();
        //     Log.Notice("設定成功");
        // } else {
        //     Log.Error("設定失敗");
        // }
    }

    /// <summary>
    /// LayerConst.CanvasLayerで定義しているレイヤー情報をタグマネージャーに書き込む.
    /// </summary>
    [MenuItem("o.k.games/01.Layer/SetCanvasLayer", false)]
    private static void SetCanvasLayer() {
        // // Unity内でレイヤー管理時に使われるプロパティの取得.
        // var tagManager = GetTagManager();
        // var layersProperty = tagManager.FindProperty("m_SortingLayers");

        // // 上書き設定したいレイヤーの内容.
        // List<LayerMask.CanvasLayer> layerList = new List<LayerMask.CanvasLayer>(
        //     Enum.GetValues(typeof(LayerMask.CanvasLayer)).Cast<LayerMask.CanvasLayer>().ToList()
        // );

        // if (SetLayerPropertyToTagManager(LayerType.CanvasLayer, layersProperty, layerList)) {
        //     // 設定の反映.
        //     tagManager.ApplyModifiedProperties();
        //     Log.Notice("設定成功");
        // } else {
        //     Log.Error("設定失敗");
        // }
    }

    /// <summary>
    /// 設定したいレイヤー情報をタグマネージャーに書き込む.
    /// </summary>
    private static bool SetLayerPropertyToTagManager<T>(LayerType layerType, SerializedProperty managerLayerProp, List<T> layerSetList) {
        // 開発確認用のログ.
        Log.Notice("Tags&LayersのLayeryへ設定する予定のレイヤー");

        if (layerSetList.Count() > managerLayerProp.arraySize) {
            // 設定しようとしているレイヤーの数がTags and Layers内のSortedLayer設定の数よりも多ければ設定できないのでエラーログを返す.
            Log.Error("Tags and Layers(TagManager.asset)内のLayer要素数をCustomLayer.cs内のLayer定義数に合わせてください");
            return false;
        }

        for (int i = 0; i < layerSetList.Count; ++i) {
            // enum定義されているレイヤー情報のindexはTag&Settings側のレイヤーの要素と一致させている.
            var mp = managerLayerProp.GetArrayElementAtIndex((int)(object)layerSetList[i]);

            // forのたびに回していてスマートじゃない書き方なので整理してもいいが、パッと思いつかず。
            // Editor拡張だし処理負荷の影響は大きくない部分なのでこのままでいく.
            switch (layerType) {
                case LayerType.CameraLayer:
                    break;
                case LayerType.CanvasLayer:
                    // SortingLayerだった場合は直にstringはなくて、namaの中の値を書き換える必要がある.
                    mp = mp.FindPropertyRelative("name");
                    break;
            }
            // レイヤー情報をTagManagerに書き込む.
            var setLayerName = layerSetList[i].ToString();
            if (mp != null && (mp.stringValue != setLayerName)) {
                mp.stringValue = setLayerName;
                Log.Notice("レイヤー追加: " + mp.stringValue);
            }
        }
        return true;
    }

    /// <summary>
    /// TagManagerの取得.
    /// </summary>
    private static SerializedObject GetTagManager() {
        return new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
    }
}
