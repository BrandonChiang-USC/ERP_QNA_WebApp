-- MSSQL Database Initialization Script
-- Run this script to create the database and table manually

-- Create database (run as sa or admin if needed)
-- CREATE DATABASE erp_qna;
-- GO

-- Use the database
-- USE erp_qna;
-- GO

-- Create table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='qna' AND xtype='U')
CREATE TABLE qna (
    id INT IDENTITY(1,1) PRIMARY KEY,
    question NVARCHAR(2000) NOT NULL,
    answer NVARCHAR(MAX) NOT NULL,
    tags NVARCHAR(500),
    remark NVARCHAR(1000),
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2
);
GO

-- Create index for better search performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_qna_created_at' AND object_id = OBJECT_ID('qna'))
CREATE INDEX idx_qna_created_at ON qna(created_at DESC);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_qna_tags' AND object_id = OBJECT_ID('qna'))
CREATE INDEX idx_qna_tags ON qna(tags) WHERE tags IS NOT NULL;
GO
