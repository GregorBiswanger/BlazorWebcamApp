using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.StartAsync();

await Task.Run(async () =>
{
    var browserWindowOptions = new BrowserWindowOptions
    {
        Height = 730,
        Width = 1000,
        Show = false
    };
    var mainWindow = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
    mainWindow.OnReadyToShow += () => mainWindow.Show();
});

await app.WaitForShutdownAsync();
