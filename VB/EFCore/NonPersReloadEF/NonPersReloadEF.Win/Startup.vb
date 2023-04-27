Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Win
Imports DevExpress.ExpressApp.Design

Namespace NonPersReloadEF.Win

    Public Class ApplicationBuilder
        Implements IDesignTimeApplicationFactory

        Public Shared Function BuildApplication(ByVal connectionString As String) As WinApplication
            Dim builder = WinApplication.CreateBuilder()
            builder.UseApplication(Of Win.NonPersReloadEFWindowsFormsApplication)()
            builder.Modules.Add(Of NonPersReloadEF.Module.NonPersReloadEFModule)().Add(Of Win.NonPersReloadEFWinModule)()
            builder.ObjectSpaceProviders.AddEFCore().WithDbContext(Of NonPersReloadEF.Module.BusinessObjects.NonPersReloadEFEFCoreDbContext)(Function(application, options)
                ' Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
                ' Do not use this code in production environment to avoid data loss.
                ' We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
                'options.UseInMemoryDatabase("InMemory");
                options.UseSqlServer(CStr(connectionString))
                options.UseChangeTrackingProxies()
                options.UseObjectSpaceLinkProxies()
            End Function).AddNonPersistent()
            builder.AddBuildStep(Function(application)
                application.ConnectionString = connectionString
#If DEBUG
                If System.Diagnostics.Debugger.IsAttached AndAlso application.CheckCompatibilityType Is CheckCompatibilityType.DatabaseSchema Then
                    application.DatabaseUpdateMode = DevExpress.ExpressApp.DatabaseUpdateMode.UpdateDatabaseAlways
                End If
#End If
            End Function)
            Dim winApplication = builder.Build()
            Return winApplication
        End Function

        Private Function Create() As XafApplication Implements IDesignTimeApplicationFactory.Create
            Return BuildApplication(XafApplication.DesignTimeConnectionString)
        End Function
    End Class
End Namespace
