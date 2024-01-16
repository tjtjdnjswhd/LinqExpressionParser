using LinqExpressionParser.AspNetCore.Authorization.Extensions;
using LinqExpressionParser.AspNetCore.Extensions;
using LinqExpressionParser.Tests.AspNetCore.Controllers;
using LinqExpressionParser.Tests.AspNetCore.TestModels;

using Microsoft.AspNetCore.Mvc.Testing;

using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace LinqExpressionParser.Tests.AspNetCore
{
    [TestClass]
    public class PropertyPermissionTest
    {
        private static readonly WebApplicationFactory<Program> _factory;

        static PropertyPermissionTest()
        {
            _factory = new();
            _factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    Assembly controllerAssembly = typeof(IEntityController).Assembly;
                    services.AddMvc().AddApplicationPart(controllerAssembly);
                    services.AddExpressionParse();
                    services.AddExpressionAuthorization(options =>
                    {
                        options.PermissionFinder = user => user.FindFirstValue("permissions")?.Split(',') ?? [];
                        options.PermissionComparsion = StringComparison.OrdinalIgnoreCase;
                    }, pb =>
                    {
                        pb.Entity<IEntity>().Property(e => e.Name, "IEntityName");
                        pb.Entity<EntityA>().Property(e => e.Name, "EntityAName");
                        pb.Entity<EntityA>().Property(e => e.AbstractName, "EntityAAbstractName");
                        pb.Entity<EntityB>().Property(e => e.Entities, "IEntityEntities");
                    });
                });
            });
        }

        [ClassCleanup]
        public static void Clean()
        {
            _factory.Dispose();
        }

        private static IEnumerable<object[]> OkTestData => new object[][]
        {
            ["IEntity", "Name", new string[] {"IEntityName"}],
            ["IEntity", "Entities", new string[] {"IEntityEntities"}],
            ["EntityA", "Name", new string[] {"EntityAName", "IEntityName"}],
            ["EntityB", "Name", new string[] {"EntityAName", "IEntityName"}],
            ["EntityA", "AbstractName", new string[] {"EntityAAbstractName"}],
            ["EntityA", "Entities", new string[] {"IEntityEntities"}],
        };

        [DataTestMethod]
        [DynamicData(nameof(OkTestData))]
        public async Task OkTest(string controllerName, string propertyName, string[] permissions)
        {
            var client = _factory.CreateClient();

            StringBuilder sb = new(256);
            foreach (var permission in permissions)
            {
                sb.Append("permissions=");
                sb.Append(permission);
                sb.Append('&');
            }

            (await client.GetAsync($"api/home/login?{sb}")).EnsureSuccessStatusCode();
            var response = await client.GetAsync($"api/{controllerName}?value={propertyName}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}