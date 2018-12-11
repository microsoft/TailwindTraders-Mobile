if [[ ("$(uname)" == "Darwin" ) || ( "$(expr substr $(uname -s) 1 5)" == "Linux") ]];  then
    RUNTIME="mono"
fi

pushd ../../TailwindTraders.Mobile/

$RUNTIME ../Tools/XamlStyler/XamlStyler.Console/xstyler.exe -c XamlStylerSettings.json -d . -r true -v

popd