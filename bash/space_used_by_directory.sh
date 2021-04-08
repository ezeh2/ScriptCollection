#!/bin/bash

FileCount=0
TotalSpace=0
while IFS= read -r line
do
   ## take some action on $line
  # echo "$line"
  space=`echo $line | cut -d " " -f 6`
  echo "space: "$space
  TotalSpace=$((TotalSpace+space))
  FileCount=$((FileCount+1))
done < <(ls -l)

echo "TotalSpace: ":$TotalSpacea
echo "File Count: ":$FileCount
