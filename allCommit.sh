#!/bin/bash

git filter-branch --msg-filter '
if [ "$GIT_COMMIT" = "commit_hash_to_ignore" ]; then
    echo "���������� ���������� ������������"
else
    cat
fi' -- --all
