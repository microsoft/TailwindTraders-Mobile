#!/usr/bin/env bash

########################################################################################################################
# Unit Tests
########################################################################################################################

UNIT_TESTS_PROJECT_PATH="Source/TailwindTraders.Mobile/UnitTests/UnitTests.csproj"

dotnet test $APPCENTER_SOURCE_DIRECTORY/$UNIT_TESTS_PROJECT_PATH -c Release

#
# Compiles a Xamarin.UITest project, and runs it as a test run in AppCenter. Android version
#
################################################################################################
##### NOTE: TO RUN THE TESTS, YOU NEED TO DEFINE THE "ENABLE_UITESTS" ENVIRONMENT VARIABLE #####
################################################################################################
#
# Environment variables :
#
# - APPCENTER_TOKEN. You need an AppCenter API token. Instructions on how to get it in https://docs.microsoft.com/en-us/appcenter/api-docs/
# - APPCENTER_PROJECT_NAME. URL of App Center project. For example: AppCenterDemos/TailWindTraders
# - DEVICES. ID or IDs of devices or device sets previously created in AppCenter. Defaults to "Pixel 2" (7c5a701f)
# - ENABLE_UITESTS. Set to true if you want to run UI Tests
#
# NOTE: UI_TEST_TOOLS_DIR is where "test-cloud.exe" is. By default in AppCenter is /Users/vsts/.nuget/packages/xamarin.uitest/<xamarin uitest version>/tools

if [ -z "$ENABLE_UITESTS" ]; then
	echo "ERROR! You need to define in AppCenter the ENABLE_UITESTS environment variable. UI Tests will not run. Exiting..."
	exit 0
fi

if [ -z "$APPCENTER_TOKEN" ]; then
	echo "ERROR! AppCenter API token is not set. Exiting..."
	exit 0
fi

UITEST_PROJECT_PATH="$APPCENTER_SOURCE_DIRECTORY/Source/TailwindTraders.Mobile/TailwindTraders.UITests"
UITEST_CSPROJ_NAME="TailwindTraders.UITests.csproj"
APK_PATH="$APPCENTER_OUTPUT_DIRECTORY/com.microsoft.tailwindtraders.apk"

DEFAULT_DEVICES="7c5a701f"
UI_TEST_TOOLS_DIR="/Users/vsts/.nuget/packages/xamarin.uitest/2.2.7/tools"

if [ -z "$APPCENTER_TOKEN" ]; then
	echo "ERROR! AppCenter API token is not set. Exiting..."
	exit 1
fi

if [ -z "$DEVICES" ]; then
	echo "WARNING! Devices variable not set. You need to previously create a device set, and specify it here, eg: <project_name>/samsunggalaxys"
	echo "Defaulting to Google Pixel 2 (7c5a701f)"
	DEVICES=$DEFAULT_DEVICES
fi

echo "UI Test .csproj path: $UITEST_CSPROJ_PATH"
echo "UI Test Tools Dir: $UI_TEST_TOOLS_DIR"

echo "### Restoring UITest NuGet packages"
msbuild $UITEST_PROJECT_PATH/$UITEST_CSPROJ_NAME /t:restore

echo "### Compiling UITest project"
msbuild $UITEST_PROJECT_PATH/$UITEST_CSPROJ_NAME /t:build /p:Configuration=Release

echo "### Launching AppCenter test run"
appcenter test run uitest --app $APPCENTER_PROJECT_NAME --devices $DEVICES --app-path $APK_PATH --test-series "master" --locale "en_US" --build-dir $UITEST_PROJECT_PATH/bin/Release --uitest-tools-dir $UI_TEST_TOOLS_DIR --token $APPCENTER_TOKEN --async

