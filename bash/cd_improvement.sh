#!/bin/bash

#https://mhoffman.github.io/2015/05/21/how-to-navigate-directories-with-the-shell.html
#how to use this:

#source ./cd_improvements.sh


function cd() {
  if [ "$#" = "0" ]
  then
  pushd ${HOME} > /dev/null
  elif [ -f "${1}" ]
  then
    ${EDITOR} ${1}
  else
  pushd "$1" > /dev/null
  fi
}

function bd(){
  if [ "$#" = "0" ]
  then
    popd > /dev/null
  else
    for i in $(seq ${1})
    do
      popd > /dev/null
    done
  fi
}

alias lln="ls -lhtr  --time-style long-iso | tac | cat -n | tac | sed -s 's/^\s*\([0-9]*\)\s*\(.*\)/[\1]  \2 [\1]/'g && pwd"
function lf() {
    if [ "x${1}" == "x" ]
    then
        n=1 
    else
        n="${1}"
    fi  
    ls -rt1 | tail -n ${n} | head -n 1
}


