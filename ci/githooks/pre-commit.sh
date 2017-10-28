#!/bin/sh

/user/local/bin/mkdocs gh-deploy --clean
rm -rf site
open ./
