﻿using System.Diagnostics;
using YukkuriMovieMaker.Commons;

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
        static public DialogResult GetOKorCancel(string message)
        {
            return MessageBox.Show(
                        message,
                        PluginInfo.Title,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                        );
        }

        /// <summary>
        /// 質問をダイアログを出し、回答を返す(Yes or No)
        /// </summary>
        /// <param name="message">質問内容</param>
        /// <returns>Yes or No</returns>
        static public DialogResult GetYESorNO(string message)
        {
            return MessageBox.Show(
                        message,
                        PluginInfo.Title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                        );
        }

        /// <summary>
        /// 警告する
        /// </summary>
        /// <param name="message">警告内容</param>
        static public void ShowWarning(string message)
        {
            MessageBox.Show(
                            message,
                            PluginInfo.Title,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                            );
        }

        /// <summary>
        /// 情報表示する（フツーのダイアログ）
        /// </summary>
        /// <param name="message">警告内容</param>
        static public void ShowInformation(string message)
        {
            MessageBox.Show(
                            message,
                            PluginInfo.Title,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );
        }


        /// <summary>
        /// エラー表示する
        /// </summary>
        /// <param name="cont">内容</param>
        static public void ShowError(string cont , string? className, string? methodName)
        {
            var result = MessageBox.Show(
                            "エラーが発生しました。" +
                            $"\nスクリーンショットと共に開発者sinβ（{SinBetaKunX}）までご報告お願いします。" +
                            "\n（「はい(Y)」を押すとリンク先をブラウザで開きます。）" +
                            "\nエラー内容" +
                            "\n---" +
                            "\n" + cont +
                            "\n---" +
                            "\nエラー発生箇所" +
                            "\n\tクラス：" + (className ?? "(情報なし)") +
                            "\n\tメソッド：" + (methodName ?? "(情報なし)"),
                            PluginInfo.Title,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Error
                            );
            if ( result == DialogResult.Yes)
            {
                ProcessStartInfo pi = new ProcessStartInfo()
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
