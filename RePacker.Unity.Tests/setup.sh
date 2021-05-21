original_path=`pwd`

echo "Building RePacking project..."

build_path=`realpath ../RePacker.Unity/`
cd `dirname $build_path/RePacker.Unity/`
./build.bat

echo "Copying DLLs to Unity project..."

cd `dirname $original_path/RePacker.Unity.Tests/`
cp ../RePacker.Unity/bin/Unity/net4.6.1/RePacker.dll ./Assets/RePacker/RePacker.dll
cp ../RePacker.Unity/bin/Unity/net4.6.1/RePacker.Unsafe.dll ./Assets/RePacker/RePacker.Unsafe.dll
cp ../RePacker.Unity/bin/Unity/net4.6.1/RePacker.Unity.dll ./Assets/RePacker/RePacker.Unity.dll