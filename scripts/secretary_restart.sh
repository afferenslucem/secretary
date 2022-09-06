#!/bin/sh

execPath=&

pushd $execPath
docker-compose pull bot
sleep 5m
docker-compose up -d --force-recreate bot
popd
