#!/bin/bash

# https://invisible-island.net/ncurses/man/terminfo.5.html#h3-Highlighting_-Underlining_-and-Visible-Bells 



# clear screen
tput clear
# clear all attributes (except colors)
tput sgr0

echo "cols:"
tput cols
echo "lines:"
tput lines

read



# clear screen
tput clear
# clear all attributes (except colors)
tput sgr0

tput bold
echo "bold"

tput sgr0
tput blink
echo "blink"

tput sgr0
tput smul
echo "underline"

tput sgr0
tput sitm
echo "italic"

tput sgr0
echo "normal"

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

tput sgr0
tput setaf 7
tput setab 1
printf "white on red"

tput setaf 4
tput setab 6 
#printf "\n"
tput cud1
printf "blue on cyan"

tput setaf 7
tput setab 0 
#printf "\n"
tput cud1

tput sgr0
printf "normal"
#printf "\n"
tput cud1

echo "please enter key"
read

tput clear
tput cup 0 0
echo "0 0"
tput cup 0 5
echo "0 5"
tput cup 2 15
echo "2 15"
tput cup 4 20
echo "4 20"


read


