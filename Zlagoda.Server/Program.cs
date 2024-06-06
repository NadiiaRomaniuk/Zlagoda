using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zlagoda.Server.Models;

var builder = WebApplication.CreateBuilder(args);
var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("JwtOptions").Bind(jwtOptions);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

//ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
//logger.LogInformation($"PROGRAM {jwtOptions.Key} {jwtOptions.ClockSkewMinutes}");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Key))
        };
    });

builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles(conf =>
{
    conf.RootPath = "wwwroot/spa";
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    // !!! https://www.thecodebuzz.com/failed-to-determine-the-https-port-for-the-redirect/
    app.UseHttpsRedirection();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSpa(spa =>
{
    //spa.Options.SourcePath = "wwwroot/spa";
    spa.Options.DefaultPage = "/index.html";
});

app.MapFallbackToFile("index.html");

app.Run();
