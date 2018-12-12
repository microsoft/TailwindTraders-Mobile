#!/usr/bin/env bash

########################################################################################################################
# XAML policies
########################################################################################################################

echo
echo "Verifying XAML policies:"
cd $APPCENTER_SOURCE_DIRECTORY/Source/Tools/XamlStyler
./verify.sh

if [[ $? -eq 0 ]]
then 
echo "Not every XAML follows defined policies :-(" 
exit 1
else 
echo "Every XAML follows defined policies :-)" 
fi