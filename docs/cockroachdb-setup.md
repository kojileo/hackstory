# CockroachDB ローカルセットアップガイド

## 概要

このガイドでは、ローカル環境でCockroachDBを起動し、HackStoryアプリケーションと接続する手順を説明します。

## 前提条件

- Docker Desktopがインストールされていること
- .NET 9.0 SDKがインストールされていること

## セットアップ手順

### 1. CockroachDBの起動

#### 方法1: Docker Composeを使用（推奨）

プロジェクトルートで以下のコマンドを実行：

```bash
cd docker
docker-compose up -d cockroachdb
```

これで、CockroachDBがバックグラウンドで起動します。

#### 方法2: Dockerコマンドを直接使用

```bash
docker run -d \
  --name hackstory-cockroachdb \
  -p 26257:26257 \
  -p 8080:8080 \
  -v cockroachdb-data:/cockroach/cockroach-data \
  cockroachdb/cockroach:latest \
  start-single-node --insecure
```

### 2. CockroachDBの確認

CockroachDBが正常に起動しているか確認：

```bash
# コンテナの状態を確認
docker ps | grep cockroachdb

# CockroachDBのログを確認
docker logs hackstory-cockroachdb
```

CockroachDBの管理UIにアクセス：
- URL: http://localhost:8080
- ブラウザで開くと、CockroachDBの管理画面が表示されます

### 3. データベースの作成

CockroachDBコンテナ内でSQLシェルを起動：

```bash
docker exec -it hackstory-cockroachdb ./cockroach sql --insecure
```

SQLシェルでデータベースを作成：

```sql
CREATE DATABASE IF NOT EXISTS hackstory_dev;
\q
```

または、コマンドラインから直接実行：

```bash
docker exec -it hackstory-cockroachdb ./cockroach sql --insecure -e "CREATE DATABASE IF NOT EXISTS hackstory_dev;"
```

### 4. 接続文字列の設定

`backend/HackStory.Api/appsettings.Development.json`に接続文字列が設定されていることを確認：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=26257;Database=hackstory_dev;Username=root;Password=;SSL Mode=Disable"
  }
}
```

### 5. Entity Framework Coreマイグレーションの作成

#### dotnet-efツールのインストール

まず、Entity Framework Coreのツールをインストール：

```bash
dotnet tool install --global dotnet-ef
```

**注意**: Windows環境でエラーが出る場合は、以下の方法を試してください：

1. 既存のツールをアンインストール：
   ```bash
   dotnet tool uninstall --global dotnet-ef
   ```

2. 最新バージョンをインストール：
   ```bash
   dotnet tool install --global dotnet-ef --version 9.0.0
   ```

3. または、プロジェクトローカルにインストール：
   ```bash
   cd backend/HackStory.Api
   dotnet new tool-manifest
   dotnet tool install dotnet-ef
   dotnet tool restore
   ```

#### マイグレーションの作成

バックエンドプロジェクトでマイグレーションを作成：

```bash
cd backend/HackStory.Api
dotnet ef migrations add InitialCreate --project ../HackStory.Infrastructure
```

**注意**: 
- `Microsoft.EntityFrameworkCore.Design`パッケージは既にインストールされています
- エラーが出る場合は、プロジェクトを再ビルドしてください：`dotnet build`

### 6. マイグレーションの実行

**重要**: CockroachDBは`LOCK TABLE`構文をサポートしていないため、`dotnet ef database update`コマンドは使用できません。代わりに、SQLスクリプトを手動で適用します。

#### 方法1: SQLスクリプトを生成して適用（推奨）

1. **SQLスクリプトの生成**:
   ```bash
   dotnet ef migrations script --project backend/HackStory.Infrastructure/HackStory.Infrastructure.csproj --startup-project backend/HackStory.Api/HackStory.Api.csproj --output migration.sql --idempotent
   ```

2. **CockroachDB互換スクリプトの作成**:
   生成された`migration.sql`をCockroachDB互換形式に変換する必要があります。
   - `DO $EF$`ブロックを削除
   - `CREATE TABLE IF NOT EXISTS`を使用
   - `CREATE INDEX IF NOT EXISTS`を使用

3. **スクリプトの適用**:
   ```bash
   Get-Content scripts/migration-cockroachdb.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
   ```

#### 方法2: マイグレーションファイルから直接SQLを実行

マイグレーションファイル（`backend/HackStory.Infrastructure/Migrations/20251220064352_InitialCreate.cs`）の`Up`メソッドの内容を、CockroachDB互換のSQLに変換して実行します。

**確認**: マイグレーションが正常に適用されたか確認：

```bash
docker exec hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev -e "SHOW TABLES;"
```

以下のテーブルが表示されれば成功です：
- `__EFMigrationsHistory`
- `Challenges`
- `Stories`
- `Users`
- `ChallengeProgresses`
- `ChapterProgresses`
- `UserProgresses`

### 7. 動作確認

#### バックエンドの起動

```bash
cd backend/HackStory.Api
dotnet run --urls "http://localhost:5000"
```

#### データベース接続の確認

1. Swagger UIにアクセス: http://localhost:5000/swagger
2. `/api/auth/verify`エンドポイントをテスト（Firebase認証が必要）
3. ログにデータベース接続のメッセージが表示されることを確認

#### データベースの確認

**重要**: CockroachDB管理UI（http://localhost:8080）では、テーブルデータを直接閲覧する機能は提供されていません。管理UIは主に監視と管理のためのツールです。データを確認するには、コマンドラインからSQLを実行する必要があります。

##### 方法1: SQLファイルを使用（推奨・最も簡単）

`scripts/check-users.sql`ファイルがあります。以下のコマンドで確認できます：

```bash
Get-Content scripts/check-users.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
```

##### 方法2: 直接クエリを実行

```bash
docker exec hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev --execute 'SELECT * FROM "Users";'
```

##### 方法3: SQLシェルを起動

```bash
docker exec -it hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
```

SQLシェルが起動したら、以下のクエリを実行：

```sql
-- テーブル一覧を表示
SHOW TABLES;

