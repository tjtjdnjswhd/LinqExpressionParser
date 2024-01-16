# LinqExpressionParser.AspNetCore

**LinqExpressionParser.AspNetCore** is class library project that you can use in your ASP.NET Core app to modelbind and authorize about **ExpressionParseResult**. see [LinqExpressionParser](..\LinqExpressionParser\README.md)

## Dependency

- ASP.NET Core 7.0 ~ 8.0

## Getting start

```csharp
var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddExpressionParse();
builder.Services.AddExpressionParse(options => {
    // configure ExpressionParseOptions
});

builder.Services.AddExpressionAuthorization(options => {
    // configure ExpressionAuthorizationOptions
}, pb => {
    // configure PermissionBuilder
});
```

```csharp
[Route("[Controller]")]
public class UserController : Controller
{
    private List<User> _users = new() {...}

    [HttpGet]
    public IActionResult Get([FromQuery] ValueParseResult<User, bool> predicate)
    {
        return Ok(_users.Where(predicate.GetExpression()))
    }
}
```

See [sample](..\LinqExpressionParser.AspNetCore.Sample) for more information

## Configuration

See [LinqExpressionParser](..\LinqExpressionParser.README.md)

### Authorization

```csharp
builder.Services.AddExpressionAuthorization(options => {
    // Set PermissionFinder to get permission from ClaimsPrincipal
    options.PermissionFinder = user => ...;
    // options.PermissionComparsion = ...;
    // Default comparsion is StringComparsion.OrdinalIgnoreCase
}, pb => {
    // Set global permission for every IExpressionParseResult<Expression>
    pb.Global("Global0", "Global1");
    
    // Set entity permission for expression that contains User or IEnumerable<User>
    pb.Entity<User>("User");

    // Set property permission for expression that contains Item.Price property
    EntityPermissionProviderBuilder itemPermission = pb.Entity<Item>();
    itemPermission.Property(i => i.Price, "ItemPrice")

    pb.Assertion(info => {
        // Return true if user required theses permissions
    }, "Assertion0", "Assertion1");
});
```

You can add **ExpressionAllowAnonymousAttribute** for controller, action or action parameter to ignore authorize.

```csharp
//[ExpressionAllowAnonymous]
public class UserController : ControllerBase
{
    //[ExpressionAllowAnonymous]
    public IActionResult Get(
        /*[ExpressionAllowAnonymous]*/
        ValueParseResult<User, bool> predicate)
    {
        ...
    }
}
```

Property, field can not authorize automatic. use **IAuthorizationService** to authorize manualy

```csharp
public class UserController : ControllerBase
{
    public ValueParseResult<User, bool> Predicate { get; set; }

    private readonly IAuthorizationService _authorizationService;

    public UserController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public Task<IActionResult> Get()
    {
        var authorizeResult = await _authorizationService.AuthorizeAsync(User, Predicate, ExpressionAuthorizationDefaults.AUTHORIZE_POLICY);

        if (!authorizeResult.Successed)
        {
            // Handle result
        }
        return ...
    }
}
```
