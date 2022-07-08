You'll need a PostgreSql server running to run these tests (they are integration tests).

```bash
docker run --name postgres -p 5432:5432 -e POSTGRES_PASSWORD=something -d postgres:latest
```

After you server is running, create a database called _junk_.  I use Bee Keeper Studio.  Remember to reference your server as _host.docker.internal_.