import platform
import re
import os
import subprocess
import shlex

isMac = re.match("[Ww]indows", platform.system()) == None
exe_path = "../build/ExcelToJson.exe"
cmd = "{} -e excels -j jsons -c cs -t ../ScriptTemplate.txt".format(exe_path)
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