#!/bin/bash

DIRNAME=nmap.logs.d/"$(echo -e $* | sed 's/[^0-9A-Za-z]/_/g')"
echo $DIRNAME

[ -d nmap.logs.d ] || mkdir nmap.logs.d 
[ -d $DIRNAME ] || mkdir $DIRNAME 

echo "###" >> $DIRNAME/nmap.log
echo "nmap $*" >> $DIRNAME/nmap.log
echo "###" >> $DIRNAME/nmap.log

/usr/bin/nmap $* 2>&1 | tee -a $DIRNAME/nmap.log
echo "---" >> $DIRNAME/nmap.log
echo "" >> $DIRNAME/nmap.log



