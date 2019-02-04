#!/usr/bin/env bash

########################################################################################################################
# XAML policies
########################################################################################################################

PROJECTPATH="$APPCENTER_SOURCE_DIRECTORY/Source/TailwindTraders.Mobile"

echo "Verifying XAML policies:"

pushd $PROJECTPATH
mono ../Tools/XamlStyler/XamlStyler.Console/xstyler.exe -c ./CodeAnalysis/XamlStylerSettings.json -d . -r true -v

if [[ $? -eq 0 ]]
then 
echo
echo "Every XAML follows defined policies :-)" 
else
echo 
echo "Not every XAML follows defined policies :-(" 
exit 1
fi

popd