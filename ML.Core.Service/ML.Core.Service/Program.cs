using Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "File Upload API", Version = "v1" });


});


//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles(); // เปิดใช้งานการให้บริการไฟล์ static
app.MapControllers();

app.Run();

