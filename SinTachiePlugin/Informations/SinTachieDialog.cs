using System.Diagnostics;
using System.Windows;
using YukkuriMovieMaker.Commons;
using MessageBox = System.Windows.MessageBox;

namespace SinTachiePlugin.Informations
{
    /// <summary>
    /// このプラグインによって表示されるダイアログ
    /// </summary>
    public class SinTachieDialog : Animatable
    {
        /// <summary>
        /// 質問をダイアログを出し、回答を返す(OK or Cancel)
        /// </summary>
        /// <param name="message">質問内容</param>
        /// <returns>OK or Cancel</returns>
        static public MessageBoxResult GetOKorCancel(string message)
        {
            return MessageBox.Show(
                message,
                PluginInfo.Title,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question
                );
        }

        /// <summary>
        /// 質問をダイアログを出し、回答を返す(Yes or No)
        /// </summary>
        /// <param name="message">質問内容</param>
        /// <returns>Yes or No</returns>
        static public MessageBoxResult GetYESorNO(string message)
        {
            return MessageBox.Show(
                message,
                PluginInfo.Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );
        }

        /// <summary>
        /// 警告する
        /// </summary>
        /// <param name="message">警告内容</param>
        static public void ShowWarning(string message)
        {
            Task.Run(() =>
            {
                MessageBox.Show(
                    message,
                    PluginInfo.Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            });
        }

        /// <summary>
        /// 情報表示する（フツーのダイアログ）
        /// </summary>
        /// <param name="message">警告内容</param>
        static public void ShowInformation(string message)
        {
            Task.Run(() =>
            {
                MessageBox.Show(
                    message,
                    PluginInfo.Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            });
        }


        /// <summary>
        /// エラー表示する
        /// </summary>
        /// <param name="cont">内容</param>
        static public void ShowError(Exception ex)
        {
            MessageBoxResult result = MessageBox.Show(
                "エラーが発生しました。" +
                $"\nスクリーンショットと共に開発者sinβ（{SinBetaKunX}）までご報告お願いします。" +
                "\n（「はい(Y)」を押すとリンク先をブラウザで開きます。）" +
                "\nエラー内容" +
                "\n---" +
                "\n" + ex.Message +
                "\n---" +
                "\nエラー発生箇所:\n" +
                ex.StackTrace,
                PluginInfo.Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Error
                );

            if (result == MessageBoxResult.Yes)
            {
                ProcessStartInfo pi = new()
                {
                    FileName = SinBetaKunX,
                    UseShellExecute = true,
                };
                Process.Start(pi);
            }
        }

        static private string SinBetaKunX => "https://x.com/sinBetaKun";

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
