using LinqExpressionParser.AspNetCore.Results;
using LinqExpressionParser.Tests.AspNetCore.TestModels;

using Microsoft.AspNetCore.Mvc;

namespace LinqExpressionParser.Tests.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class EntityController<T> : ControllerBase
    {
        public IActionResult Get([FromQuery(Name = "value")] ValueParseResult<T, object> valueParse)
        {
            return Ok();
        }
    }

    public class IEntityController : EntityController<IEntity> { }
    public class EntityAController : EntityController<EntityA> { }
    public class EntityBController : EntityController<EntityB> { }
    public class GenericAIEntityController : EntityController<GenericA<IEntity>> { }
    public class GenericBEntityAController : EntityController<GenericB<EntityA>> { }
    public class GenericAEntityAController : EntityController<GenericA<EntityA>> { }
    public class GenericBIEntityController : EntityController<GenericB<IEntity>> { }
    public class IEntityEnumerableController : EntityController<IEntity> { }
    public class EntityAEnumerableController : EntityController<EntityAEnumerable> { }
    public class EntityBEnumerableController : EntityController<EntityBEnumerable> { }
    public class IGenericEntityIEntityController : EntityController<IGenericEntity<IEntity>> { }
    public class GenericEntityAEntityAController : EntityController<GenericEntityA<EntityA>> { }
    public class IGenericEntityEntityAController : EntityController<IGenericEntity<EntityA>> { }
    public class GenericEntityAIEntityController : EntityController<GenericEntityA<IEntity>> { }
    public class IGenericEntityStringController : EntityController<IGenericEntity<string>> { }
    public class GenericEntityAStringController : EntityController<GenericEntityA<string>> { }
}