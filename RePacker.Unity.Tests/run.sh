red="$bold$(tput setaf 1)"
normal=$'\e[0m'

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

./setup.sh

current_dir=`pwd`
echo "Running tests with: $unity_path"

echo "${red}"
status=`"$unity_path" -runTests -batchmode -projectPath . -testPlatform PlayMode -testResults $current_dir/TestResults/results.xml` > /dev/stderr
echo "${normal}"

if [ -z $status ]; then
    echo "Processing test results..."
    ./process_results.sh
else
    echo ""
fi
