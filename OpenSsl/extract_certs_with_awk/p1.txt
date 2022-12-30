#!/bin/bash
#https://www.gnu.org/software/gawk/manual/html_node/index.html
#
awk '
BEGIN {
# field separator, so that $2 returns everything after "CN = "
FS="CN = "
}
# selects line which contains CN (e.g.  0 s:CN = www.openssl.org)
/^ [0-9]+ s:.*CN = / {
# use CN (e.g. www.openssl.org) as filename
filename=$2".cer"
}
# write all lines between "BEGIN CERTIFICATE" and "END CERTIFICATE" to filename
/BEGIN CERTIFICATE/,/END CERTIFICATE/ {
print $0 > filename
}
' out.txt