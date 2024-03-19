using Microsoft.AspNetCore.Mvc.Versioning;
using NLog;
using NLog.Web;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Controllers;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("init main");

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<HttpResponseExceptionFilter>();
        options.Filters.Add<Filter>();
    });
    builder.Services.AddMvc(options =>
    {
    });
    builder.Services.AddApiVersioning(options =>
    {
        options.ApiVersionReader = new MediaTypeApiVersionReader("version");
    })
    .AddMvc();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    //other classes that need the logger 
    //builder.Services.AddTransient<ShipmentController>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // http://surferonwww.info/BlogEngine/post/2021/04/15/how-to-read-posted-body-from-httprequest-body.aspx
    app.Use(async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    });

    //app.UseMiddleware<IOMiddleware>(); //’Ç‹L

    await app.RunAsync();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
