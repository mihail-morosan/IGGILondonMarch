@ECHO OFF
ECHO.Please wait while the level is generated...
clingo --rand-freq=1 --seed=%RANDOM% %1 > %2
ECHO ON