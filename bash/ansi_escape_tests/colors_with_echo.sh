#!/bin/bash

tput clear
echo "hello"
echo  -e "\e[0;0H"
echo "line 0"

echo -e "\e[2;0H"
echo "line 2"

echo -e "\e[4;0H"
echo "line 4"

sleep 5 
tput clear


for i in {1..10};do 
  echo -e "\e[0;0H"
  echo $i
#  echo -e "\033[5;5f"
  echo -e "\033[5C"
  echo $i
  sleep 1 
done 
