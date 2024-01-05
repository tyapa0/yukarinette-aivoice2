# ゆかりねっと A.I.VOICE2 連携プラグイン
A.I.VOICE2 を ゆかりねっと で制御するためのプラグインです。

# Features
* テストリリースです。APIが公開されるまでの暫定リリースとなります。

# Installation
1. [Releaseページ](https://github.com/tyapa0/yukarinette-aivoice/releases/) から最新バージョンの `Aivoice2Control_vX.X.zip` をダウンロードします
2. ダウンロードした zip ファイルのプロパティを開いて、ブロックされていたら解除します。
3. ダウンロードした zip ファイルを展開します。
4. 以下のファイルを ゆかりねっと の`Plugins`フォルダーにコピーします。
   - `Aivoice2ControlPlugin.dll`  
   ※解凍ツールによってはセキュリティ許可がされていない場合があります。  
   ファイルを右クリック→プロパティで表示し、セキュリティを許可してください。  
   ![kyoka.png.](/image/kyoka.png "kyoka")

5. ゆかりねっと を起動したら 音声認識 の欄に「A.I.VOICE」が追加されているのでチェックを入れます。

以上

# Problem
* 発話一発目に再生ボタンが押せない  (ボタンのグレーアウト状態が取れない)
* 再生のたびにWindowフォーカスが持って行かれてしまう。(フォーカスが移動して困るゲームと同時使用できない)

# Knowledge
* FriendlyではVisualTreeが取得できない(おそらくUWPなためFriendlyの実装がない)
* コントロールはUIAutomationでほぼ取得できる
* UIのフォーカスは.core側のUIAutomatでのみ設定できる(そのためFlaUI.UIA3を使用)
* Editのtextは取得できるが、書き換えができない(RangeValueのSetValueが機能しない)

# Author
* [おかゆう](http://www.okayulu.moe/)氏作のVOICEROID EX/EX+制御プラグインを改造しています。
* NuGetパッケージの Interop.UIAutomationClient.10.19041.0を使用しています。
* NuGetパッケージの FlaUI.UIA3.4.0.0を使用しています。
* NuGetパッケージの Costura.Fody.5.7.0を使用しています。

# License
"yukarinette-aivoice" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
