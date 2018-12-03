#!/usr/bin/env bash
#
# Taken from: https://github.com/Microsoft/appcenter-build-scripts-examples/blob/master/xamarin/nunit-test/appcenter-post-build.sh

echo "Found NUnit test projects:"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*UnitTests\.csproj' -exec echo {} \;
echo
echo "Building NUnit test projects:"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*UnitTests\.csproj' -exec msbuild {} \;
echo
echo "Compiled projects to run NUnit tests:"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*bin.*UnitTests\.dll' -exec echo {} \;
echo
echo "Running NUnit tests:"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*bin.*UnitTests\.dll' -exec nunit3-console {} \;
echo
echo "NUnit tests result:"
pathOfTestResults=$(find $APPCENTER_SOURCE_DIRECTORY -name 'TestResult.xml')
cat $pathOfTestResults
echo

#look for a failing test
grep -q 'result="Failed"' $pathOfTestResults

if [[ $? -eq 0 ]]
then 
echo "At least one test failed :-(" 
exit 1
else 
echo "All tests passed :-)" 
fi

echo "Running XamlStyler verify"
cd $APPCENTER_SOURCE_DIRECTORY/Tools/XamlStyler
./verify.sh

if [[ $? -eq 0 ]]
then 
echo "XamlStyler errors :-(" 
exit 1
else 
echo "XamlStyler is ok :-)" 
fi
