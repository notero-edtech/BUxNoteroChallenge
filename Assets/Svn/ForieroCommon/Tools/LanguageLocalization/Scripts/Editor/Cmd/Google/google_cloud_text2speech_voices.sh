#!/bin/bash

d=$(dirname "$0")

jsonVar="$1"
sdkVar="$2"
gcloud="$2/bin/gcloud"

s="${d}/google_cloud_credentials.sh"
source "$s"

curl -H "Authorization: Bearer "$($gcloud auth application-default print-access-token) \
    -H "Content-Type: application/json; charset=utf-8" \
    "https://texttospeech.googleapis.com/v1/voices"
