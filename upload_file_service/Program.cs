using upload_file_service;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Upload file client service";
});

builder.Services.AddHostedService<Worker>();
var host = builder.Build();

host.Run();
