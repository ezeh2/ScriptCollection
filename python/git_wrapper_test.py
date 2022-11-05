import subprocess
import numpy as np

_cwd = "/home/edward/git_repositories/ExperimentsWithGit2"
_branch_name = "exp1/doc"

result = subprocess.run(["git","log","-5",_branch_name,'--format=%H;%s;%aD'], cwd=_cwd,capture_output=True,encoding="UTF8")
result_string = result.stdout

# put git history into 2d-array
data = []

lines = result_string.split("\n")
for line in lines:
    if len(line)>0:
        print(line)        
        cells = line.split(";")
        data.append(cells)

# print(len(data))
# print(len(data[0]))
# print(data[4])

# data_np = np.array(data)
# print(data_np.shape)