using Microsoft.FluentUI.AspNetCore.Components;
using ERP_QNA_WebApp.Components;
using ERP_QNA_WebApp.Data;
using ERP_QNA_WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Fluent UI
builder.Services.AddFluentUIComponents();

// Configure Database Connection Factory
var databaseProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "PostgreSQL";
var connectionString = builder.Configuration.GetConnectionString(databaseProvider)
    ?? throw new InvalidOperationException($"Connection string '{databaseProvider}' not found.");

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    return databaseProvider.ToUpperInvariant() switch
    {
        "POSTGRESQL" => new PostgreSqlConnectionFactory(connectionString),
        "MSSQL" => new MsSqlConnectionFactory(connectionString),
        _ => throw new NotSupportedException($"Database provider '{databaseProvider}' is not supported. Use 'PostgreSQL' or 'MSSQL'.")
    };
});

// Add Services
builder.Services.AddScoped<QnAService>();

var app = builder.Build();

// Initialize database (create tables if not exist)
using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
    var initializer = new DatabaseInitializer(connectionFactory);
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
