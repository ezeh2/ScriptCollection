#!/bin/bash


PS1=""
tput clear

tput cup 0 0
echo "position: 0 0"

tput cup 5 5
echo "position: 5 5"

tput cup 6 10
tput bold
echo "bold at position: 6 10"

tput cup 7 0
echo "cols:"
tput cols
echo "lines:"
tput lines


# https://invisible-island.net/ncurses/man/terminfo.5.html#h3-Color-Handling
#  Color       #define       Value       RGB
#  black     COLOR_BLACK       0     0, 0, 0
#  red       COLOR_RED         1     max,0,0
#  green     COLOR_GREEN       2     0,max,0
#  yellow    COLOR_YELLOW      3     max,max,0
#  blue      COLOR_BLUE        4     0,0,max
#  magenta   COLOR_MAGENTA     5     max,0,max
#  cyan      COLOR_CYAN        6     0,max,max
#  white     COLOR_WHITE       7     max,max,max
tput cup 2 0
tput setaf 3
tput setab 2
echo "foregr=3, backgr=2"


tput setaf 7
tput setab 0
echo "back to normal"



