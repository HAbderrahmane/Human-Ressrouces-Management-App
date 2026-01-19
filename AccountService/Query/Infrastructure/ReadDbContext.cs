using MongoDB.Driver;
using AccountService.Query.Domain;
using Microsoft.Extensions.Configuration;

namespace AccountService.Query.Infrastructure;

public class ReadDbContext
{
    private readonly IMongoDatabase _database;

    public ReadDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReadDatabase")
            ?? "mongodb://root:root@localhost:27017/admin?authSource=admin";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("account_read");
    }

    public IMongoCollection<AccountReadModel> Accounts =>
        _database.GetCollection<AccountReadModel>("accounts");
}
