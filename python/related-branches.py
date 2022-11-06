#!/usr/bin/python3

import subprocess
import numpy as np
import argparse
import os

def getLogEntries(cwd, branch_name,count):

    # similar to but with autor and with date --oneline: 
    # git log -3 --format="%C(auto)%h %cd %an %d %s" --date='format:%Y-%m-%d %H:%M'
    # git config --global --add format.pretty "%C(auto)%h %cd %an %d %s"
    # git config --global --replace-all log.date 'format:%Y-%m-%d %H:%M'
    result = subprocess.run(["git","log","-"+str(count),branch_name,'--format=%H;%s;%ad','--date=format:%Y-%m-%d %H:%M'], cwd=_cwd,capture_output=True,encoding="UTF8")
    result_string = result.stdout

    # put git history into 2d-array
    data = []

    lines = result_string.split("\n")
    for line in lines:
        if len(line)>0:
            cells = line.split(";")
            #print(line)        
            #print(cells[0])        
            r2 = subprocess.run(["git","branch","--contains",cells[0]], cwd=_cwd,capture_output=True,encoding="UTF8")
            # print(r2.stdout)
            data.append(cells)
    return data

def getBranches(cwd):
    result = subprocess.run(["git","branch"], cwd=_cwd,capture_output=True,encoding="UTF8")
    result_string = result.stdout
    lines = result_string.split("\n")

    branches = []
    for line in lines:
        if len(line)>0:
            line = line.strip()
            branch = line
            parts = line.split(" ")
            if len(parts)>1:
                branch = parts[1]
            branches.append(branch)

    return branches

def getMergeBases(cwd,myBranch):

    mergeBases = []

    branches = getBranches(cwd)
    for branch in branches:
        if branch!=myBranch:
            result = subprocess.run(["git","merge-base",myBranch,branch], cwd=_cwd,capture_output=True,encoding="UTF8")
            result_string = result.stdout
            lines = result_string.split("\n")
            if result.returncode==0:
                commitId = lines[0]

                logEntryForCommitId = getLogEntries(cwd, commitId, 1)

                mergeBaseEntry = [branch]
                mergeBaseEntry.extend(logEntryForCommitId[0])

                mergeBases.append(mergeBaseEntry)

    return mergeBases

parser = argparse.ArgumentParser(description='Find related branches')
parser.add_argument('branch', type=str,help='name of branch to look for related branches')  
parser.add_argument('-c', dest="count", type=int,help='maximum count of related branches to show')  

args = parser.parse_args()

_cwd = os.curdir
_branch_name = args.branch
_count = args.count

print(" ")

mergeBases = getMergeBases(_cwd, _branch_name)

mergeBases_np = np.array(mergeBases)

# sort by DateTime descending (column3)
if len(mergeBases_np.shape)==2:
    mergeBases_np= mergeBases_np[mergeBases_np[:,3].argsort()]
    mergeBases_np = np.flip(mergeBases_np,0)
    # print(mergeBases_np)

    print("\'"+_branch_name + "\'" + " was forked at")

    for mergeBase in mergeBases_np[:_count,:]:
        # print(mergeBase)
        print(mergeBase[1] + " " + mergeBase[3] + " from " + mergeBase[0])

else:
    print("no related branches found")


print(" ")