#!/bin/bash

# https://gist.github.com/komasaru/789216cf9cf5e1aa339c#file-test_bash_cursor_2-sh-L9

# Clear the screen
tput clear

# Write a frame
echo
echo "  +---------------------+"
echo "  |                     |"
echo "  +---------------------+"
echo

# Set font
tput setb 1
tput setf 6
tput bold

# Output date and time
for i in {0..4};
do
  sleep 1
  tput cup 2 4
  echo -n `date +"%Y-%m-%d %H:%M:%S"`
done;
tput cup 2 4
echo -n `date +"%Y-%m-%d %H:%M:%S"`

# Move the cursor position
tput cud 2
echo

# Reset tput setting
tput init

