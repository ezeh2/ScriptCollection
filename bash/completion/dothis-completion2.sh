#!/bin/bash

#https://iridakos.com/programming/2018/03/01/bash-programmable-completion-tutorial

_dothis_completions()
{
	  COMPREPLY+=("here")
	    COMPREPLY+=("there")
	      COMPREPLY+=("nowhere")
      }

complete -F _dothis_completions dothis



