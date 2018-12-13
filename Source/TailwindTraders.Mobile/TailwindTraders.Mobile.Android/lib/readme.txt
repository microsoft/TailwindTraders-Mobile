To build libtfliteextern for Android:

Ubuntu:

-clone https://github.com/emgucv/tensorflow/tree/v1.12.0-emgu in this directory
-install bazel from apt
-in "tensorflow/contrib/lite/kernels/internal/BUILD" remove "//tensorflow:android_arm64": ["-mfloat-abi=softfp"],
-run build_tf_lite_android