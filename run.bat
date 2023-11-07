@echo off
%~d0

cd Reptile
%CD%/env/Scripts/python.exe Reptile.py
pause
goto end

::以下内容已经废弃

:base
cd %~dp0Reptile/env/Scripts
call activate
start %~dp0runInner.bat

:end
exit
