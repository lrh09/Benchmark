# ExecutionBenchmark

## Postgres
`docker run --name rh-postgres -e POSTGRES_PASSWORD=Random123! -p 5430:5432 -d postgres`

## RH-Rabbit-MQ
`docker run -d --hostname rh-host --name rh-rabbit-server -p 8080:15672 -p 5678:5672 rabbitmq:3-management`

## Danny-Rabbit-MQ
`docker run --hostname=3a5091ed267c --mac-address=02:42:ac:11:00:03 --env=PATH=/opt/rabbitmq/sbin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin --env=RABBITMQ_DATA_DIR=/var/lib/rabbitmq --env=RABBITMQ_VERSION=3.11.14 --env=RABBITMQ_PGP_KEY_ID=0x0A9AF2115F4687BD29803A206B73A36E6026DFCA --env=RABBITMQ_HOME=/opt/rabbitmq --env=HOME=/var/lib/rabbitmq --env=LANG=C.UTF-8 --env=LANGUAGE=C.UTF-8 --env=LC_ALL=C.UTF-8 --volume=/var/lib/rabbitmq -p 15672:15672 -p 5672:5672 --restart=no --label='org.opencontainers.image.ref.name=ubuntu' --label='org.opencontainers.image.version=20.04' --runtime=runc -d rabbitmq:3-management`