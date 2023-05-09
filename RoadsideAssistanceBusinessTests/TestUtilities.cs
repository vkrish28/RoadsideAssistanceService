using ERAEntities;
using ERAEntities.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusinessTests
{
   
    internal static class TestUtilities
    {
        internal static Mock<ERAContext> GetERAEntitiesMock()
        {
            var mockERAEntities = new Mock<ERAContext>(new DbContextOptions<ERAContext>());

            var asistant = new Assistant();
            var assistants = new List<Assistant>() { asistant };
            var mockAssistantDbSet = SetUpMockDbSet(assistants);
            mockERAEntities.MockPropertyCall(mockAssistantDbSet);

            var customers = new List<Customer>();
            var mockCustomerDbSet = SetUpMockDbSet(customers);
            mockERAEntities.MockPropertyCall(mockCustomerDbSet);

            var astLocations = new List<AssistantLocation>();
            var mockAssistantLocationsDbSet = SetUpMockDbSet(astLocations);
            mockERAEntities.MockPropertyCall(mockAssistantLocationsDbSet);

            var custAstAssigments = new List<CustomerAssistantAssignment>();
            var mockcustAstAssigmentsDbSet = SetUpMockDbSet(custAstAssigments);
            mockERAEntities.MockPropertyCall(mockcustAstAssigmentsDbSet);

            return mockERAEntities;
        }

        internal static Mock<ERAContext> MockPropertyCall<T>(this Mock<ERAContext> mockERAEntities, Mock<DbSet<T>> mockDbSet) where T : class
        {
            var prop = typeof(ERAContext).GetProperties().Single(p => p.PropertyType == typeof(DbSet<T>));
            var param = Expression.Parameter(typeof(ERAContext));
            var body = Expression.PropertyOrField(param, prop.Name);
            var lambdaExp = Expression.Lambda<Func<ERAContext, DbSet<T>>>(body, param);
            mockERAEntities.Setup(lambdaExp).ReturnsDbSet(mockDbSet.Object);
            return mockERAEntities;
        }

        internal static Mock<DbSet<T>> SetUpMockDbSet<T>(IEnumerable<T> entities) where T : class
        {
            IQueryable<T> data = entities.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(a => a.Provider).Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(a => a.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(a => a.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(a => a.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.As<IDbAsyncEnumerable<T>>().Setup(a => a.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
            mockSet.As<IAsyncEnumerable<T>>().Setup(a => a.GetAsyncEnumerator(new CancellationToken(false)))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            return mockSet;
        }


        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Execute<TResult>(expression);
            }
        }

        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider
            {
                get { return new TestAsyncQueryProvider<T>(this); }
            }
            public IAsyncEnumerator<T> GetEnumerator()
            {
                return GetAsyncEnumerator();
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public T Current
            {
                get { return _inner.Current; }
            }
            public Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                return Task.FromResult(_inner.MoveNext());
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask(Task.CompletedTask);
            }
        }

        internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider, IAsyncQueryProvider
        {
            private readonly IQueryProvider _qp;

            internal TestDbAsyncQueryProvider(IQueryProvider qp)
            {
                _qp = qp;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object? Execute(Expression expression)
            {
                return _qp.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _qp.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                
                return Task.FromResult(Execute(expression));
            }

          

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {

                return Execute<TResult>(expression);

            }
        }
    }

    internal class TestAsyncEnumerable<T> : List<T>, IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(GetEnumerator());
    }


    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>, IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumarable) : base(enumarable)
        {
        }

        public TestDbAsyncEnumerable(Expression exp) : base(exp)
        {
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }


    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }
        public T Current => _enumerator.Current;

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_enumerator.MoveNext());
        }
        

    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _e;

        public TestDbAsyncEnumerator(IEnumerator<T> e)
        {
            this._e = e;
        }

        public T Current
        {
            get { return _e.Current; }
        }
        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            _e.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_e.MoveNext());
        }
    }
}
