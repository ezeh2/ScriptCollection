#!/bin/bash

# reverse DNS lookup for address range 192.168.1.0 - 192.168.1.255
# this allows to get a list of recently used IP adresses,
# which is much faster than ping

adress_prefix=192.168.1
adress_prefix_reverse=1.168.192

for i in {0..255};do
# echo $i
# use PTR instead of -x, because -x does not support +noall +answer
dns=`dig PTR +noall +answer $i.$adress_prefix_reverse.in-addr.arpa`
# number of characters in $dns>0 means dig found hostname for ip-adress
c=`echo $dns | wc -m`
if [ $c -gt 1 ]
then
	echo "dig: "$dns
	# ping host so that mac-adress is in arp-table
	nmap=`nmap -sn $adress_prefix.$i`
	echo "nmap -sn: "$nmap
fi
done

# this shows hostname, ip-adress and mac-adress
arp -a


