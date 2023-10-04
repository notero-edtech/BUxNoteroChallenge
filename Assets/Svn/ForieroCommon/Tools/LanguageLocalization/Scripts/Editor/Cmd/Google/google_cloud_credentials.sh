#!/bin/bash

# jsonVar="/Volumes/CalDigitT4R4TB/Development/Google Cloud/musicuso-72c562453bc6.json"
# sdkVar="/Volumes/CalDigitT4R4TB/Development/Google Cloud/google-cloud-sdk"

jsonVar="$1"
sdkVar="$2"

pathBashInc="${sdkVar}/path.bash.inc"
completionBashInc="${sdkVar}/completion.bash.inc"

export GOOGLE_APPLICATION_CREDENTIALS="${jsonVar}"

# The next line updates PATH for the Google Cloud SDK.
if [ -f "$pathBashInc" ]; then . "$pathBashInc"; fi

# The next line enables shell command completion for gcloud.
if [ -f "$completionBashInc" ]; then . "$completionBashInc"; fi
