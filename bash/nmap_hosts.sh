#!/bin/bash

# list of hosts to be scanned
#hosts=('sunrise' 'Sun' 'EdwardZehsiPad' 'Galaxy-A8-2018' 'DESKTOP-Q0NRAHP' 'Galaxy-J32017' 'Chromecast' 'LAPTOP-K8G8TGAS' 'W1375')
#hosts=('EdwardZehsiPad' 'Galaxy-A8-2018' 'Chromecast' 'sunrise' 'Sun' 'DESKTOP-Q0NRAHP' )
hosts=('192.168.56.1' '192.168.1.92' 'sunrise' 'Sun' '127.0.0.1' )



# timestamp used to build name of directory
d=`date +%Y-%d-%m_%H_%M_%S` 
dir=nmap_host_$d
mkdir $dir

# loop over all host names
j=0
while [ $j -lt ${#hosts[*]} ];do
	echo ${hosts[$j]}
	# scan host with nmap and write result to file
	nmap -vvv -T5 --min-parallelism 5 --reason -Pn -sSU -A ${hosts[$j]} --top-ports 30 > nmap_host_$d/${hosts[$j]}.txt
	# nmap ${hosts[$j]} > nmap_host_$d/${hosts[$j]}.txt
	# out=`ping -c 1 ${hosts[$j]}`
	# echo $out > nmap_host_$d/${hosts[$j]}.txt
	#echo $out
        ((j=j+1))
done

