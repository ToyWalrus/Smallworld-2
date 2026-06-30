# Watches Project/Scripts for changes and rebuilds Smallworld.dll into Unity/Assets/Plugins/Smallworld/
Set-Location "$PSScriptRoot/Project"
dotnet watch build -c Release Smallworld.csproj
