# WxBeacon2-csharp-Extend

[WxBeacon2-csharp](https://github.com/weathernews/WxBeacon-csharp)の改良版です。

## 改良点

- 破棄の適切化により`Dispose()`を実行することで他のデバイスからWxBeacon2に接続できるように
- コードの簡潔化(少し)
- サンプルの微調整
- `WxBeacon2Data.ToString()`をJSON形式に

切断してもスマホアプリ等から切断できない場合、`GC.Collect();`を試してみてください。

## 更新履歴

### v1.0.2

2024/11/26
- 上手く接続解除できない時がある問題を仮対処
- `WxBeacon2Data.ToString()`をJSON形式に

### v1.0.1

2024/11/25
- 破棄の適切化によりDispose()を実行することで他のデバイスからWxBeacon2に接続できるように
- コードの簡潔化(少し)
- サンプルの微調整

---

以下原文

---

# WxBeacon2-csharp

WxBeacon2をWindowsから操作するための.NETライブラリです。

## 必須条件

* Windows 10 ビルド15063（Creators Update）以降
* Visual Studio 2017 (Universal Windows Appの開発機能が有効であること)
* Bluetooth LEに対応しているBluetooth 4.0 アダプタ（内蔵でもUSBでもよい）
* WxBeacon2

## 使用方法

- このリポジトリをフォークまたはダウンロード
- Visual Studioで```WxBeacon2.sln```を開く
- SampleAppプロジェクトを起動する
