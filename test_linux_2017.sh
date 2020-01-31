if [ -z "${WORKSPACE}" ]; then WORKSPACE=$(PWD); fi

/Applications/Unity/Hub/Editor/2017.4.34c1/Unity.app/Contents/MacOS/Unity -runTests -batchmode -projectPath $WORKSPACE -testResults ./results.xml

EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
      cat ./results.xml
        exit $EXIT_CODE
fi

