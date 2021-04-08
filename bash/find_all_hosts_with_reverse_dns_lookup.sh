#!/bin/bash

# reverse DNS lookup for address range 192.168.1.0 - 192.168.1.255
# this allows to get a list of recently used IP adresses,
# which is much faster than ping


for i in {0..255};do
#	echo $i
# use PTR instead of -x, because -x does not support +noall +answer
dns=`dig PTR +noall +answer $i.99.168.192.in-addr.arpa`
c=`echo $dns | wc -m`
if [ $c -gt 1 ]
then
	echo $dns
	#ip=192.168.1.$i
	#iping -c 1 $ip 
fi
done
