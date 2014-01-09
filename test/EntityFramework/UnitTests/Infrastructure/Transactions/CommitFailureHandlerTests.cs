﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure.Transactions
{
    using System.Data.Common;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Data.Entity.Resources;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using MockHelper = System.Data.Entity.Core.Objects.MockHelper;

    public class CommitFailureHandlerTests
    {
        public class Initialize : TestBase
        {
            [Fact]
            public void Initializes_with_ObjectContext()
            {
                var context = MockHelper.CreateMockObjectContext<object>();

                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context);

                    Assert.Same(context, handler.ObjectContext);
                    Assert.Same(context.InterceptionContext.DbContexts.FirstOrDefault(), handler.DbContext);
                    Assert.Same(((EntityConnection)context.Connection).StoreConnection, handler.Connection);
                    Assert.Same(((EntityConnection)context.Connection).StoreConnection, handler.Connection);
                    Assert.IsType<TransactionContext>(handler.TransactionContext);
                }
            }

            [Fact]
            public void Initializes_with_DbContext()
            {
                var context = new DbContext("c");

                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context, context.Database.Connection);

                    Assert.Null(handler.ObjectContext);
                    Assert.Same(context, handler.DbContext);
                    Assert.Same(context.Database.Connection, handler.Connection);
                    Assert.IsType<TransactionContext>(handler.TransactionContext);
                }
            }

            [Fact]
            public void Throws_for_null_parameters()
            {
                using (var handler = new CommitFailureHandler())
                {
                    Assert.Equal(
                        "connection",
                        Assert.Throws<ArgumentNullException>(() => handler.Initialize(new DbContext("c"), null)).ParamName);
                    Assert.Equal(
                        "context",
                        Assert.Throws<ArgumentNullException>(() => handler.Initialize(null, new Mock<DbConnection>().Object)).ParamName);
                    Assert.Equal(
                        "context",
                        Assert.Throws<ArgumentNullException>(() => handler.Initialize(null)).ParamName);
                }
            }


            [Fact]
            public void Throws_if_already_initialized_with_ObjectContext()
            {
                var context = MockHelper.CreateMockObjectContext<object>();

                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context);

                    Assert.Equal(
                        Strings.TransactionHandler_AlreadyInitialized,
                        Assert.Throws<InvalidOperationException>(() => handler.Initialize(context)).Message);

                    var dbContext = new DbContext("c");
                    Assert.Equal(
                        Strings.TransactionHandler_AlreadyInitialized,
                        Assert.Throws<InvalidOperationException>(() => handler.Initialize(dbContext, dbContext.Database.Connection)).Message);
                }
            }

            [Fact]
            public void Throws_if_already_initialized_with_DbContext()
            {
                var dbContext = new DbContext("c");

                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(dbContext, dbContext.Database.Connection);

                    var context = MockHelper.CreateMockObjectContext<object>();
                    Assert.Equal(
                        Strings.TransactionHandler_AlreadyInitialized,
                        Assert.Throws<InvalidOperationException>(() => handler.Initialize(context)).Message);

                    Assert.Equal(
                        Strings.TransactionHandler_AlreadyInitialized,
                        Assert.Throws<InvalidOperationException>(() => handler.Initialize(dbContext, dbContext.Database.Connection)).Message);
                }
            }
        }

        public class Dispose : TestBase
        {
            [Fact]
            public void Removes_interceptor()
            {
                var mockConnection = new Mock<DbConnection>().Object;
                var handlerMock = new Mock<CommitFailureHandler> { CallBase = true };
                using (handlerMock.Object)
                {
                    DbInterception.Dispatch.Connection.Close(mockConnection, new DbInterceptionContext());
                    handlerMock.Verify(m => m.Closed(It.IsAny<DbConnection>(), It.IsAny<DbConnectionInterceptionContext>()), Times.Once());

                    handlerMock.Object.Dispose();

                    DbInterception.Dispatch.Connection.Close(mockConnection, new DbInterceptionContext());
                    handlerMock.Verify(m => m.Closed(It.IsAny<DbConnection>(), It.IsAny<DbConnectionInterceptionContext>()), Times.Once());
                }
            }

            [Fact]
            public void Can_be_invoked_twice_without_throwing()
            {
                var handler = new CommitFailureHandler();

                handler.Dispose();
                handler.Dispose();
            }
        }

        public class MatchesParentContext : TestBase
        {
            [Fact]
            public void Throws_for_null_parameters()
            {
                using (var handler = new CommitFailureHandler())
                {
                    Assert.Equal(
                        "connection",
                        Assert.Throws<ArgumentNullException>(() => handler.MatchesParentContext(null, new DbInterceptionContext()))
                            .ParamName);
                    Assert.Equal(
                        "interceptionContext",
                        Assert.Throws<ArgumentNullException>(() => handler.MatchesParentContext(new Mock<DbConnection>().Object, null))
                            .ParamName);
                }
            }

            [Fact]
            public void Returns_false_with_DbContext_if_nothing_matches()
            {
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(new DbContext("c"), CreateMockConnection());

                    Assert.False(
                        handler.MatchesParentContext(
                            new Mock<DbConnection>().Object,
                            new DbInterceptionContext().WithObjectContext(MockHelper.CreateMockObjectContext<object>())
                                .WithDbContext(new DbContext("c"))));
                }
            }

            [Fact]
            public void Returns_false_with_DbContext_if_different_context_same_connection()
            {
                var connection = CreateMockConnection();
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(new DbContext("c"), connection);

                    Assert.False(
                        handler.MatchesParentContext(
                            connection,
                            new DbInterceptionContext().WithObjectContext(MockHelper.CreateMockObjectContext<object>())
                                .WithDbContext(new DbContext("c"))));
                }
            }

            [Fact]
            public void Returns_true_with_DbContext_if_same_context()
            {
                var context = new DbContext("c");
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context, CreateMockConnection());

                    Assert.True(
                        handler.MatchesParentContext(
                            new Mock<DbConnection>().Object,
                            new DbInterceptionContext().WithObjectContext(MockHelper.CreateMockObjectContext<object>())
                                .WithDbContext(context)));
                }
            }

            [Fact]
            public void Returns_true_with_DbContext_if_no_context_same_connection()
            {
                var connection = CreateMockConnection();
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(new DbContext("c"), connection);

                    Assert.True(
                        handler.MatchesParentContext(
                            connection,
                            new DbInterceptionContext()));
                }
            }

            [Fact]
            public void Returns_false_with_ObjectContext_if_nothing_matches()
            {
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(MockHelper.CreateMockObjectContext<object>());

                    Assert.False(
                        handler.MatchesParentContext(
                            new Mock<DbConnection>().Object,
                            new DbInterceptionContext().WithObjectContext(MockHelper.CreateMockObjectContext<object>())
                                .WithDbContext(new DbContext("c"))));
                }
            }

            [Fact]
            public void Returns_false_with_ObjectContext_if_different_context_same_connection()
            {
                var context = MockHelper.CreateMockObjectContext<object>();
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context);

                    Assert.False(
                        handler.MatchesParentContext(
                            ((EntityConnection)context.Connection).StoreConnection,
                            new DbInterceptionContext().WithObjectContext(MockHelper.CreateMockObjectContext<object>())
                                .WithDbContext(new DbContext("c"))));
                }
            }

            [Fact]
            public void Returns_true_with_ObjectContext_if_same_ObjectContext()
            {
                var context = MockHelper.CreateMockObjectContext<object>();
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context);

                    Assert.True(
                        handler.MatchesParentContext(
                            new Mock<DbConnection>().Object,
                            new DbInterceptionContext().WithObjectContext(context)
                                .WithDbContext(new DbContext("c"))));
                }
            }

            [Fact]
            public void Returns_true_with_ObjectContext_if_no_context_same_connection()
            {
                var context = MockHelper.CreateMockObjectContext<object>();
                using (var handler = new CommitFailureHandler())
                {
                    handler.Initialize(context);

                    Assert.True(
                        handler.MatchesParentContext(
                            ((EntityConnection)context.Connection).StoreConnection,
                            new DbInterceptionContext()));
                }
            }
        }

        public class PruneTransactionRows : TestBase
        {
            [Fact]
            public void Delegates_to_protected_method()
            {
                var handlerMock = new Mock<CommitFailureHandler> { CallBase = true };
                handlerMock.Protected().Setup("PruneTransactionRows", ItExpr.IsAny<bool>()).Callback(() => { });
                using (var handler = handlerMock.Object)
                {
                    handler.PruneTransactionRows();
                    handlerMock.Protected().Verify("PruneTransactionRows", Times.Once(), true);
                }
            }
        }

