#!/bin/bash

# https://invisible-island.net/ncurses/man/terminfo.5.html#h3-Highlighting_-Underlining_-and-Visible-Bells 

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

