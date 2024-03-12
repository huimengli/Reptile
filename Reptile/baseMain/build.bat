@echo off
setlocal

:: 设置 Python 脚本的名称
set SCRIPT_NAME=main

:: 设置 Python 脚本的名称
set SCRIPT_FULL_NAME=%SCRIPT_NAME%.py

:: 设置可执行文件的名称
set EXECUTABLE_NAME=%SCRIPT_NAME%.exe

:: 切换到脚本所在的目录
cd /d %~dp0

:: 调用 PyInstaller 来打包 Python 脚本
call env\Scripts\pyinstaller.exe --add-data "write.exe;." --onefile %SCRIPT_FULL_NAME%

:: 检查 main.exe 是否生成
if not exist dist\%EXECUTABLE_NAME% (
    echo Error: Failed to create %EXECUTABLE_NAME%
    exit /b 1
)

:: 将打包好的可执行文件移动到当前目录
move /Y dist\%EXECUTABLE_NAME% .\%EXECUTABLE_NAME%

:: 清理打包过程中生成的临时文件和目录
rmdir /s /q build
rmdir /s /q dist
rmdir /s /q __pycache__
del /q %SCRIPT_NAME%.spec

echo Done.
endlocal