#!/usr/bin/env bash

########################################################################################################################
# Unit Tests
########################################################################################################################

UNIT_TESTS_PROJECT_PATH="Source/TailwindTraders.Mobile/UnitTests/UnitTests.csproj"

dotnet test $APPCENTER_SOURCE_DIRECTORY/$UNIT_TESTS_PROJECT_PATH -c Release

#
# Compiles a Xamarin.UITest project, and runs it as a test run in AppCenter. iOS version
#
################################################################################################
##### NOTE: TO RUN THE TESTS, YOU NEED TO DEFINE THE "ENABLE_UITESTS" ENVIRONMENT VARIABLE #####
################################################################################################
#
# Environment variables :
#
# - APPCENTER_TOKEN. You need an AppCenter API token. Instructions on how to get it in https://docs.microsoft.com/en-us/appcenter/api-docs/
# - APPCENTER_PROJECT_NAME. URL of App Center project. For example: AppCenterDemos/TailWindTraders
# - DEVICES. ID or IDs of devices or device sets previously created in AppCenter. Defaults to "iPhone 8, iOS 12.1" (de95e76a)
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

# DEFAULTS

DEFAULT_XAMARIN_UITEST_VERSION="2.2.7"
DEFAULT_DEVICES="9a697840"

if [ -z "$DEVICES" ]; then
	echo "WARNING! 'DEVICES' variable not set. You need to previously create a device set, and specify it here, eg: <project_name>/iPhonesWithNotch"
	echo "Defaulting to iPhone 8, iOS 12.1 (de95e76a)"
	DEVICES=$DEFAULT_DEVICES
fi

# INSTALL XAMARIN UITEST TOOLS

if [ -z "$XAMARIN_UITEST_VERSION" ]; then
	XAMARIN_UITEST_VERSION=$DEFAULT_XAMARIN_UITEST_VERSION
	echo "WARNING! XAMARIN_UITEST_VERSION environment variable not set. Setting it to its default. Check the version of Xamarin.UITest you are using in your project"
fi

echo "Xamarin UITest version set to $XAMARIN_UITEST_VERSION"

echo "### Install Xamarin.UITest tools..."
nuget install Xamarin.UITest -ExcludeVersion -Version $XAMARIN_UITEST_VERSION

UI_TEST_TOOLS_DIR="Xamarin.UITest/tools"

# UI TEST PROJECT BUILD

UITestProject=$(find "$APPCENTER_SOURCE_DIRECTORY" -name TailwindTraders.UITests.csproj)
echo UITestProject: $UITestProject

echo "#### Starting UI Test project build..."
msbuild $UITestProject /t:build /p:Configuration=Release

echo "#### Starting test run..."
appcenter test run uitest --app $APPCENTER_PROJECT_NAME --devices $DEVICES --app-path $APPCENTER_OUTPUT_DIRECTORY/TailwindTraders.Mobile.iOS.ipa  --test-series "master" --locale "en_US" --build-dir $APPCENTER_SOURCE_DIRECTORY/Source/TailwindTraders.Mobile/TailwindTraders.UITests/bin/Release --uitest-tools-dir $UI_TEST_TOOLS_DIR --token $APPCENTER_TOKEN --async
