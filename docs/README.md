# HackStory ドキュメント

このディレクトリには、HackStoryプロジェクトのドキュメントが含まれています。

## 主要なドキュメント

### セットアップガイド

- **[quick-start.md](./quick-start.md)**: クイックスタートガイド
  - Firebase設定
  - アプリケーションの起動方法
  - 基本的な動作確認
  - CockroachDBのクイックセットアップ

- **[cockroachdb-setup.md](./cockroachdb-setup.md)**: CockroachDBセットアップガイド
  - ローカル環境でのCockroachDBの起動方法
  - データベースの作成とマイグレーション
  - データベースの確認方法（コマンドライン）
  - CockroachDB管理UIについて
  - トラブルシューティング

### その他のドキュメント

- **[requirements.md](./requirements.md)**: 要件定義書
- **[testing-guide.md](./testing-guide.md)**: テストガイド
- **[verification-summary.md](./verification-summary.md)**: 検証サマリー
- **[cost-optimization-strategy.md](./cost-optimization-strategy.md)**: コスト最適化戦略

## 重要な注意事項

### CockroachDB管理UIについて

**CockroachDB管理UI（http://localhost:8080）では、テーブルデータを直接閲覧する機能は提供されていません。**

- 管理UIは主に**監視と管理**のためのツールです
- テーブルの**スキーマ（構造）**や**統計情報**は確認できますが、**データ（行）**は表示できません
- データを確認するには、**コマンドラインからSQLを実行する必要があります**

詳細は [cockroachdb-setup.md](./cockroachdb-setup.md) の「データベースの確認」セクションを参照してください。

### データ確認の推奨方法

`scripts/check-users.sql`ファイルがあります。以下のコマンドで確認できます：

```bash
Get-Content scripts/check-users.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
```

