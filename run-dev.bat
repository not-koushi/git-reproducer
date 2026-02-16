@echo off
title Git Reproducer

dotnet build
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b
)

powershell -ExecutionPolicy Bypass -File client.ps1