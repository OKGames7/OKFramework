using OKGamesLib;
using UnityEngine;
using TMPro;

// ---------------------------------------------------------
// セーブデータの挙動をチェックするテストシーン用のシーンクラス.
// ---------------------------------------------------------
public class TestStorageScene : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text = null;

    // セーブデータがある場合はその値を、無い場合はその旨をテキストへ表示する.
    public void SetText() {
        string key = "test2";
        var o = StorageUtility.Load(key);

        string v = (o != null) ? o.ToString() : "Not Exsist";
        text.text = v;
    }
}
