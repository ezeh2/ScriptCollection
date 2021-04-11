#!/bin/bash

# trap ctrl-c and call ctrl_c()
trap ctrl_c INT

function ctrl_c() {
	tput sgr0
	tput setaf 7
	tput setbg 0
	tput clear
	exit
}



tput sgr0
tput setaf 7
tput setbg 0
tput clear

cols=$(tput cols)
lines=$(tput lines)
echo "lines: $lines cols: $cols, please press enter"
read

for i in {1..1000};do
  let row=$RANDOM%$lines
  let col=$RANDOM%cols
  let fg=$RANDOM%8
  tput cup $row $col
  tput setaf $fg 
  echo "*" 
  sleep 0.01 
done 


