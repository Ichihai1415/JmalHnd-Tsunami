### 気象庁xml処理ソフト(津波情報)

- 一応仮扱いです。
- 気象庁防災情報XMLフォーマット形式電文-Atomフィード-高頻度フィードの地震火山から5分ごとに津波情報を検索し取得して描画します。
- 左側にマップと詳細情報、右側に地区ごとの情報を10件まで描画します。
- コンソールに進行情報を表示します。

- 現在`津波警報・注意報・予報a`のみ対応しています。
- 現在設定はありません。
- 右クリックで長期フィードからの取得(発表から数日間で残っていれば描画可能)をするなどできます。
- `output`フォルダを作るとxmlファイル・画像ファイルが保存されます。
- 画像は自由にお使いいただけますが公開する場合「気象データ・地図データ:気象庁」の表示を消したり誤解を生むような表示をしたりしないようにしてください。

### サンプル画像
<!--GitHubで確認できます-->
<div display="flex">
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.0-1.png" width="24%" />
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.0-2.png" width="24%" />
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.0-3.png" width="24%" />
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.0-4.png" width="24%" />
</div>

## 履歴

### v1.0.1
2023/07/21

<div display="flex">
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.1-1.png" width="24%" />
  <img src="https://github.com/Ichihai1415/JmalHnd-Tsunami/blob/master/image/v1.0.1-2.png" width="24%" />
</div>

- 情報がない場合の描画を追加
- 情報未受信時の表示を追加
- 情報失効時の表示を追加
- 長期フィードからの取得(右クリック)を追加
- フォントを同梱
- 処理修正

### v1.0.0
2023/07/21

- とりあえず。
