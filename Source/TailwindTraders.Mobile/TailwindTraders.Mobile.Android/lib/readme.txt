Instructions to build libtfliteextern for Android in Ubuntu:

Prerequisites:
android-ndk-r12b
Bazel

-From current dir execute the following commands:

git clone https://github.com/emgucv/tensorflow.git
pushd tensorflow
git checkout v1.12.0-emgu
popd

in "tensorflow/tensorflow/contrib/lite/kernels/internal/BUILD" remove "//tensorflow:android_arm64": ["-mfloat-abi=softfp"],

chmod +x build_tf_lite_android.sh

./build_tf_lite_android.sh x86
./build_tf_lite_android.sh x86_64
./build_tf_lite_android.sh arm64-v8a
./build_tf_lite_android.sh armeabi-v7a