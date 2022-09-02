#!/bin/sh

pathDB=?
pathBackup=?

cp $pathDB/database.sqlite $pathBackup/database-$(date "+%Y-%m-%dT%H:%M").sqlite
