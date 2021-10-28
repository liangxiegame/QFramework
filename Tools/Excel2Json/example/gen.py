import platform
import re
import os
import subprocess
import shlex

isMac = re.match("[Ww]indows", platform.system()) == None
config_path = os.path.abspath("config.txt")
exe_path = ""
para = []
with open(config_path, 'r', encoding="UTF-8") as f:
        content = f.readlines()
        for line in content:
            if(line.startswith('#') or len(line) <= 0):
                continue
            match = re.match(r"(\w+)\s*=\s*(.+)",line)
            key = match.group(1)
            key = "-"+key
            value = match.group(2)
            if("path" in key):
                value = os.path.abspath(value)
            if("exe_path" in key):
                exe_path = value
                continue
            para.append(key)
            para.append(value)

cmd = "{} {} ".format(exe_path, " ".join(para))
# print(cmd)
if isMac:
    cmd = "mono " + cmd

def run_command(command):
    process = subprocess.Popen(command.split(' '), stdout=subprocess.PIPE)
    while True:
        output = process.stdout.readline()
        if process.poll() is not None:
            break
        if output:
            print (output.strip())
    rc = process.poll()
    return rc

run_command(cmd)
try:
    input("Press Enter to exit...")
except:
    pass