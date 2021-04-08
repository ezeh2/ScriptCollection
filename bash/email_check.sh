#!/bin/bash

: '
This script checks for a number of domains whethey they have SPF and DMARC.
'

DOMAINS="postfinance.ch
migros.ch
coop.ch
denner.ch
aldi.ch
hays.ch
leiern.ch
teamplanbuch.ch
symex.ch
zuehlke.ch
noser.ch
bbv.ch
erni.ch
swisscom.ch
ctc.ch
novartis.com
gmx.ch
gemalto.com
credit-suisse.com
ubs.com"

[ -f 'email_check.log' ] && rm email_check.log

while read -r line;do
	domain_ns=$(dig -t NS +noall +answer $line)
	spf_txt=$(dig -t TXT +noall +answer $line | grep -i "v=spf" )
	dmarc_txt=$(dig -t TXT +noall +answer _dmarc.$line | grep -i "v=dmarc" )
	echo "$domain_ns" >> email_check.log
	echo "$spf_txt" >> email_check.log
	echo "$dmarc_txt" >> email_check.log
	echo "" >> email_check.log

	domain_yn="No Domain"
	spf_yn="No SPF"
	dmarc_yn="No DMARC"
	if [[ $domain_ns == *"NS"* ]]
       	then
		domain_yn="DOMAIN"
	fi
	if [[ $spf_txt == *"spf"* ]]
       	then
		spf_yn="SPF"
	fi
	if [[ $dmarc_txt == *"DMARC"* ]]
       	then
		dmarc_yn="DMARC"
	fi

	echo $line";"$domain_yn";"$spf_yn";"$dmarc_yn";"
done <<< "$DOMAINS"

