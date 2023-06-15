using BenchmarkDotNet.Running;
using ExecutionBenchmark.Tests;
using Environment = ExecutionBenchmark.Services.Environment;

//docker run --name rh-postgres -e POSTGRES_PASSWORD=Random123! -p 5430:5432 -d postgres
//docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Random123!" -p 1436:1433 --name rh-mssql --hostname rh-mssql-host -d mcr.microsoft.com/mssql/server:2022-latest
//docker run -d --hostname rh-host --name rh-rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management
//docker run --hostname=3a5091ed267c --mac-address=02:42:ac:11:00:03 --env=PATH=/opt/rabbitmq/sbin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin --env=RABBITMQ_DATA_DIR=/var/lib/rabbitmq --env=RABBITMQ_VERSION=3.11.14 --env=RABBITMQ_PGP_KEY_ID=0x0A9AF2115F4687BD29803A206B73A36E6026DFCA --env=RABBITMQ_HOME=/opt/rabbitmq --env=HOME=/var/lib/rabbitmq --env=LANG=C.UTF-8 --env=LANGUAGE=C.UTF-8 --env=LC_ALL=C.UTF-8 --volume=/var/lib/rabbitmq -p 15672:15672 -p 5672:5672 --restart=no --label='org.opencontainers.image.ref.name=ubuntu' --label='org.opencontainers.image.version=20.04' --runtime=runc -d rabbitmq:3-management

var environment = Environment.Local;

await BasicTest.RabbitMq(environment);
await BasicTest.PostgresRawSql(environment);
// await BasicTest.DannyRabbitMq();
// await BasicTest.DannyPostgre();
// await BasicTest.RhPostgre();`

var summary = BenchmarkRunner.Run<BenchmarkTests>();