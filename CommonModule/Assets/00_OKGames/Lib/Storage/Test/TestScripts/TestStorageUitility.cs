using UnityEngine;
using OKGamesLib;

// ---------------------------------------------------------
// StorageUtilityクラスの機能テスト用.
// ---------------------------------------------------------
public class TestStorageUitility : MonoBehaviour {

    private void TestSave(string s, int i) {
        StorageUtility.Save(s, i);
    }

    // セーブデータが無いものをkey指定してロードしようとした場合
    public void TestLoadNotExist() {
        var a = StorageUtility.Load("test1");

        Debug.Assert(a == null);
    }

    // 初めてセーブデータをセーブした場合
    public void TestSaveFirstTime() {
        string key = "test2";
        int value = 1;

        TestSave(key, value);

        int i = (int)StorageUtility.Load(key);
        Debug.Assert(i == value);
    }

    // 既存セーブデータを上書き更新した場合
    public void TestSaveOverwrite() {
        TestSaveFirstTime();

        string key = "test2";
        int value = 2;
        TestSave(key, value);
        int i = (int)StorageUtility.Load(key);
        Debug.Assert(i == value);
    }

    // セーブデータを削除する場合(all delete)
    public void TestAllDelete() {
        TestSaveFirstTime();
        StorageUtility.DeleteAll();

        string key = "test2";
        var o = StorageUtility.Load(key);
        Debug.Assert(o == null);
    }

    // セーブデータを削除する場合(all deleteで除外を入れた場合の挙動)
    public void TestAllDeleteExclude() {
        string key = "test3";
        int value = 3;
        TestSave(key, value);

        string[] keys = new string[1] { "test2" };
        TestSaveFirstTime();
        StorageUtility.DeleteAll(keys);

        var o = StorageUtility.Load(key);
        Debug.Assert(o == null);

        int i = (int)StorageUtility.Load("test2");
        Debug.Assert(i == 1);
    }

    // 指定したセーブデータを削除する場合
    // セーブデータを削除する場合(all deleteで除外を入れた場合の挙動)
    public void TestDeleteDirection() {
        string key = "test2";
        TestSaveFirstTime();
        StorageUtility.Delete(key);

        var o = StorageUtility.Load(key);
        Debug.Assert(o == null);

    }
}
