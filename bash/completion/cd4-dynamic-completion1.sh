#!/bin/bash

alias cd4='cd'

_cd4_completions()
{
  if [ "${#COMP_WORDS[@]}" == "2" ]; then
	# first parameter is search term
	search_term=${COMP_WORDS[1]}
	# use grep if search term is defined
	[[ $search_term ]] && COMPREPLY=($(cat test_directories | grep $search_term))
	# DON'T use grep if search term is NOT defined
	[[ $search_term ]] || COMPREPLY=($(cat test_directories))
  fi
} 


complete -F _cd4_completions cd4 

