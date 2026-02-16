@echo off
title Git Reproducer Dev Launcher

echo Building solution...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed. Fix errors before running.
    pause
    exit /b
)

echo Starting API...
start "API" powershell -NoExit -Command "dotnet run --no-build --project src/Api"

timeout /t 2 > nul

echo Starting Worker...
start "Worker" powershell -NoExit -Command "dotnet run --no-build --project src/Workers"

timeout /t 2 > nul

echo Opening Test Terminal...
start "Test Client" powershell -NoExit

echo.
echo ======================================
echo API:    http://localhost:5000
echo Worker: running in background
echo ======================================