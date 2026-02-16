@echo off
title Git Reproducer Dev Launcher

echo Starting API...
start "API" cmd /k dotnet run --project .\src\Api\

timeout /t 2 > nul

echo Starting Worker...
start "Worker" cmd /k dotnet run --project .\src\Workers\

timeout /t 2 > nul

echo Opening Test Terminal...
start "Test Client" cmd

echo.
echo ======================================
echo API: http://localhost:5000
echo Worker: running in background
echo Use the third terminal for requests
echo ======================================