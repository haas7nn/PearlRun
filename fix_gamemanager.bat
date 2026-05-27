@echo off
title PearlRun GameManager Fix
color 0A

echo ========================================
echo   PearlRun GameManager Reference Fix
echo ========================================
echo.
echo This will replace 'GameManager' with 'Level3GameManager'
echo in PlayerController.cs and PlayerCollision.cs
echo.
echo Press any key to continue...
pause > nul

set "PROJECT_PATH=C:\Users\Haas7n\Documents\GamesDevProject2026\PearlRun\PearlRun\Assets"
set "FILE1=%PROJECT_PATH%\Scripts_HASAN\Player\PlayerController.cs"
set "FILE2=%PROJECT_PATH%\Scripts_HASAN\Player\PlayerCollision.cs"

echo.
echo Processing: PlayerController.cs
if exist "%FILE1%" (
    powershell -Command "(Get-Content '%FILE1%') -replace '(?<![a-zA-Z0-9_])GameManager(?![a-zA-Z0-9_])', 'Level3GameManager' | Set-Content '%FILE1%'"
    echo [OK] Fixed PlayerController.cs
) else (
    echo [ERROR] File not found: %FILE1%
)

echo.
echo Processing: PlayerCollision.cs
if exist "%FILE2%" (
    powershell -Command "(Get-Content '%FILE2%') -replace '(?<![a-zA-Z0-9_])GameManager(?![a-zA-Z0-9_])', 'Level3GameManager' | Set-Content '%FILE2%'"
    echo [OK] Fixed PlayerCollision.cs
) else (
    echo [ERROR] File not found: %FILE2%
)

echo.
echo ========================================
echo   FIX COMPLETE
echo ========================================
echo.
echo Next steps:
echo 1. Go back to Unity
echo 2. Unity will auto-recompile
echo 3. Check Console for errors
echo.
pause
