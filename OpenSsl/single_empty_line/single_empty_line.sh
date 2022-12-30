awk '
# line starts with ###.
/^##########/ {
# if the previous was not ### then print empty line
if (state==1) print ""
# output current line
print $0
# change to state 0
state=0
}
# change to state 1, if line does not start with #
/^[^#]/ {
state=1
}
' test.txt