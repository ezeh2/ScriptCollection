#!/bin/bash

#https://iridakos.com/programming/2018/03/01/bash-programmable-completion-tutorial


# COMP_WORDS: an array of all the words typed after the name of the program the compspec belongs to
# COMP_CWORD: an index of the COMP_WORDS array pointing to the word the current cursor is at - in other words, the index of the word the cursor was when the tab key was pressed
# COMP_LINE: the current command line


_dothis_completions()
{
	  COMPREPLY=($(compgen -W "basel zuerich bern" "${COMP_WORDS[1]}"))
}

complete -F _dothis_completions dothis



