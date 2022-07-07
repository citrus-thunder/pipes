#!/bin/bash
rm -rf ./TestResults &&
dotnet test --collect:"Xplat Code Coverage" &&
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetDir:"./TestReports" -reporttypes:Html