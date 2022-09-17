set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    create user secretary with password 'masterkey';
    create database secretary;
    grant all privileges on database secretary to secretary;
EOSQL