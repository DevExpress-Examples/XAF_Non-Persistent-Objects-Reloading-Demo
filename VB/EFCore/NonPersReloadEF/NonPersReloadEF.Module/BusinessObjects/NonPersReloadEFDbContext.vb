Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports DevExpress.ExpressApp.Design
Imports DevExpress.ExpressApp.EFCore.DesignTime
Imports NonPersistentObjectsDemo.Module.BusinessObjects

Namespace NonPersReloadEF.Module.BusinessObjects

    ' This code allows our Model Editor to get relevant EF Core metadata at design time.
    ' For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
    Public Class NonPersReloadEFContextInitializer
        Inherits DbContextTypesInfoInitializerBase

        Protected Overrides Function CreateDbContext() As DbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of NonPersReloadEFEFCoreDbContext)().UseSqlServer(";").UseChangeTrackingProxies().UseObjectSpaceLinkProxies()
            Return New NonPersReloadEFEFCoreDbContext(optionsBuilder.Options)
        End Function
    End Class

    'This factory creates DbContext for design-time services. For example, it is required for database migration.
    Public Class NonPersReloadEFDesignTimeDbContextFactory
        Implements IDesignTimeDbContextFactory(Of NonPersReloadEFEFCoreDbContext)

        Public Function CreateDbContext(ByVal args As String()) As NonPersReloadEFEFCoreDbContext Implements IDesignTimeDbContextFactory(Of NonPersReloadEFEFCoreDbContext).CreateDbContext
            Throw New InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.")
        'var optionsBuilder = new DbContextOptionsBuilder<NonPersReloadEFEFCoreDbContext>();
        'optionsBuilder.UseSqlServer("Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=NonPersReloadEF");
        'optionsBuilder.UseChangeTrackingProxies();
        'optionsBuilder.UseObjectSpaceLinkProxies();
        'return new NonPersReloadEFEFCoreDbContext(optionsBuilder.Options);
        End Function
    End Class

    <TypesInfoInitializer(GetType(NonPersReloadEFContextInitializer))>
    Public Class NonPersReloadEFEFCoreDbContext
        Inherits DbContext

        Public Sub New(ByVal options As DbContextOptions(Of NonPersReloadEFEFCoreDbContext))
            MyBase.New(options)
        End Sub

        Public Property Orders As DbSet(Of Order)

        Public Property OrderLines As DbSet(Of OrderLine)

        Public Property Products As DbSet(Of Product)

        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues)
        End Sub
    End Class
End Namespace
