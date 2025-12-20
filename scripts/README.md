# SQLスクリプト

このディレクトリには、データベース操作用のSQLスクリプトが含まれています。

## ファイル一覧

### check-users.sql

ユーザーデータを確認するためのSQLクエリです。

**使用方法**:
```bash
Get-Content scripts/check-users.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
```

### migration-cockroachdb.sql

CockroachDB用のマイグレーションスクリプトです。EF CoreのマイグレーションをCockroachDB互換形式に変換したものです。

**使用方法**:
```bash
Get-Content scripts/migration-cockroachdb.sql | docker exec -i hackstory-cockroachdb ./cockroach sql --insecure -d hackstory_dev
```

**注意**: このファイルは`.gitignore`に含まれているため、Gitにはコミットされません。必要に応じて再生成してください。

