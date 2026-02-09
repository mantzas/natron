#!/bin/sh

dotnet watch --project tests/Natron.Tests/Natron.Tests.csproj test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info /p:Exclude="[xunit.runner.*]*"
