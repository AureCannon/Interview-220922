using Ct.Interview.Web.Api.Extensions;
using Ct.Interview.Web.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args)
                            .ConfigureApplication();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
