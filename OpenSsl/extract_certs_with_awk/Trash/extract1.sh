awk '
BEGIN {b=0}
/BEGIN CERTIFICATE/ {
b=1
}
/END CERTIFICATE/ {
print $0
b=0
}
{
if (b==1) print $0
}
' out.txt
