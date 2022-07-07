if(Test-Path -Path ".\TestResults" )
{
	Remove-Item -r ".\TestResults"
}
dotnet test --collect:"Xplat Code Coverage"
reportgenerator -reports:".\TestResults\**\coverage.cobertura.xml" -targetDir:".\TestReports" -reporttypes:Html