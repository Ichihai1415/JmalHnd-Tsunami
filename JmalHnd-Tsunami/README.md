### 気象庁xml処理ソフト(津波情報)

- 一応仮扱いです。
- 気象庁防災情報XMLフォーマット形式電文-Atomフィード-高頻度フィードの地震火山から5分ごとに津波情報を検索し取得して描画します。
- 左側にマップと詳細情報、右側に地区ごとの情報を10件まで描画します。
- コンソールに進行情報を表示します。

- 現在設定はありません。
- 右クリックで長期フィードからの取得(発表から数日間で残っていれば描画可能)をするなどできます。
- `output`フォルダを作るとxmlファイル・画像ファイルが保存されます。`津波警報・注意報・予報a`のみです。
- 画像は自由にお使いいただけますが公開する場合「気象データ・地図データ:気象庁」の表示を消したり誤解を生むような表示をしたりしないようにしてください。

### サンプル画像
<!--GitHubで確認できます-->
<div display="flex">
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.0-1.png" width="32%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.0-2.png" width="32%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.0-3.png" width="32%" />
</div><div display="flex">
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.0-4.png" width="32%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.1-2.png" width="32%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/JmalHnd-Tsunami/master/image/v1.0.5-1.png" width="32%" />
</div>

## 履歴
### v1.0.6

- その他津波情報に含まれる予報情報を描画するように

### v1.0.5
2024/01/05

- 大津波警報の時色が表示されず表示がおかしくなる問題を修正

### v1.0.4
2023/12/03

- 震源情報が複数あるとき1つしか表示されない問題を修正
- 画像保存が同じidで1つの画像に上書きしていたので発表時刻表記を追加
- 無更新時前回の発表状況をコンソールに表示

### v1.0.3
2023/10/09

- 右クリックメニューに即取得を追加
- 津波注意報以上発表時取得間隔を1分に
- 文字がはみ出る問題を修正

### v1.0.2
2023/10/05

- 津波予報に引き下げられたとき?(要確認)、高さのデータがなくエラーとなる問題を修正

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
