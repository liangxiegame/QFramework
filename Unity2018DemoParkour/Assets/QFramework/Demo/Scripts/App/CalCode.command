baseDirForScriptSelf=$(cd "$(dirname "$0")"; pwd)
cd ${baseDirForScriptSelf}/

find ./ -name "*.m" -or -name "*.cs" -or -name "*.h" -or -name "*.cpp" -or -name "*.lua" |xargs grep -v "^$"|wc -l
