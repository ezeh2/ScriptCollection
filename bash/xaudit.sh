#!/bin/bash
echo "$@"

logdir="xaudit_log"
[[ -d $logdir ]] || mkdir $logdir

date=`date --iso-8601=seconds`
logfilename=$(echo "$@" | sed 's/ /_/g')_$date.log
#echo "$logfilename"

sleep_after_tshark_start=2
duration=6
# start tshark in background
tshark -a duration:$duration -i eth0 -w $logdir/$logfilename.pcap  >> $logdir/$logfilename.tshark.log 2>&1 &
echo "tshark started"
sleep $sleep_after_tshark_start
echo "sleep $sleep_after_tshark_start done"

echo "$@" > $logdir/$logfilename
echo "" >> $logdir/$logfilename
exec "$@" 2>&1| tee -a $logdir/$logfilename

echo ""
echo "wait for tshark to finish"
wait %1
echo "tshark finished"

