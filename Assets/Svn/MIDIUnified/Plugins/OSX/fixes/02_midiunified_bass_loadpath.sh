#!/bin/bash

cd ../BASS24/Libraries

sudo install_name_tool -id "@loader_path/bass.dylib" bass.dylib

sudo install_name_tool -id "@loader_path/bass_fx.dylib" bass_fx.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bass_fx.dylib

sudo install_name_tool -id "@loader_path/bassmidi.dylib" bassmidi.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassmidi.dylib

sudo install_name_tool -id "@loader_path/libbassmix.dylib" bassmix.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassmix.dylib

sudo install_name_tool -id "@loader_path/libbassflac.dylib" bassflac.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassflac.dylib

sudo install_name_tool -id "@loader_path/libbassopus.dylib" bassopus.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassopus.dylib

sudo install_name_tool -id "@loader_path/libbasswv.dylib" basswv.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" basswv.dylib

sudo install_name_tool -id "@loader_path/libbassape.dylib" bassape.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassape.dylib

sudo install_name_tool -id "@loader_path/libbass_mpc.dylib" bass_mpc.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bass_mpc.dylib

sudo install_name_tool -id "@loader_path/bassenc.dylib" bassenc.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassenc.dylib

sudo install_name_tool -id "@loader_path/bassenc_flac.dylib" bassenc_flac.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassenc_flac.dylib

sudo install_name_tool -id "@loader_path/bassenc_mp3.dylib" bassenc_mp3.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassenc_mp3.dylib

sudo install_name_tool -id "@loader_path/bassenc_ogg.dylib" bassenc_ogg.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassenc_ogg.dylib

sudo install_name_tool -id "@loader_path/bassenc_opus.dylib" bassenc_opus.dylib
sudo install_name_tool -change "@loader_path/libbass.dylib" "@loader_path/bass.dylib" bassenc_opus.dylib