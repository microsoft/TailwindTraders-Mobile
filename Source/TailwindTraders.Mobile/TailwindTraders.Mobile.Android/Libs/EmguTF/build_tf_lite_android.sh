#!/bin/bash

pushd tensorflow/tensorflow

bazel build -c opt \
      --cpu=$1\
      --cxxopt='--std=c++11' \
      --crosstool_top=external:android/crosstool \
      --host_crosstool_top=@bazel_tools//tools/cpp:toolchain \
      //tensorflow/tfliteextern:libtfliteextern.so
popd

cp tensorflow/bazel-bin/tensorflow/tfliteextern/*.so $1
