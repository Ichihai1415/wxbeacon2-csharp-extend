# WxBeacon2-csharp-Extend

[WxBeacon2-csharp](https://github.com/weathernews/WxBeacon-csharp)の改良版です。

## 改良点

- 破棄の適切化によりDispose()を実行することで他のデバイスからWxBeacon2に接続できるように
- コードの簡潔化(少し)
- サンプルの微調整

## 更新履歴

### v1.0.1

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
