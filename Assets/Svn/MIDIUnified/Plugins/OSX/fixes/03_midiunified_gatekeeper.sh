#!/bin/bash

cd ..

function fix(){

	xattr -r -d com.apple.quarantine *

	for f in *;
	do
		if [[ "$f" == *dylib ]]
		then
	  		lib=$(xattr -p com.apple.quarantine $f)
			lib="00c1${lib:4}"
			echo $lib
			xattr -w com.apple.quarantine "$lib"
		fi	
	done

}

fix

cd BASS24/Libraries

fix

