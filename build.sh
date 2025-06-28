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

# Define the project path
PROJECT_DIR="PrimeGeneratorApp"
PROJECT_FILE="$PROJECT_DIR/PrimeGeneratorApp.csproj"

if [ ! -f "$PROJECT_FILE" ]; then
    echo "Error: Project file not found at $PROJECT_FILE"
    exit 1
fi

echo "Publishing application for RID: $RID..."
dotnet publish "$PROJECT_FILE" -r "$RID" -c Release --self-contained false

# Get the expected output directory
OUTPUT_DIR="$PROJECT_DIR/bin/Release/net8.0/$RID/publish/"

echo "Build complete. Output located in: $OUTPUT_DIR"
echo "To run the application:"
echo "cd $OUTPUT_DIR && ./PrimeGeneratorApp"
