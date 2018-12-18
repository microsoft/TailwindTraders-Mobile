#!/bin/bash

pushd tensorflow/tensorflow/contrib/lite/tools/make
./download_dependencies.sh
popd

./tensorflow/tensorflow/contrib/lite/tools/make/build_ios_universal_lib.sh 

cp tensorflow/tensorflow/contrib/lite/tools/make/gen/lib/libtensorflow-lite.a libtfliteextern.a
