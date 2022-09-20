#!/bin/sh

execPath=&

pushd $execPath
/usr/local/bin/docker-compose pull bot panel-backend panel-frontend
/usr/local/bin/docker-compose up -d bot panel-backend panel-frontend
popd
