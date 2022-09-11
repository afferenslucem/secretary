#!/bin/sh

execPath=&

pushd $execPath
docker-compose pull bot
sleep 1m
docker-compose up -d bot
sleep 1m
popd
