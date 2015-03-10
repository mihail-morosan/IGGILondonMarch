@ECHO OFF
ECHO.Please wait while the level is generated...
clingo --rand-freq=1 --seed=%RANDOM% level.txt > final2.txt
ECHO ON