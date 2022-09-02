#!/bin/sh

execPath=&

pushd $execPath
docker-compose pull bot
docker-compose up -d --force-recreate bot
popd
