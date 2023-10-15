using ServiceInvocation.UI.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddDapr();
builder.Services.AddDaprClient();
builder.Services.AddHostedService<StoreOrderBackgroundWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//For Using Async Messaging
app.UseCloudEvents();
app.MapSubscribeHandler();

app.Run();
