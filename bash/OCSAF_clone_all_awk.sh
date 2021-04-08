curl https://github.com/OCSAF?tab=repositories > ocsaf.txt
grep /OCSAF/.*codeRep  ocsaf.txt > reps.txt
awk -F \" '{print "git clone https://github.com/" $2}' reps.txt > reps.sh
chmod +x reps.sh
./reps.sh
