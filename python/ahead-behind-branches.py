#!/usr/bin/python3

import subprocess
import numpy as np
import argparse
import os

class RelatedBranchesFinder:
    def __init__(self,relatedBranchNamesWildcard) -> None:
        self.relatedBranchNamesWildcard = relatedBranchNamesWildcard
        self.cwd = os.curdir

    def log(self,branchname, count):
        result = subprocess.run(["git","log","-"+str(count),branchname,'--format=%H;%s;%ad','--date=format:%Y-%m-%d %H:%M'], cwd=self.cwd,capture_output=True,encoding="UTF8")
        if result.returncode==0:        
            result_string = result.stdout
            lines = result_string.split("\n")
            for line in lines:
                if len(line)>0:                
                    splits = line.split(";")
                    commitId = splits[0]

                    print(line)                
                    # print(commitId)
                    self.getBranchesContains(commitId)
        else:
            print("log, error: " + str(result.returncode))
                        
    def getBranchesContains(self,commitId):
        result = subprocess.run(["git","branch","-v","-r","--contains",commitId],cwd=self.cwd,capture_output=True,encoding="UTF8")
        if result.returncode==0:        
            result_string = result.stdout
            print(result_string)
        else:
            print("getBranchesContains, error: " + result.returncode)

    # returns list of all remote branches of git-repostory
    def getBranchesRemote(self):
        # result = subprocess.run(["git","branch","-r"],cwd=self.cwd,capture_output=True,encoding="UTF8")

        # alternative to git branch: 
        #  for remote-branches:
        #   git for-each-ref 'refs/remotes/**' --format='%(refname);%(objectname);%(objecttype)'
        #  for local-branches:
        #   git for-each-ref 'refs/heads/**' --format='%(refname);%(objectname);%(objecttype)'
        # https://opensource.com/article/22/4/git-each-ref-command 

        branches = []

        result = subprocess.run(["git","for-each-ref",self.relatedBranchNamesWildcard,"--format=%(refname)"],cwd=self.cwd,capture_output=True,encoding="UTF8")

        if result.returncode==0:        
            result_string = result.stdout
            lines = result_string.split("\n")
            for line in lines:
                if len(line)>0:     
                    branches.append(line.strip())           
        else:
            print("getBranchesRemote, error: " + result.returncode)

        return branches

    # use this instead of getBranchesRemote(..)
    # to improve performance
    def getExampleBranchesRemote(self):

        branches = []

        return branches        


    def getLineCount(delf,s):

        lineCount = 0

        lines = s.split("\n")
        for line in lines:
            if len(line)>0:
                lineCount+=1
        
        return  lineCount;

    def getLines(delf,s):

        retLines = []        

        lines = s.split("\n")
        for line in lines:
            if len(line)>0:
                retLines.append(line)
        
        return  retLines

    def getAheadBehindOfBranch(self,branchName):

        aheadBehindOfBranches = []

        remoteBranches = self.getBranchesRemote()
        # remoteBranches = self.getExampleBranchesRemote()
        for remoteBranche in remoteBranches:

            # https://stackoverflow.com/questions/20433867/git-ahead-behind-info-between-master-and-branch
            # git rev-list --left-right --count webauthn...master

            # git log origin/feat/weakPasswordCode..origin/master | wc -l
            # git log origin/master..origin/feat/weakPasswordCode  | wc -l

            aheadCount = 0
            result1 = subprocess.run(["git","log","--first-parent",""+branchName,"^"+remoteBranche],cwd=self.cwd,capture_output=True,encoding="UTF8")
            if result1.returncode==0:        
                aheadCount = self.getLineCount(result1.stdout)
            else:
                print("getAheadBehindOfBranch, result1.returncode: " + str(result1.returncode))
                print("remoteBranche: " + remoteBranche)

            behindCount = 0
            result2 = subprocess.run(["git","log","--first-parent","^"+branchName,""+remoteBranche],cwd=self.cwd,capture_output=True,encoding="UTF8")
            if result2.returncode==0:        
                behindCount = self.getLineCount(result2.stdout)
            else:
                print("getAheadBehindOfBranch, result2.returncode: " + str(result2.returncode))
                print("remoteBranche: " + remoteBranche)

            mergeBase = ""
            result3 = subprocess.run(["git","merge-base",branchName,remoteBranche],cwd=self.cwd,capture_output=True,encoding="UTF8")
            if result3.returncode==0:        
                mergeBaseLines = self.getLines(result3.stdout)
                if len(mergeBaseLines)>0:
                    mergeBase = mergeBaseLines[0]
            else:
                print("getAheadBehindOfBranch, result3.returncode: " + str(result3.returncode))
                print("remoteBranche: " + remoteBranche)

            mergeBaseLogLine = ""
            result4 = subprocess.run(["git","log","-1",mergeBase,'--format=%H;%ad;%an;%cd;%cn;%s','--date=format:%Y-%m-%d %H:%M'], cwd=self.cwd,capture_output=True,encoding="UTF8")
            if result4.returncode==0:        
                logLines = self.getLines(result4.stdout)
                if len(logLines)>0:
                    mergeBaseLogLine = logLines[0]
            else:
                print("getAheadBehindOfBranch, result4.returncode: " + str(result4.returncode))
                print("remoteBranche: " + remoteBranche)

            aheadBehindOfBranches.append([aheadCount,behindCount,remoteBranche,mergeBase, mergeBaseLogLine])      
            # print(str(aheadCount) + " commits ahead and " + str(behindCount) +" commits behind " + remoteBranche + " merge-base logline: " + mergeBaseLogLine)          

        # print('###')
        # print('###')
        return aheadBehindOfBranches


def sortByColumn0(item):
    return item[0]*1000000+item[1]

parser = argparse.ArgumentParser(description='calculate number of commits ahead begin every branch (reference)')
parser.add_argument('branch', type=str,help='name of branch to calculate ahead and behind commits')  
parser.add_argument('-p', type=str,required=False,default='refs/remotes/**',help='pattern to get branches to calculate ahead begind against')  

args = parser.parse_args()




relatedBranchesFinder = RelatedBranchesFinder(args.p)

# relatedBranchesFinder.log('origin/develop',20)


aheadOfBranches = relatedBranchesFinder.getAheadBehindOfBranch(args.branch)

aheadOfBranches.sort(key=sortByColumn0)

print(" ")
print("branch pattern: " + args.p)
print(" ")
print("branch " + args.branch +" is")
for aheadOfBranch in aheadOfBranches:
    print(str(aheadOfBranch[0]) + " commits ahead and " + str(aheadOfBranch[1]) +" commits behind " + aheadOfBranch[2] + " merge-base logline: " + aheadOfBranch[4])



