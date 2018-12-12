SOLUTIONNAME="TailwindTraders.Mobile"

if [[ ("$(uname)" == "Darwin") || ("$(expr substr $(uname -s) 1 5)" == "Linux") ]]; then
    RUNTIME="mono"
fi

pushd ../../$SOLUTIONNAME/

$RUNTIME ../Tools/XamlStyler/XamlStyler.Console/xstyler.exe -c XamlStylerSettings.json -d . -r true -v

popd