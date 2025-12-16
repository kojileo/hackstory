# HackStory（ハックストーリー）

物語を進めながら、実践的なハッキング技術とセキュリティ知識を学習できるインタラクティブなWebアプリケーション。

## プロジェクト概要

HackStoryは、ゲーミフィケーションの要素を取り入れ、楽しみながらセキュリティスキルを身につけられる学習プラットフォームです。

## 技術スタック

- **フロントエンド**: Next.js 14 (TypeScript, Tailwind CSS)
- **バックエンド**: .NET 8 (ASP.NET Core)
- **データベース**: CockroachDB Serverless
- **インフラ**: Google Cloud Run
- **CI/CD**: GitHub Actions

## プロジェクト構造

```
hackstory/
├── frontend/          # Next.js アプリケーション
├── backend/           # .NET API
├── docs/              # ドキュメント
├── docker/            # Docker設定ファイル
├── .github/           # CI/CD設定
└── README.md
```

## コスト最適化方針

本プロジェクトは低コストでの運用を重視して設計されています。

### コスト削減戦略

1. **Cloud Runの最適化**
   - 最小インスタンス数0（コールドスタート許容）
   - リソース制限の適切な設定（CPU 1コア、メモリ 512MB-1GB）
   - リクエストタイムアウトの設定

2. **データベースの最適化**
   - CockroachDB Serverless（従量課金、無料枠活用）
   - クエリの最適化（インデックス、必要なカラムのみ取得）
   - 接続プールの適切な設定

3. **キャッシング戦略**
   - フロントエンド: 静的JSONファイル（Cloud Storage不要）
   - バックエンド: インメモリキャッシュ（Redis不要）
   - APIレスポンス: キャッシュヘッダーの設定

4. **API呼び出しの削減**
   - フロントエンドで直接JSON読み込み
   - デバウンス・バッチ処理
   - SSG/ISRの活用

5. **モニタリング・アラート**
   - 予算アラートの設定（月額$20上限）
   - リソース使用率の監視
   - 異常検知

### コスト見積もり（1000ユーザー）

- **Cloud Run**: $0-5/月（無料枠内）
- **CockroachDB Serverless**: $0（無料枠内）
- **その他**: $0-5/月
- **合計**: **$0-10/月**（目標: $20以下）

詳細は `docs/requirements.md` を参照してください。

## 開発環境セットアップ

### 必要な環境

- .NET 8 SDK
- Node.js 18以上
- Docker & Docker Compose
- Git

### セットアップ手順

1. リポジトリのクローン
```bash
git clone <repository-url>
cd hackstory
```

2. バックエンドのセットアップ
```bash
cd backend
dotnet restore
dotnet build
```

3. フロントエンドのセットアップ
```bash
cd frontend
npm install
```

4. ローカル開発環境の起動
```bash
docker-compose up -d
```

## 開発ガイドライン

- コード品質: ESLint、Prettier（フロントエンド）、StyleCop（バックエンド）
- テストカバレッジ: 80%以上を目標
- コミットメッセージ: 明確で説明的なメッセージを使用

## ライセンス

[ライセンスファイルを追加予定]

## ドキュメント

- [ビジネス要件定義書](docs/requirements.md)
- [開発計画](.cursor/plans/)

