#!/usr/bin/env bash

########################################################################################################################
# Unit Tests
########################################################################################################################

UNIT_TESTS_PROJECT_PATH="Source/TailwindTraders.Mobile/UnitTests/UnitTests.csproj"

dotnet test $APPCENTER_SOURCE_DIRECTORY/$UNIT_TESTS_PROJECT_PATH -c Release