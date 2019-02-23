#!/bin/sh

dotnet watch --project tests/Natron.Tests.Unit/Natron.Tests.Unit.csproj test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info /p:Exclude="[xunit.runner.*]*"
