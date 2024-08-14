@echo off
setlocal enabledelayedexpansion
%~d0

cd Reptile

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

:: 开始爬取
echo start Reptile...
%VIRTUAL_ENV%\Scripts\python.exe Reptile.py
pause
goto end

::以下内容已经废弃

:base
cd %~dp0Reptile/env/Scripts
call activate
start %~dp0runInner.bat

:end
exit
