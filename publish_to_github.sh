#!/bin/bash -xe
ROBUSTA_VERSION=$(jq -r ".version" < Plugin/Robusta/package.json)

git checkout github-prod-master || git checkout -b github-prod-master github-prod/master

git checkout master .

git commit -m "Update to v${ROBUSTA_VERSION}"

git tag "v${ROBUSTA_VERSION}"

git push --tags github-prod github-prod-master:master

git push --tags origin github-prod-master
