while getopts u: flag
do
    case "${flag}" in
        u) unity_path=${OPTARG};;
    esac
done

if [ -z "$unity_path" ] 
    then
        unity_path="C:\Program Files\Unity\Hub\Editor\2019.4.22f1\Editor\Unity.exe"
fi

current_dir=`pwd`
echo "Building Unity with: $unity_path"

"$unity_path" -batchmode -nographics -projectPath . -quit