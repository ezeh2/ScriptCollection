#!/bin/bash
ifconfig wlan0 down
iwconfig wlan0 mode monitor
macchanger -r wlan0
ifconfig wlan0 up
airmon-ng check kill
airmon-ng check
iwconfig wlan0 | grep Mode

