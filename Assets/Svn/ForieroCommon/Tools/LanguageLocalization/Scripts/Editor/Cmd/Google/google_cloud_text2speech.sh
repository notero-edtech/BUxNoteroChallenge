#!/bin/bash

d=$(dirname "$0")
s="${d}/google_cloud_credentials.sh"
source "$s"

languageCode=$3
name=$4
ssmlGender=$5
audioEncoding="MP3"
text=$6
filename=$7
tmpPath=$8
destPath=$9

filename_txt="$tmpPath/$filename.txt"
filename_out_1="$tmpPath/$filename.out.1.txt"
filename_out_2="$tmpPath/$filename.out.2.txt"
filename_out_3="$tmpPath/$filename.out.3.mp3"
filename_audio="$destPath/$filename.ogg"

curl -H "Authorization: Bearer $(gcloud auth application-default print-access-token)" \
  -H "Content-Type: application/json; charset=utf-8" \
  --data "{
    'input':{
      'text':'$text'
    },
    'voice':{
      'languageCode':'$languageCode',
      'name':'$name'
    },
    'audioConfig':{
      'audioEncoding':'$audioEncoding'
    }
  }" "https://texttospeech.googleapis.com/v1/text:synthesize" > "$filename_txt"

sed 's|audioContent| |' < "$filename_txt" > "$filename_out_1"
tr -d '\n ":{}' < "$filename_out_1" > "$filename_out_2"
base64 "$filename_out_2" --decode > "$filename_out_3"
#/opt/local/bin/ffmpeg -i "$filename_out_3" -c:a libvorbis -q:a 4 "$filename_audio"
/opt/local/bin/sox "$filename_out_3" "$filename_audio"
rm "$filename_txt"
rm "$filename_out_1"
rm "$filename_out_2"
rm "$filename_out_3"
