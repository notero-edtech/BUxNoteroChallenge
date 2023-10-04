#!/bin/bash



rm ../BASS24/Libraries/bass.dylib
rm ../BASS24/Libraries/bass_fx.dylib
rm ../BASS24/Libraries/bassmidi.dylib
rm ../BASS24/Libraries/bassmix.dylib
rm ../BASS24/Libraries/bassflac.dylib
rm ../BASS24/Libraries/bassopus.dylib
rm ../BASS24/Libraries/basswv.dylib
rm ../BASS24/Libraries/bassape.dylib
rm ../BASS24/Libraries/bass_mpc.dylib
rm ../BASS24/Libraries/bassenc.dylib
rm ../BASS24/Libraries/bassenc_flac.dylib
rm ../BASS24/Libraries/bassenc_mp3.dylib
rm ../BASS24/Libraries/bassenc_ogg.dylib
rm ../BASS24/Libraries/bassenc_opus.dylib






sudo lipo ../BASS24/Downloads/libbass.dylib -remove i386 -output ../BASS24/Libraries/bass.dylib
sudo lipo ../BASS24/Downloads/libbass_fx.dylib -remove i386 -output ../BASS24/Libraries/bass_fx.dylib
sudo lipo ../BASS24/Downloads/libbassmidi.dylib -remove i386 -output ../BASS24/Libraries/bassmidi.dylib
sudo lipo ../BASS24/Downloads/libbassmix.dylib -remove i386 -output ../BASS24/Libraries/bassmix.dylib
sudo lipo ../BASS24/Downloads/libbassflac.dylib -remove i386 -output ../BASS24/Libraries/bassflac.dylib
sudo lipo ../BASS24/Downloads/libbassopus.dylib -remove i386 -output ../BASS24/Libraries/bassopus.dylib
sudo lipo ../BASS24/Downloads/libbasswv.dylib -remove i386 -output ../BASS24/Libraries/basswv.dylib
sudo lipo ../BASS24/Downloads/libbassape.dylib -remove i386 -output ../BASS24/Libraries/bassape.dylib
sudo lipo ../BASS24/Downloads/libbass_mpc.dylib -remove i386 -output ../BASS24/Libraries/bass_mpc.dylib
sudo lipo ../BASS24/Downloads/libbassenc.dylib -remove i386 -output ../BASS24/Libraries/bassenc.dylib
sudo lipo ../BASS24/Downloads/libbassenc_flac.dylib -remove i386 -output ../BASS24/Libraries/bassenc_flac.dylib
sudo lipo ../BASS24/Downloads/libbassenc_mp3.dylib -remove i386 -output ../BASS24/Libraries/bassenc_mp3.dylib
sudo lipo ../BASS24/Downloads/libbassenc_ogg.dylib -remove i386 -output ../BASS24/Libraries/bassenc_ogg.dylib
sudo lipo ../BASS24/Downloads/libbassenc_opus.dylib -remove i386 -output ../BASS24/Libraries/bassenc_opus.dylib
