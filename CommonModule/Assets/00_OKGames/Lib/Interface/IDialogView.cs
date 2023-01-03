namespace OKGamesLib {

    /// <summary>
    /// DialogViewのインターフェース.
    /// </summary>
    public interface IDialogView {

        string Header { get; }

        string Body { get; }

        DialogButtonType ButtonType { get; }


    }
}
