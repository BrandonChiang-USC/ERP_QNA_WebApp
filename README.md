# ERP Q&A System

一個使用 .NET 10.0 + Blazor 建構的 ERP 問答管理系統，用於管理企業資源規劃相關的知識庫。

## 功能特色

- **問答管理**: 新增、編輯、刪除 Q&A 記錄
- **全文搜尋**: 在問題、答案、備註欄位中搜尋關鍵字
- **標籤篩選**: 依據分類標籤過濾問答內容
- **Excel 匯入**: 批次從 Excel 檔案匯入 Q&A 資料
- **響應式介面**: 支援桌面與行動裝置

## 技術架構

| 項目 | 技術 |
|------|------|
| 框架 | ASP.NET Core Blazor (Interactive Server) |
| 語言 | C# 12 |
| 資料庫 | PostgreSQL |
| ORM | Entity Framework Core |
| UI 元件 | Microsoft Fluent UI |
| Excel 處理 | ClosedXML |
| 執行環境 | .NET 10.0 |

## 系統需求

- .NET 10.0 SDK
- PostgreSQL 14+

## 安裝與執行

### 1. 安裝 PostgreSQL (Windows 11)

1. 前往 [PostgreSQL 官網](https://www.postgresql.org/download/windows/) 下載安裝程式
2. 執行安裝程式，記住設定的密碼（預設使用者為 `postgres`）
3. 安裝完成後，PostgreSQL 服務會自動啟動

### 2. 複製專案

```bash
git clone https://github.com/BrandonChiang-USC/ERP_QNA_WebApp.git
cd ERP_QNA_WebApp
```

### 3. 建立資料庫

開啟 pgAdmin 或使用 psql 命令列工具：

```sql
CREATE DATABASE erp_qna;
```

### 4. 建立資料表

連線到 `erp_qna` 資料庫後，執行以下 SQL：

```sql
CREATE TABLE qna (
    "Id" SERIAL PRIMARY KEY,
    "Question" VARCHAR(2000) NOT NULL,
    "Answer" TEXT NOT NULL,
    "Tags" VARCHAR(500),
    "Remark" VARCHAR(1000),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedAt" TIMESTAMP
);
```

或者使用 EF Core 遷移自動建立：

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

### 5. 設定連線字串

編輯 `appsettings.json`，修改資料庫連線設定：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=erp_qna;Username=postgres;Password=你的密碼"
  }
}
```

### 6. 啟動應用程式

```bash
dotnet run
```

應用程式將在以下位址啟動：
- HTTP: http://localhost:5183
- HTTPS: https://localhost:7019

## 資料結構

| 欄位 | 類型 | 說明 |
|------|------|------|
| Id | int | 主鍵，自動遞增 |
| Question | string | 問題 (必填，最多 2000 字元) |
| Answer | string | 答案 (必填) |
| Tags | string | 分類標籤 (選填) |
| Remark | string | 備註 (選填，最多 1000 字元) |
| CreatedAt | DateTime | 建立時間 (UTC) |
| UpdatedAt | DateTime | 更新時間 |

## Excel 匯入格式

匯入的 Excel 檔案需包含以下欄位（第一列為標題）：

| Question | Answer | Tags | Remark |
|----------|--------|------|--------|
| 問題內容 | 答案內容 | 標籤 | 備註 |

## 授權

MIT License
