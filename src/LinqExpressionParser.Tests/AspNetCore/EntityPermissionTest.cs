using LinqExpressionParser.AspNetCore.Authorization.Extensions;
using LinqExpressionParser.AspNetCore.Extensions;
using LinqExpressionParser.Tests.AspNetCore.Controllers;
using LinqExpressionParser.Tests.AspNetCore.TestModels;

using Microsoft.AspNetCore.Mvc.Testing;

using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace LinqExpressionParser.Tests.AspNetCore
{
    [TestClass]
    public class EntityPermissionTest
    {
        private static readonly WebApplicationFactory<Program> _factory;

        static EntityPermissionTest()
        {
            _factory = new();
            _factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(service =>
                {
                    Assembly controllerAssembly = typeof(IEntityController).Assembly;
                    service.AddMvc().AddApplicationPart(controllerAssembly);
                    service.AddExpressionParse();
                    service.AddExpressionAuthorization(options =>
                    {
                        options.PermissionFinder = user => user.FindFirstValue("permissions")?.Split(',') ?? [];
                        options.PermissionComparsion = StringComparison.OrdinalIgnoreCase;
                    },
                    pb =>
                    {
                        pb.Entity<IEntity>("IEntity");
                        pb.Entity<EntityA>("EntityA");
                        pb.Entity<EntityB>("EntityB");

                        pb.Entity<GenericA<IEntity>>("GenericA");
                        pb.Entity<GenericB<EntityA>>("GenericB");

                        pb.Entity<IGenericEntity<IEntity>>("IGenericEntity");
                        pb.Entity<GenericEntityA<EntityA>>("GenericEntityA");

                        pb.Entity<IGenericEntity<EntityA>>("IGenericEntity");
                        pb.Entity<GenericEntityA<IEntity>>("GenericEntityA");

                        pb.Entity<IGenericEntity<string>>("IGenericEntity");
                        pb.Entity<GenericEntityA<string>>("GenericEntityA");
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
            ["IEntity", new string[] {"IEntity"}],
            ["EntityA", new string[] {"IEntity", "EntityA"}],
            ["EntityB", new string[] {"IEntity", "EntityA", "EntityB"}],

            ["GenericAIEntity", new string[] {"IEntity", "GenericA"}],
            ["GenericAEntityA", new string[] {"IEntity", "GenericA"}],
            ["GenericBEntityA", new string[] {"IEntity", "GenericA", "GenericB"}],
            ["GenericBIEntity", new string[] {"IEntity", "GenericA", "GenericB"}],

            ["IEntityEnumerable", new string[] {"IEntity"}],
            ["EntityAEnumerable", new string[] {"IEntity", "EntityA"}],
            ["EntityBEnumerable", new string[] {"IEntity", "EntityA", "EntityB"}],

            ["IGenericEntityIEntity", new string[] {"IGenericEntity"}],
            ["GenericEntityAEntityA", new string[] { "IGenericEntity", "GenericEntityA" }],

            ["IGenericEntityEntityA", new string[] {"IGenericEntity"}],
            ["GenericEntityAIEntity", new string[] {"IGenericEntity", "GenericEntityA" }],

            ["IGenericEntityString", new string[] {"IGenericEntity"}],
            ["GenericEntityAString", new string[] {"IGenericEntity", "GenericEntityA"}],
        };

        [TestMethod]
        [DynamicData(nameof(OkTestData))]
        public async Task OkTest(string controllerName, string[] permissions)
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
            var response = await client.GetAsync($"api/{controllerName}?value='aa'");

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        private static IEnumerable<object[]> ForbidTestData => new object[][]
        {
            ["IEntity", new string[] {"EntityA"}],
            ["EntityA", new string[] {"IEntity", "EntityB"}],
            ["EntityB", new string[] {"EntityA"}],

            ["GenericAIEntity", new string[] {"GenericAIEntity"}],
            ["GenericAEntityA", new string[] {"GenericAEntityA"}],
            ["GenericBEntityA", new string[] {"GenericAEntityA", "GenericBEntityA"}],
            ["GenericBIEntity", new string[] {"GenericAIEntity", "GenericBIEntity"}],

            ["IEntityEnumerable", new string[] {"EntityA"}],
            ["EntityAEnumerable", new string[] {"EntityA"}],
            ["EntityBEnumerable", new string[] {"EntityB"}],

            ["IGenericEntityIEntity", new string[] {"GenericEntityIEntity"}],
            ["GenericEntityAEntityA", new string[] {"GenericEntityAEntityA" }],

            ["IGenericEntityEntityA", new string[] {"GenericEntityEntityA"}],
            ["GenericEntityAIEntity", new string[] {"GenericEntityAIEntity" }],

            ["IGenericEntityString", new string[] {"GenericEntityString"}],
            ["GenericEntityAString", new string[] {"IGenericEntityString"}],
        };

        [TestMethod]
        [DynamicData(nameof(ForbidTestData))]
        public async Task ForbidTest(string controllerName, string[] permissions)
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
            var response = await client.GetAsync($"api/{controllerName}?value='aa'");

            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}