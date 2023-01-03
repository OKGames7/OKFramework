using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

namespace OKGamesLib {

    /// <summary>
    /// 前に戻る機能を管理するクラスのインターフェース.
    /// </summary>
    public interface IPrev {

        /// <summary>
        /// 購読処理.
        /// </summary>
        /// <param name="popNotify">ポップ通知管理クラス.</param>
        /// <param name="inputBlocker">ユーザーインプットのブロック状態を管理するクラス.</param>
        /// <param name="textMaster">テキストマスター.</param>
        void Inject(IPopNotify popNotify, IInputBlocker inputBlocker, Entity_text textMaster);

        /// <summary>
        /// 購読処理.
        /// </summary>
        /// <param name="parent">AddToに紐付けするオブジェクト(DontDestory扱いなので削除される想定はないが).</param>
        void Bind(GameObject parent);


        /// <summary>
        /// 戻るアクションをPush(設定)する.
        /// </summary>
        /// <param name="action">戻るアクション.</param>
        void Push(Action action);

        /// <summary>
        /// 戻るアクションをPop(取得)する
        /// PopするとStackからそのアクションは消失する.
        /// </summary>
        /// <returns></returns>
        Action Pop();

        /// <summary>
        /// 戻るアクションのスタックをクリア(リセット)する
        /// </summary>
        void Clear();

        /// <summary>
        /// アプリケーション側で別途バリデーションを挟みたい場合はこの関数からセットすれば良い.
        /// </summary>
        /// <param name="validate"></param>
        void SetApplicationValidate(Func<bool> validate);
    }
}
