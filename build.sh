#!/bin/bash

# Install .NET SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0

# Add .NET to PATH
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$DOTNET_ROOT:$PATH

# Build the project
dotnet publish MiniGameCollection.Web -c Release -o output
