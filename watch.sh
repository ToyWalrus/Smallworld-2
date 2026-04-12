#!/bin/bash
# Watches Project/Scripts for changes and rebuilds Smallworld.dll into Unity/Assets/Plugins/Smallworld/
cd "$(dirname "$0")/Project"
dotnet watch build -c Release
