#!/usr/bin/env bash

########################################################################################################################
# Unit Tests
# 
# (Based on:
# https://github.com/Microsoft/appcenter-build-scripts-examples/blob/master/xamarin/nunit-test/appcenter-post-build.sh)
########################################################################################################################

PROJECTNAME="UnitTests"

echo "Building projects matching \"${PROJECTNAME}\":"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*'"$PROJECTNAME"'\.csproj' -exec msbuild {} /p:Configuration=Release \;
echo
echo "Running tests matching \"${PROJECTNAME}\":"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*bin.*'"$PROJECTNAME"'\.dll' -exec nunit3-console {} \;
echo
echo "Result:"
pathOfTestResults=$(find $APPCENTER_SOURCE_DIRECTORY -name 'TestResult.xml')
cat $pathOfTestResults
echo

grep -q 'result="Failed"' $pathOfTestResults

if [[ $? -eq 0 ]]
then 
echo "At least one test failed :-(" 
exit 1
else 
echo "All tests passed :-)" 
fi

########################################################################################################################
# XAML policies
########################################################################################################################

echo "Verifying XAML policies:"
cd $APPCENTER_SOURCE_DIRECTORY/Tools/XamlStyler
./verify.sh

if [[ $? -eq 0 ]]
then 
echo "Not every XAML follows defined policies :-(" 
exit 1
else 
echo "Every XAML follows defined policies :-)" 
fi