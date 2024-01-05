using System;
using System.Diagnostics;
using Yukarinette.Distribution.Plugin.Core;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.Windows.Forms;

namespace Yukarinette.Distribution.Plugin
{
    /// <summary>
    /// A.I.VOICEプラグインやっつけ
    /// </summary>
    public class YukariManager : VoiceroidManager
    {
        protected Process AivoiceControlProcess;

        private UIA3Automation mAuto;
        private FlaUI.Core.Application mApp;
        private FlaUI.Core.AutomationElements.Window mWindow;

        AutomationElement mButton;
        AutomationElement mEdit;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public YukariManager(String voiceroidName)
            : base(voiceroidName)
        {
            YukarinetteLogger.Instance.Debug("start.");

            mAuto = new UIA3Automation();
            this.ClearMember();

            YukarinetteLogger.Instance.Debug("end.");
        }

        /// <summary>
        /// 破棄するときに呼ばれる
        /// </summary>
        public override void Dispose(Boolean autoexit)
        {
            if (autoexit)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1);
                });
            }

            base.Dispose(autoexit);

            YukarinetteLogger.Instance.Debug("start.");

            this.ClearMember();

            YukarinetteLogger.Instance.Debug("end.");
        }

        /// <summary>
        /// メンバ変数を初期化する。
        /// </summary>
        private void ClearMember()
        {
            YukarinetteLogger.Instance.Debug("start.");

            mButton = null;
            mEdit = null;

            YukarinetteLogger.Instance.Debug("end.");
        }

        /// <summary>
        /// 認識するための部品を指定する。
        /// </summary>
        /// <param name="form"></param>
        protected override void GetComponent(String voiceroidPath)
        {
            YukarinetteLogger.Instance.Debug("start.");
            try
            {
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("aivoice");
                if (ps.Length != 0)
                {
                    // 既に起動済みの場合
                    mApp = FlaUI.Core.Application.Attach(ps[0]);
                    mWindow = mApp.GetMainWindow(mAuto);
                }
                else
                {
                    // AI Voice2を起動する
                    mApp = FlaUI.Core.Application.Launch(voiceroidPath);
                    Thread.Sleep(100);
                    mWindow = mApp.GetMainWindow(mAuto);
                }
            }
            catch (Exception ex)
            {
                throw new YukarinetteException("AIVOICE2 の認識に失敗しました。" + Environment.NewLine + ex.Message);
            }

            YukarinetteLogger.Instance.Debug("end.");
        }

        /// <summary>
        /// 実際に喋らす際の制御を行う。
        /// </summary>
        /// <param name="text"></param>
        protected override void SpeechControl(String text)
        {
            YukarinetteLogger.Instance.Debug("start. text=" + text);

            // Busy中は最大10秒まで待つ
            int tickStart = System.Environment.TickCount & int.MaxValue;
            while (!IsCanSpeech())
            {
                int tickCount = System.Environment.TickCount & int.MaxValue;
                if (tickCount > (tickStart + 10000)) {
                    break;
                }
                Thread.Sleep(100);
            }

            try
            {
                // OBS用のテキスト出力先を通知する
                if (ObsOutTxt == true)
                {
                    ObsOutText(text);
                }

                //テキスト設定
                mEdit = mWindow.FindAllDescendants(factory => factory.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).ElementAt(1);
                //mEdit.AsTextBox().Enter("you context"); //Do not work!!
                mEdit.Focus();

                SendKeys.SendWait("^{HOME}");   // Move to start of control
                SendKeys.SendWait("^+{END}");   // Select everything
                SendKeys.SendWait("{DEL}");     // Delete selection
                SendKeys.SendWait(text);

                //スピーチ
                mButton = mWindow.FindAllDescendants(factory => factory.ByName("再生")).First();
                mButton.AsButton().Invoke(); // Buttonがグレーアウトしてるか判別する手段がないので時々機能しない

            }
            catch (Exception ex)
            {
                YukarinetteLogger.Instance.Error(ex);
                YukarinetteLogger.Instance.Info("voiceroid process is missing.");

                this.ClearMember();

                throw ex;
            }

            YukarinetteLogger.Instance.Debug("end.");
        }

        /// <summary>
        /// 読み上げ可能か否かを返す。
        /// </summary>
        /// <returns></returns>
        protected override bool IsCanSpeech()
        {
            YukarinetteLogger.Instance.Debug("start.");
            try
            {
                // 再生ボタンが見つかるかどうかで確認をする
                AutomationElement elem1 = mWindow.FindAllDescendants(factory => factory.ByName("再生")).First();
            }
            catch 
            {
                return false;
            }
            YukarinetteLogger.Instance.Debug("end. return=true");
            return true;
        }

        //OBSテキストへ出力する
        private void ObsOutText(string text)
        {
            try
            {
                if (ObsOutTextPath != "")
                {
                    File.WriteAllText(ObsOutTextPath, text, System.Text.Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
