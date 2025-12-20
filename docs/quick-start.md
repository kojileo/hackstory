# クイックスタートガイド

## セットアップ

### Firebase設定

Firebase Authenticationを使用するために、以下の手順でFirebaseプロジェクトを設定します。

#### 1. Firebaseプロジェクトの作成

1. [Firebase Console](https://console.firebase.google.com/)にアクセス
2. 「プロジェクトを追加」をクリック
3. プロジェクト名を入力（例: `hackstory`）
4. Google Analyticsの設定（任意）を選択
5. 「プロジェクトを作成」をクリック
6. プロジェクトの作成が完了するまで待機（数分かかる場合があります）

#### 2. Authenticationの有効化

1. Firebase Consoleで作成したプロジェクトを選択
2. 左メニューから「Authentication」をクリック
3. 「始める」ボタンをクリック
4. 「Sign-in method」タブを選択
5. 「メール/パスワード」をクリック
6. 「有効にする」をクリック
7. 「保存」をクリック

これで、メールアドレスとパスワードによる認証が有効になります。

#### 3. Webアプリの登録

1. Firebase Consoleのプロジェクト設定（⚙️アイコン）をクリック
2. 「マイアプリ」セクションで「</>」アイコン（Webアプリを追加）をクリック
3. アプリのニックネームを入力（例: `HackStory Dev`）
4. 「このアプリのFirebase Hostingも設定します」はチェックを外してOK
5. 「アプリを登録」をクリック
6. 表示された設定情報をコピー（後で使用します）

表示される設定情報の例：
```javascript
const firebaseConfig = {
  apiKey: "AIza...",
  authDomain: "your-project.firebaseapp.com",
  projectId: "your-project-id",
  storageBucket: "your-project.appspot.com",
  messagingSenderId: "123456789",
  appId: "1:123456789:web:abc123"
};
```

#### 4. サービスアカウントキーの取得（バックエンド用）

1. Firebase Consoleのプロジェクト設定（⚙️アイコン）をクリック
2. 「サービスアカウント」タブを選択
3. 「新しい秘密鍵の生成」をクリック
4. 確認ダイアログで「キーを生成」をクリック
5. JSONファイルがダウンロードされます（例: `your-project-firebase-adminsdk-xxxxx.json`）
6. このファイルを安全な場所に保存（**重要**: このファイルは機密情報です。Gitにコミットしないでください）

#### 5. フロントエンド設定

1. `frontend`ディレクトリに`.env.local`ファイルを作成（存在しない場合）

2. 以下の内容を記述し、先ほどコピーしたFirebase設定情報を入力：

```env
# Firebase Configuration
NEXT_PUBLIC_FIREBASE_API_KEY=AIza...（apiKeyの値）
NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN=your-project.firebaseapp.com
NEXT_PUBLIC_FIREBASE_PROJECT_ID=your-project-id
NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET=your-project.appspot.com
NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID=123456789
NEXT_PUBLIC_FIREBASE_APP_ID=1:123456789:web:abc123

# API URL
NEXT_PUBLIC_API_URL=http://localhost:5000
```

3. ファイルを保存

**注意**: 
- `.env.local`ファイルは`.gitignore`に含まれているため、Gitにコミットされません
- 本番環境では、環境変数を適切に設定してください

#### 6. バックエンド設定

##### 開発環境（ローカル）

1. ダウンロードしたサービスアカウントキーJSONファイルを`backend/HackStory.Api`ディレクトリに配置
   - 例: `backend/HackStory.Api/firebase-service-account.json`

2. `backend/HackStory.Api/appsettings.Development.json`を作成（存在しない場合）:
   - `appsettings.Development.json.example`をコピーして`appsettings.Development.json`を作成
   - または、以下の内容で新規作成：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=26257;Database=hackstory_dev;Username=root;Password=;SSL Mode=Disable"
  },
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "CredentialsPath": "firebase-service-account.json"
  }
}
```

**重要**: 
- `appsettings.Development.json`は`.gitignore`に含まれているため、Gitにはコミットされません
- 個人の開発環境に合わせて設定を変更してください

**重要**: 
- `CredentialsPath`は、`appsettings.Development.json`からの相対パスを指定します
- サービスアカウントキーファイルは`.gitignore`に追加してください

##### 本番環境（Google Cloud Run）

Google Cloud Run環境では、以下のいずれかの方法で設定できます：

**方法1: 環境変数を使用**
- Cloud Runの環境変数に`GOOGLE_APPLICATION_CREDENTIALS`を設定
- サービスアカウントキーをSecret Managerに保存

**方法2: デフォルト認証情報を使用**
- Cloud RunのサービスアカウントにFirebase Admin SDKの権限を付与
- `ProjectId`のみを設定すれば、自動的に認証情報が検出されます

`appsettings.json`の例：
```json
{
  "Firebase": {
    "ProjectId": "your-project-id"
  }
}
```

#### 7. .gitignoreの確認

以下のファイルが`.gitignore`に含まれていることを確認してください：

```
# Firebase
frontend/.env.local
backend/HackStory.Api/firebase-service-account.json
backend/HackStory.Api/*-firebase-adminsdk-*.json
```

#### 8. 動作確認

設定が完了したら、以下のコマンドで動作確認を行います：

```bash
# バックエンドの起動
cd backend/HackStory.Api
dotnet run --urls "http://localhost:5000"

# 別ターミナルでフロントエンドの起動
cd frontend
npm run dev
```

ブラウザで`http://localhost:3000/auth/register`にアクセスし、ユーザー登録ができることを確認してください。

## CockroachDBのセットアップ（オプション）

ローカルでCockroachDBを使用する場合は、[CockroachDBセットアップガイド](./cockroachdb-setup.md)を参照してください。

**注意**: CockroachDB管理UI（http://localhost:8080）では、テーブルデータを直接閲覧する機能は提供されていません。データを確認するには、コマンドラインからSQLを実行する必要があります。詳細は [CockroachDBセットアップガイド](./cockroachdb-setup.md) の「データベースの確認」セクションを参照してください。

### クイックセットアップ

1. **CockroachDBの起動**:
   ```bash
   cd docker
   docker-compose up -d cockroachdb
   ```

2. **データベースの作成**:
   ```bash
   docker exec hackstory-cockroachdb ./cockroach sql --insecure -e "CREATE DATABASE IF NOT EXISTS hackstory_dev;"
   ```

3. **接続文字列の確認**:
   `backend/HackStory.Api/appsettings.Development.json`に接続文字列が設定されていることを確認してください（既に設定済みです）。

4. **マイグレーションの作成**:
   ```bash
   dotnet ef migrations add InitialCreate --project backend/HackStory.Infrastructure/HackStory.Infrastructure.csproj --startup-project backend/HackStory.Api/HackStory.Api.csproj
   ```

5. **マイグレーションの適用**:
   **重要**: CockroachDBは`LOCK TABLE`構文をサポートしていないため、`dotnet ef database update`は使用できません。代わりに、SQLスクリプトを手動で適用します：
   
   ```bash
   # SQLスクリプトの生成
   dotnet ef migrations script --project backend/HackStory.Infrastructure/HackStory.Infrastructure.csproj --startup-project backend/HackStory.Api/HackStory.Api.csproj --output migration.sql --idempotent
   
   # CockroachDB互換スクリプトの適用
   Get-Content scripts/migration-cockroachdb.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
   ```

詳細は [CockroachDBセットアップガイド](./cockroachdb-setup.md) を参照してください。

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
3. **Firebase IDトークン検証**: `POST http://localhost:5000/api/auth/verify` (Firebase IDトークン必要)
4. **現在のユーザー情報**: `GET http://localhost:5000/api/auth/me` (認証必要)
5. **Swagger UI**: `http://localhost:5000/swagger`

#### フロントエンド

1. **ホームページ**: `http://localhost:3000`
2. **ログインページ**: `http://localhost:3000/auth/login`
3. **登録ページ**: `http://localhost:3000/auth/register`
4. **ストーリー一覧**: `http://localhost:3000/stories` (認証必要)
5. **ストーリービューアー**: `http://localhost:3000/stories/story-1` (認証必要)

## 現在の実装状況

### 完了している機能

- ✅ プロジェクト基盤構築
- ✅ バックエンドAPI（ストーリー取得、進捗保存）
- ✅ フロントエンドストーリービューアー
- ✅ コスト最適化（静的JSON配信、キャッシング、デバウンス）
- ✅ Docker設定
- ✅ ユーザー認証（Firebase Authentication、ログイン/登録機能）

### 未実装の機能

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

### Firebase認証エラー

#### 「Firebase ID token verification failed」エラー

- Firebase設定が正しく行われているか確認
- `appsettings.Development.json`の`Firebase:ProjectId`が正しいか確認
- サービスアカウントキーファイルのパスが正しいか確認
- サービスアカウントキーファイルが存在し、読み取り可能か確認

#### 「Invalid Firebase ID token」エラー

- フロントエンドの`.env.local`ファイルの設定が正しいか確認
- Firebase ConsoleでAuthenticationが有効になっているか確認
- ブラウザのコンソールでFirebase初期化エラーがないか確認
- `NEXT_PUBLIC_FIREBASE_PROJECT_ID`が正しく設定されているか確認

#### 認証状態が保持されない

- ブラウザのCookie設定を確認
- `NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN`が正しく設定されているか確認
- ブラウザの開発者ツールでFirebase認証のエラーがないか確認

#### 「Firebase: Error (auth/configuration-not-found)」エラー

- `.env.local`ファイルが正しい場所（`frontend`ディレクトリ直下）にあるか確認
- 環境変数の値が正しく設定されているか確認
- フロントエンドを再起動（`.env.local`の変更は再起動が必要）

#### サービスアカウントキーのエラー

- サービスアカウントキーファイルのパスが正しいか確認（相対パスは`appsettings.Development.json`からの相対パス）
- JSONファイルの形式が正しいか確認
- ファイルの読み取り権限があるか確認

#### その他のトラブルシューティング

- ブラウザの開発者ツール（F12）のコンソールタブでエラーメッセージを確認
- ネットワークタブでAPIリクエストが正しく送信されているか確認
- Firebase ConsoleのAuthenticationタブでユーザーが作成されているか確認

