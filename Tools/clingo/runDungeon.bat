@ECHO OFF
ECHO.Please wait while the level is generated...
clingo --rand-freq=1 --seed=%RANDOM% levelDungeon.txt > finalDungeon.txt
ECHO ON