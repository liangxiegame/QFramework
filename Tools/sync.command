baseDirForScriptSelf=$(cd "$(dirname "$0")"; pwd)
cd ${baseDirForScriptSelf}/

rm -rf ../UnityExample/Assets/QFramework/Core/CSharp/Libs
cp -rf ../QFramework/Core/CSharp/Libs ../UnityExample/Assets/QFramework/Core/CSharp/Libs
