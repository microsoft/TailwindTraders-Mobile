Instructions to build libtfliteextern for iOS in OSX:

Prerequisites:
xcode

-From current dir execute the following commands:

git clone https://github.com/emgucv/tensorflow.git
pushd tensorflow
git checkout v1.12.0-emgu
popd
chmod +x build_tf_lite_ios.sh
./build_tf_lite_ios.sh