#!/bin/bash
#https://www.gnu.org/software/gawk/manual/html_node/index.html
#
awk '
BEGIN {
FPAT = "([^,]*)|(\"[^\"]+\")"
}
{
   print $2
   print $3
   print $4
}
' testdata.txt