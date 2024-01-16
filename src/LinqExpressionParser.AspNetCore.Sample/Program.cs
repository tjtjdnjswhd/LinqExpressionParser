using LinqExpressionParser.AspNetCore.Authorization.Extensions;
using LinqExpressionParser.AspNetCore.Extensions;
using LinqExpressionParser.AspNetCore.Results;
using LinqExpressionParser.AspNetCore.Sample.Data;
using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using System.Linq.Expressions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SearchSampleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SearchSampleContext") ?? throw new InvalidOperationException("Connection string 'SearchSampleContext' not found.")));

//builder.Services.AddExpressionParse();
builder.Services.AddExpressionParse(options =>
{
    MethodMapOptions methodMapOptions = MethodMapOptions.Default;
    methodMapOptions.MethodMap.Add("UserName", new List<MethodMapOptions.GetMethodCallExpressionDelegate> { SubstringUserNameOrNull });
    methodMapOptions.MethodMap.Remove("Substring");

    options.MethodMapOptions = methodMapOptions;
    options.OperatorMapOptions = OperatorMapOptions.Default;
});

builder.Services.AddExpressionAuthorization(
    options =>
    {
        options.PermissionFinder = user => (user.Identity?.IsAuthenticated ?? false) ? new List<string>() { "User", "Global", "UserId" } : new List<string>() { "Global", "User" };
        options.PermissionComparsion = StringComparison.OrdinalIgnoreCase;
    },
    pb =>
    {
        pb.Entity<User>("User").Property(u => u.Email);
        pb.Global("Global");
    }
);
builder.Services.AddControllers();

//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType(typeof(ValueParseResult<,>), () => new OpenApiSchema() { Type = "string" });
    options.MapType(typeof(SelectorParseResult<>), () => new OpenApiSchema() { Type = "string" });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.Events.OnRedirectToAccessDenied = (context) =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };

    o.Events.OnRedirectToLogin = (context) =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


static MethodCallExpression? SubstringUserNameOrNull(List<Expression> args) => args switch
{
    [Expression user, Expression startIndex] when user.Type == typeof(User) && startIndex.Type == typeof(int) => Expression.Call(Expression.Property(user, nameof(User.Name)), typeof(string).GetMethod(nameof(string.Substring), BindingFlags.Public | BindingFlags.Instance, new[] { typeof(int) })!, startIndex),
    _ => null
};