-- PostgreSQL Database Initialization Script
-- Run this script to create the database and table manually

-- Create database (run as superuser if needed)
-- CREATE DATABASE erp_qna;

-- Connect to erp_qna database, then run:
CREATE TABLE IF NOT EXISTS ERP_QNA (
    id SERIAL PRIMARY KEY,
    question VARCHAR(2000) NOT NULL,
    answer TEXT NOT NULL,
    tags VARCHAR(500),
    remark VARCHAR(1000),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE
);

-- Create index for better search performance
CREATE INDEX IF NOT EXISTS idx_qna_created_at ON ERP_QNA(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_qna_tags ON ERP_QNA(tags) WHERE tags IS NOT NULL;
