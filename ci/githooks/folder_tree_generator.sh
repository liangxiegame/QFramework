#!bin/sh

tree -I 'mkdocs.*|*.meta|docs|ci|README.md|dist|*.mp3|*.jpg|QFramework.sln|*.userprefs|*.csproj|Properties|*.txt|*.sh' > tree.txt
open tree.txt