-- Usersテーブルの構造を確認
\d "Users";

-- データを確認
SELECT "Id", "Email", "Username", "FirebaseUid", "CreatedAt" FROM "Users";

-- 全カラムを表示
SELECT * FROM "Users";

-- ユーザー数をカウント
SELECT COUNT(*) as user_count FROM "Users";
```

SQLシェルを終了するには、`\q`と入力するか、`Ctrl+D`を押します。

**注意**: 
- CockroachDBでは、テーブル名やカラム名が大文字小文字を区別する場合は、ダブルクォートで囲む必要があります
- CockroachDB管理UIでは、テーブルのスキーマ（構造）や統計情報は確認できますが、データ（行）は表示できません

#### CockroachDB管理UIについて

管理UI（http://localhost:8080）で確認できること：

- **Overview**: クラスター全体の状態（ノード、容量、レプリケーション）
- **Databases**: データベースとテーブルの一覧、テーブルのスキーマ（構造）の確認
- **SQL Activity**: 過去に実行されたSQLステートメントの履歴（新しいクエリを実行する機能はありません）

管理UIで確認できないこと：

- テーブル内のデータ（行）の直接閲覧

## トラブルシューティング

### CockroachDBが起動しない

1. ポート26257が使用中でないか確認：
   ```bash
   netstat -an | grep 26257
   ```

2. 既存のコンテナを削除して再起動：
   ```bash
   docker stop hackstory-cockroachdb
   docker rm hackstory-cockroachdb
   docker-compose up -d cockroachdb
   ```

### マイグレーションエラー

#### 「LOCK TABLE syntax error」エラー

CockroachDBは`LOCK TABLE`構文をサポートしていないため、`dotnet ef database update`コマンドは使用できません。代わりに、SQLスクリプトを手動で適用してください（上記の「方法1」を参照）。

#### 「CREATE TABLE usage inside a function definition is not supported」エラー

CockroachDBは`DO $EF$`ブロック内での`CREATE TABLE`をサポートしていません。SQLスクリプトから`DO $EF$`ブロックを削除し、`CREATE TABLE IF NOT EXISTS`を使用してください。

#### その他のマイグレーションエラー

1. データベースが存在するか確認
2. 接続文字列が正しいか確認
3. CockroachDBが起動しているか確認
4. テーブルが既に存在する場合は、`IF NOT EXISTS`を使用

### 接続エラー

1. `appsettings.Development.json`の接続文字列を確認
2. CockroachDBのログを確認：
   ```bash
   docker logs hackstory-cockroachdb
   ```

3. ネットワーク接続を確認：
   ```bash
   telnet localhost 26257
   ```

## 便利なコマンド

### CockroachDBの停止

```bash
docker stop hackstory-cockroachdb
```

### CockroachDBの再起動

```bash
docker restart hackstory-cockroachdb
```

### データベースの削除（注意：すべてのデータが削除されます）

```bash
docker exec -it hackstory-cockroachdb ./cockroach sql --insecure -e "DROP DATABASE IF EXISTS hackstory_dev CASCADE;"
```

### ボリュームの削除（注意：すべてのデータが削除されます）

```bash
docker-compose down -v
```

## 動作確認のまとめ

セットアップが完了したら、以下を確認してください：

1. **テーブルの確認**:
   ```bash
   docker exec hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev -e "SHOW TABLES;"
   ```
   以下の7つのテーブルが作成されていることを確認：
   - `__EFMigrationsHistory`
   - `Challenges`
   - `Stories`
   - `Users`
   - `ChallengeProgresses`
   - `ChapterProgresses`
   - `UserProgresses`

2. **バックエンドの起動**:
   ```bash
   cd backend/HackStory.Api
   dotnet run --urls "http://localhost:5000"
   ```

3. **フロントエンドの起動**（別ターミナル）:
   ```bash
   cd frontend
   npm run dev
   ```

4. **ユーザー登録のテスト**:
   - ブラウザで `http://localhost:3000/auth/register` にアクセス
   - ユーザー登録を実行
   - データベースにユーザーが保存されることを確認（上記の「データベースの確認」を参照）

## 次のステップ

データベースが正常に動作していることを確認したら：

1. フロントエンドを起動して認証機能をテスト
2. ユーザー登録を行い、データベースに保存されることを確認
3. ストーリーの進捗が保存されることを確認

