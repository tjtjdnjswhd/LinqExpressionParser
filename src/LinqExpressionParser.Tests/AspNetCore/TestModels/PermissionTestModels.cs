#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
using System.Collections;

namespace LinqExpressionParser.Tests.AspNetCore.TestModels
{
    public interface IEntity
    {
        public string Name { get; }
        public IEnumerable<IEntity> Entities { get; }
    }

    public abstract class EntityA : IEntity
    {
        public string Name { get; }
        public string AName { get; }
        public abstract string AbstractName { get; }

        public IEnumerable<IEntity> Entities { get; }
    }

    public class EntityB : EntityA
    {
        public string BName { get; }
        public override string AbstractName { get; }
    }

    public class GenericA<T> : IEntity
    {
        public string Name { get; }
        public Guid Uid { get; }
        public T GenericProperty { get; }
        public IEnumerable<IEntity> Entities { get; }
    }

    public class GenericB<T> : GenericA<T> { }

    public class IEntityEnumerable : IEnumerable<IEntity>
    {
        public IEnumerator<IEntity> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class EntityAEnumerable : IEnumerable<EntityA>
    {
        public IEnumerator<EntityA> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class EntityBEnumerable : IEnumerable<EntityB>
    {
        public IEnumerator<EntityB> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public interface IGenericEntity<T>
    {
        public IGenericEntity<T> Child { get; }
        public T GenericProperty { get; }
    }

    public class GenericEntityA<T> : IGenericEntity<T>
    {
        public IGenericEntity<T> Child { get; }

        public T GenericProperty { get; }
    }
}