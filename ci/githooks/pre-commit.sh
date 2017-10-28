#!/bin/sh

/usr/local/bin/mkdocs gh-deploy --clean
rm -rf site
open ./
