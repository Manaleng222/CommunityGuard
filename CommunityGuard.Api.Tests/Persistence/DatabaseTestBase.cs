using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CommunityGuard.Api.Tests.Persistence;

// Base class: gives every test its own fresh in-memory database.
public abstract class DatabaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    protected DatabaseTestBase()
    {
        // An open connection keeps the in-memory database alive.[reference:13]
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema once per test class.
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    protected AppDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();   // Closes and wipes the DB.
}
