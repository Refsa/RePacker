normal=$'\e[0m'
red="$bold$(tput setaf 1)"
green="$bold$(tput setaf 2)"

echoerr() { cat <<< "$@" 1>&2; }

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
            echo "Test Case $classname::$methodname -> ${red}Failed${normal}" > /dev/stderr
        else
            echo "Test Case $classname::$methodname -> ${green}Passed${normal}"
        fi
    fi
}

while read_xml; do
    parse_xml
done < ./TestResults/results.xml

#grep 'test-case.' ./TestResults/results.xml