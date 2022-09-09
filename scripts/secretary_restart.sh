#!/bin/sh

execPath=&

pushd $execPath
docker-compose pull bot
sleep 1m
docker-compose up -d --force-recreate bot
sleep 1m
popd
