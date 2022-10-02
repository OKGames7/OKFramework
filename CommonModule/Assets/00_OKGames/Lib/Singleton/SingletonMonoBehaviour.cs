using System;
using UnityEngine;

namespace OKGamesLib {

    // ---------------------------------------------------------
    // MonoBehaviourを継承したシングルトンクラス.
    // SingletonにMonoBehaviourのクラスに継承して使用することでそのクラスをシングルトンにする.
    // ---------------------------------------------------------
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    Type t = typeof(T);

                    _instance = (T)FindObjectOfType(t);
                    if (_instance == null) {
                        Log.Error(t + " をアタッチしているGameObjectがHierachy上に存在しない.");
                    }
                }

                return _instance;
            }
        }

        virtual protected void Awake() {
            if (this != Instance) {
                Destroy(this);

                Log.Error(
                   typeof(T) +
                   " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                   " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
                return;
            }

            // シーンを跨いで存在させるために破棄しないオブジェクトとして登録する.
            DontDestroyOnLoad(this.gameObject);
        }


    }
}
