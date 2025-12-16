# 動作確認ガイド

## 前提条件

- .NET 9 SDK がインストールされていること
- Node.js 18以上がインストールされていること
- Docker Desktopがインストールされていること（ローカル開発用）

## ローカル開発環境の起動

### 1. データベースの起動（Docker Compose）

```bash
cd docker
docker-compose up -d cockroachdb
```

### 2. バックエンドの起動

```bash
cd backend/HackStory.Api
dotnet run
```

バックエンドは `http://localhost:5000` で起動します。

### 3. フロントエンドの起動

別のターミナルで：

```bash
cd frontend
npm run dev
```

フロントエンドは `http://localhost:3000` で起動します。

## 動作確認項目

### APIエンドポイントの確認

#### 1. ストーリー一覧取得
```bash
curl http://localhost:5000/api/stories
```

#### 2. ストーリー詳細取得
```bash
curl http://localhost:5000/api/stories/story-1
```

#### 3. Swagger UI
ブラウザで `http://localhost:5000/swagger` にアクセス

### フロントエンドの確認

1. ホームページ: `http://localhost:3000`
2. ストーリー一覧: `http://localhost:3000/stories`
3. ストーリービューアー: `http://localhost:3000/stories/story-1`

## トラブルシューティング

### データベース接続エラー

データベースが起動していない場合、APIは起動しますが、データベース関連のエンドポイントはエラーになります。
この場合、進捗保存機能は動作しませんが、ストーリーの閲覧は可能です。

### CORSエラー

フロントエンドからAPIにアクセスできない場合、`appsettings.json`の`FrontendUrl`を確認してください。

