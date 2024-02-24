#!/bin/bash

git filter-branch --msg-filter '
if [ "$GIT_COMMIT" = "commit_hash_to_ignore" ]; then
    echo "Глобальное обновление комментариев"
else
    cat
fi' -- --all
