#!/bin/bash -xe
# the directory of the script
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd "$DIR"

# GIT_COMMIT=$(git rev-parse --short HEAD)
# now="$(date +'%y%m%d-%H%M%S')"

# the temp directory used, within $DIR
WORK_DIR=$(mktemp -d)

# check if tmp dir was created
if [[ ! "$WORK_DIR" || ! -d "$WORK_DIR" ]]; then
  echo "Could not create temp dir"
  exit 1
fi

# deletes the temp directory
function cleanup {      
  rm -rf "$WORK_DIR"
  echo "Deleted temp working directory $WORK_DIR"
}

# register the cleanup function to be called on the EXIT signal
trap cleanup EXIT


mkdir -p "builds"

(
    cd Plugin/Robusta
    npm pack
)


(
    cd Plugin/Robusta.Facebook
    npm pack
)

cp Plugin/Robusta/*.tgz "$WORK_DIR"
cp Plugin/Robusta.Facebook/*.tgz "$WORK_DIR"
cp builds/readme.png "$WORK_DIR"
(
    cd  "$WORK_DIR"
    zip -r robusta.zip .
    mv robusta.zip "$DIR/builds"
)


mv Plugin/Robusta/*.tgz builds/
mv Plugin/Robusta.Facebook/*.tgz builds/