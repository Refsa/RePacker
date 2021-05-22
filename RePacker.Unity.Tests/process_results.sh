normal=$'\e[0m'
red="$bold$(tput setaf 1)"
green="$bold$(tput setaf 2)"
class_color="$bold$(tput setaf 4)"
method_color="$bold$(tput setaf 6)"
method_failed_color=$bold$'\e[0;38;2;191;063;000m'

echoerr() { cat <<< "$@" 1>&2; }

while getopts i: flag
do
    case "${flag}" in
        i) results_path=${OPTARG};;
    esac
done

if [ -z "$results_path" ] 
    then
        results_path="./TestResults/results.xml"
fi

read_xml () {
    local IFS=\>
    read -d \< ENTITY CONTENT
    local ret=$?
    TAG_NAME=${ENTITY%% *}
    ATTRIBUTES=${ENTITY#* }
    return $ret
}

parse_xml () {
    if [[ $TAG_NAME = "test-case" ]]; then
        # Suppress parsing errors
        eval local $ATTRIBUTES 2> /dev/null

        if [[ $result = "Failed" ]]; then
            echo "${red}Failed${normal}: ${class_color}$classname::${red}$methodname${normal}" > /dev/stderr
        else
            echo "${green}Passed${normal}: ${class_color}$classname::${method_color}$methodname${normal}"
        fi
    fi
}

while read_xml; do
    parse_xml
done < $results_path

#grep 'test-case.' ./TestResults/results.xml