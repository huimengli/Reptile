@echo off
cd Reptile/env/Scripts
call activate
start %~dp0runInner.bat
exit
