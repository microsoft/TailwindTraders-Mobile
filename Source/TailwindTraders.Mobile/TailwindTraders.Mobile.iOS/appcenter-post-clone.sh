#!/usr/bin/env bash

########################################################################################################################
# XAML policies
########################################################################################################################

echo "Verifying XAML policies:"
echo
cd $APPCENTER_SOURCE_DIRECTORY/Source/Tools/XamlStyler
./verify.sh

if [[ $? -eq 0 ]]
then 
echo
echo "Every XAML follows defined policies :-)" 
else
echo 
echo "Not every XAML follows defined policies :-(" 
exit 1
fi