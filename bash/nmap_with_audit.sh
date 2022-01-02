#!/bin/bash

# This scripts adds auditing to nmap.
# Plase add following line to your .bashrc
# alias nmap=~/git_repositories/ScriptCollection/bash/nmap_with_audit.sh

# nmap is invoked by passing all parameters ($*).
# Everything is written to a logfile in nmap.logs.d/nn/nmap.log .
# nn is derived from passed arguments.

# build directory-name from passed arguments: everything except lettes and digits are converted to _.
DIRNAME=~edward/nmap.logs.d/"$(echo -e $* | sed 's/[^0-9A-Za-z]/_/g')"
echo $DIRNAME

# create directory nmap.logs.d if not already there
[ -d nmap.logs.d ] || mkdir nmap.logs.d 
# create directory nmap.logs.d if not already there
[ -d $DIRNAME ] || mkdir $DIRNAME 

# write passed arguments to logfile
echo "###" >> $DIRNAME/nmap.log
date >> $DIRNAME/nmap.log
echo "nmap $*" >> $DIRNAME/nmap.log
echo "###" >> $DIRNAME/nmap.log

# execute nmap and write output also to logfile
/usr/bin/nmap $* 2>&1 | tee -a $DIRNAME/nmap.log
echo "---" >> $DIRNAME/nmap.log
echo "" >> $DIRNAME/nmap.log



