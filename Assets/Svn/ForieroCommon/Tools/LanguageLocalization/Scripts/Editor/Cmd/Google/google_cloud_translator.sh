#!/bin/bash

d=$(dirname "$0")
s="${d}/google_cloud_credentials.sh"
source "$s"

langFrom=$3
langTo=$4
text=$5

curl -H "Authorization: Bearer $(gcloud auth application-default print-access-token)" \
  -H "Content-Type: application/json; charset=utf-8" \
  --data "{
    'q': '$text',
    'source': '$langFrom',
    'target': '$langTo',
    'format': 'text'
  }" "https://translation.googleapis.com/language/translate/v2"
