#!/bin/bash

# Exit immediately if a command exits with a non-zero status.
set -e

# Determine the Runtime Identifier (RID)
echo "Detecting Runtime Identifier (RID)..."
RID=$(dotnet --info | grep "RID:" | awk '{print $2}')

if [ -z "$RID" ]; then
    echo "Error: Could not determine RID."
    exit 1
fi

echo "Detected RID: $RID"

# Define the project and expected output directory
PROJECT_DIR="PrimeGeneratorApp"
PUBLISH_DIR="$PROJECT_DIR/bin/Release/net8.0/$RID/publish/"

if [ ! -d "$PUBLISH_DIR" ]; then
    echo "Error: Publish directory not found at $PUBLISH_DIR"
    echo "Please run the build script (./build.sh) first."
    exit 1
fi

echo "Listing files in $PUBLISH_DIR recursively:"
ls -R "$PUBLISH_DIR"