#if !NET40
        public class PruneTransactionRowsAsync : TestBase
        {
            [Fact]
            public void Delegates_to_protected_method()
            {
                var handlerMock = new Mock<CommitFailureHandler> { CallBase = true };
                handlerMock.Protected().Setup<Task>("PruneTransactionRowsAsync", ItExpr.IsAny<bool>(), ItExpr.IsAny<CancellationToken>())
                    .Returns(() => Task.FromResult(true));
                using (var handler = handlerMock.Object)
                {
                    handler.PruneTransactionRowsAsync().Wait();
                    handlerMock.Protected().Verify<Task>("PruneTransactionRowsAsync", Times.Once(), true, CancellationToken.None);
                }
            }

            [Fact]
            public void Delegates_to_protected_method_with_CancelationToken()
            {
                var handlerMock = new Mock<CommitFailureHandler> { CallBase = true };
                handlerMock.Protected().Setup<Task>("PruneTransactionRowsAsync", ItExpr.IsAny<bool>(), ItExpr.IsAny<CancellationToken>())
                    .Returns(() => Task.FromResult(true));
                using (var handler = handlerMock.Object)
                {
                    var token = new CancellationToken();
                    handler.PruneTransactionRowsAsync(token).Wait();
                    handlerMock.Protected().Verify<Task>("PruneTransactionRowsAsync", Times.Once(), true, token);
                }
            }
        }
#endif

        private static DbConnection CreateMockConnection()
        {
            var connectionMock = new Mock<DbConnection>();
            connectionMock.Protected()
                .Setup<DbProviderFactory>("DbProviderFactory")
                .Returns(GenericProviderFactory<DbProviderFactory>.Instance);

            return connectionMock.Object;
        }
    }
}