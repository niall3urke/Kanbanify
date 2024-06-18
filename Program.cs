using Kanbanify;
using Kanbanify.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TG.Blazor.IndexedDB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ModalService>();
ConfigureDatabase(builder.Services);


await builder.Build().RunAsync();

void ConfigureDatabase(IServiceCollection services)
{
    // Create our tables first

    var users = new StoreSchema()
    {
        Name = "Users",
        PrimaryKey = new IndexSpec
        {
            Name = "id",
            KeyPath = "id",
            Unique = true
        }
    };

    var projects = new StoreSchema()
    {
        Name = "Projects",
        PrimaryKey = new IndexSpec
        {
            Name = "id",
            KeyPath = "id",
            Unique = true
        }
    };

    var stages = new StoreSchema()
    {
        Name = "Stages",
        PrimaryKey = new IndexSpec
        {
            Name = "id",
            KeyPath = "id",
            Unique = true
        }
    };

    var items = new StoreSchema()
    {
        Name = "Items",
        PrimaryKey = new IndexSpec
        {
            Name = "id",
            KeyPath = "id",
            Unique = true
        }
    };

    var tasks = new StoreSchema()
    {
        Name = "Tasks",
        PrimaryKey = new IndexSpec
        {
            Name = "id",
            KeyPath = "id",
            Unique = true
        }
    };

    services.AddIndexedDB(db =>
    {
        db.DbName = "Kanbanify";
        db.Version = 1;
        db.Stores.Add(users);
        db.Stores.Add(projects);
        db.Stores.Add(stages);
        db.Stores.Add(items);
        db.Stores.Add(tasks);
    });

    services.AddScoped<DatabaseOperations>();
}