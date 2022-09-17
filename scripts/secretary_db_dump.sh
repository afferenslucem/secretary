#!/bin/sh
PATH=/etc:/bin:/sbin:/usr/bin:/usr/sbin:/usr/local/bin:/usr/local/sbin

PGPASSWORD=sc@ry_passwo0rd
export PGPASSWORD
pathB=/root/Yandex.Disk/backups/secretary/database/
dbUser=secretary
database=secretary

pg_dump -U $dbUser -d $database -h localhost -p 35432 | gzip > $pathB/database-$(date "+%Y-%m-%dT%H:%M").bak.gzip

unset PGPASSWORD