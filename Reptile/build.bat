@echo off
setlocal enabledelayedexpansion

:: 设置 Python 脚本的名称
set SCRIPT_NAME=main

:: 设置 Python 脚本的名称
set SCRIPT_FULL_NAME=%SCRIPT_NAME%.py

:: 设置可执行文件的名称
set EXECUTABLE_NAME=%SCRIPT_NAME%.exe

:: 切换到脚本所在的目录
cd /d %~dp0

:: 初始化计数器
set count=0

:: 查找以env或venv开头的文件夹并列出
for /d %%i in (env* venv*) do (
    set /a count+=1
    echo !count!. %%i
    set "env_!count!=%%i"
)

:: 如果没有找到符合条件的文件夹
if %count% equ 0 (
    echo No virtual environments found.
    exit /b 1
)

:: 选择虚拟环境
set /p choice=Choose your virtual environment (1-%count%, 0 to exit):

:: 退出选项
if "%choice%" equ "0" (
    echo Exiting...
    exit /b 0
)

:: 检查选择是否在有效范围内
if %choice% leq 0 if %choice% gtr %count% (
    echo Invalid choice.
    exit /b 1
)

:: 设置选择的虚拟环境路径
set VIRTUAL_ENV=!env_%choice%!

:: 检测图标
echo now dir: %CD%
if exist favicon.ico (
    echo Icon file exists
) else (
    echo Icon file not found
)

:: 调用 PyInstaller 来打包 Python 脚本
call %VIRTUAL_ENV%\Scripts\pyinstaller.exe --add-data "write.exe;." --icon favicon.ico --onefile %SCRIPT_FULL_NAME%

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