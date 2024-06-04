#if DEBUG
var builder = WebApplication.CreateBuilder(args);
#else
var options = new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = "wwwroot/spa",
    //WebRootPath = "\\",
};
var builder = WebApplication.CreateBuilder(options);
#endif

builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles(conf =>
{
    conf.RootPath = "wwwroot/spa";
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.UseSpa(spa =>
{
    //spa.Options.SourcePath = "wwwroot/spa";
    spa.Options.DefaultPage = "/index.html";
});

app.MapFallbackToFile("index.html");

app.Run();
