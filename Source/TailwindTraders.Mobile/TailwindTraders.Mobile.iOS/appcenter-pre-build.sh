#!/usr/bin/env bash

set -v

PROJECTPATH="$APPCENTER_SOURCE_DIRECTORY/Source/TailwindTraders.Mobile/TailwindTraders.Mobile.iOS"

echo "Environment variables:"
printenv

echo "Pre Info.plist content:"
cat $PROJECTPATH/Info.plist

if ! [[ -z ${TAIL_APPID+x} ]]; then
	/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier $TAIL_APPID" $PROJECTPATH/Info.plist
fi

echo "Post Info.plist content:"
cat $PROJECTPATH/Info.plist
