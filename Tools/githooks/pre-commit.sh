#!/bin/sh

sh ci/githooks/folder_tree_generator.sh
/usr/local/bin/mkdocs gh-deploy --clean
rm -rf site
