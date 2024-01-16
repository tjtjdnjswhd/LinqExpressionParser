using LinqExpressionParser.AspNetCore.Results;
using LinqExpressionParser.AspNetCore.Sample.Data;
using LinqExpressionParser.Expressions.Exceptions;
using LinqExpressionParser.Expressions.Parser;
using LinqExpressionParser.Segments;
using LinqExpressionParser.Segments.Exceptions;
using LinqExpressionParser.Segments.Parser;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
using System.Security.Claims;

namespace LinqExpressionParser.AspNetCore.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly SearchSampleContext _context;
    private readonly ISegmentParser _segmentParser;
    private readonly IExpressionParser _expressionParser;
    private readonly IAuthorizationService _authorizationService;

    public SampleController(SearchSampleContext context, ISegmentParser segmentParser, IExpressionParser expressionParser, IAuthorizationService authorizationService)
    {
        _context = context;
        _segmentParser = segmentParser;
        _expressionParser = expressionParser;
        _authorizationService = authorizationService;
    }

    [HttpGet("Login")]
    public async Task<ActionResult> Login(string role)
    {
        Claim[] claims = new Claim[]
        {
            new(ClaimTypes.Role, role)
        };

        ClaimsIdentity identity = new(claims, "Cookie");
        ClaimsPrincipal principal = new(identity);
        await HttpContext.SignInAsync(principal);
        return Ok();
    }

    [HttpGet("Logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [HttpGet("Get/modelbind")]
    public IActionResult Get([FromQuery] ValueParseResult<User, bool> search, [FromQuery] SelectorParseResult<User>? selector)
    {
        IQueryable<User> query = _context.User.Where(search.GetExpression());
        if (selector is not null)
        {
            return Ok(selector.Select(query));
        }

        return Ok(query.AsAsyncEnumerable());
    }

    [HttpGet("Get/manual")]
    public async Task<IActionResult> Get([FromQuery] string search, [FromQuery] string selector)
    {
        try
        {
            ValueSegment searchSegment = _segmentParser.ParseValue(search);
            Expression<Func<User, bool>> searchExp = _expressionParser.ParseValueExpression<User, bool>(searchSegment);

            SelectorSegment selectorSegment = _segmentParser.ParseSelector(selector);
            LambdaExpression selectorExp = _expressionParser.ParseSelectorExpression<User>(selectorSegment);

            var searchExpAuthorize = await _authorizationService.AuthorizeAsync(User, searchExp, "manual");
            var selectorExpAuthorize = await _authorizationService.AuthorizeAsync(User, selectorExp, "manual");

            if (!searchExpAuthorize.Succeeded || !selectorExpAuthorize.Succeeded)
            {
                return User.Identity?.IsAuthenticated ?? false ? Forbid() : Unauthorized();
            }

            return Ok(_context.User.Where(searchExp));
        }
        catch (SegmentParseExceptionBase e)
        {
            //Handle segment parser exception
            switch (e)
            {
                case UndefinedSegmentParseException _:
                case SegmentFormatException _:
                    break;
            }
            throw;
        }
        catch (ExpressionParseExceptionBase e)
        {
            //Handle expression parser exception
            switch (e)
            {
                case InvalidBodyTypeException _:
                case InvalidLambdaSourceException _:
                case InvalidOperationSegmentsException _:
                case LambdaMethodNotMappedException _:
                case MethodNotMappedException _:
                case OperatorNotMappedException _:
                case PrefixChildNotExistException _:
                case PrefixDuplicateException _:
                case PropertyNotExistException _:
                case UndefinedExpressionParseException _:
                    break;
            }
            throw;
        }
    }
}