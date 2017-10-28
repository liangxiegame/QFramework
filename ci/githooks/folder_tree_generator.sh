#!bin/sh

# /usr/local/bin/tree -I 'mkdocs.*|*.meta|docs|ci|README.md|dist|*.mp3|*.jpg|QFramework.sln|*.userprefs|*.csproj|Properties|*.txt|*.sh' > tree.md
cd QFramework
/usr/local/bin/md-file-tree > ../docs/folder_tree.md