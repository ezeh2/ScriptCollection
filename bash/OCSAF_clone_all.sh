#!/bin/bash

: '
This bash script fetches all github repositories of user "OCSAF".
curl is used to download html-page of repository tab. This html page
is then parsed for html-links pointing to repositories.

A directory with name of user is created. And in this direcory all
repositories are fetched.

This script makes use of:
* here string  (<<<)
* double bracket conditional compound command [[ ... ]]

Please not that this script does NOT make use of:
* pipes (|)
* temporary files

'

USER=OCSAF
GITHUB_URL=https://github.com
URL=$GITHUB_URL/$USER?tab=repositories

html_content=`curl $URL 2>null`
line_count=`wc -l <<< $html_content`
if [[ $line_count -lt 2 ]]; then
	echo "User $USER does not exist on github"
	exit 1
else
	echo "number of lines returned in HTML file: $line_count"
fi
#echo "$html_content"

# ask user to remove directory $OCSAF, if already exists
if [[ -d $USER ]]; then
	echo "Directory $USER already exists."
	read -p "Do you want to delete it ? [y|N]" yn
	if [[ $yn == "y" ]]; then
		echo "deleting directory $USER"
		rm -r -f $USER
	else
		echo "cloning without deleting existing directory is not possible"
		exit 1
	fi
fi
mkdir $USER
cd $USER

# iterate over $html_content line by line
while IFS= read -r line; do
if [[ $line =~ "codeRep" ]]; then
   #echo "$line"
   # use " as field seprator, get field 2
   REPOSITORY=`cut -d "\"" -f 2 <<<$line`
   #echo "cloning "$GITHUB_URL$REPOSITORY
   git clone $GITHUB_URL$REPOSITORY
fi
done <<< "$html_content"

