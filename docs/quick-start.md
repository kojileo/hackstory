# クイックスタートガイド

## 動作確認手順

### 1. バックエンドの起動

```bash
cd backend/HackStory.Api
dotnet run --urls "http://localhost:5000"
```

バックエンドは `http://localhost:5000` で起動します。

**注意**: データベース接続がない場合でも、InMemoryデータベースを使用して動作します。

### 2. フロントエンドの起動（別ターミナル）

```bash
cd frontend
npm run dev
```

フロントエンドは `http://localhost:3000` で起動します。

### 3. 動作確認

#### APIエンドポイント

1. **ストーリー一覧**: `http://localhost:5000/api/stories`
2. **ストーリー詳細**: `http://localhost:5000/api/stories/story-1`
3. **Swagger UI**: `http://localhost:5000/swagger`

#### フロントエンド

1. **ホームページ**: `http://localhost:3000`
2. **ストーリー一覧**: `http://localhost:3000/stories`
3. **ストーリービューアー**: `http://localhost:3000/stories/story-1`

## 現在の実装状況

### 完了している機能

- ✅ プロジェクト基盤構築
- ✅ バックエンドAPI（ストーリー取得、進捗保存）
- ✅ フロントエンドストーリービューアー
- ✅ コスト最適化（静的JSON配信、キャッシング、デバウンス）
- ✅ Docker設定

### 未実装の機能

- ⏳ ユーザー認証（現在は仮のユーザーIDを使用）
- ⏳ データベースマイグレーション（InMemoryデータベースで動作）
- ⏳ ハッキングチャレンジ機能
- ⏳ 進捗ダッシュボード

## トラブルシューティング

### バックエンドが起動しない

1. ポート5000が使用中でないか確認
2. ビルドエラーがないか確認: `dotnet build`

### フロントエンドが起動しない

1. ポート3000が使用中でないか確認
2. 依存関係がインストールされているか確認: `npm install`

### APIエラー

データベース接続エラーが発生する場合、`appsettings.Development.json`の`ConnectionStrings:DefaultConnection`を空文字列に設定すると、InMemoryデータベースが使用されます。

