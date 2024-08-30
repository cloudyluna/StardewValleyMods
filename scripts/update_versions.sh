#!/usr/bin/env bash

if [[ -z "${1}" ]];then
    echo "Need version"
    exit 1
fi

jq --arg VERSION "${1}" '.Version=$VERSION' manifest.json  \
    | sponge manifest.json

sed -i -E "s/^\\\def\\\projectVersion.*$/\\\def\\\projectVersion\{$1\}/" docs/README.tex

exit