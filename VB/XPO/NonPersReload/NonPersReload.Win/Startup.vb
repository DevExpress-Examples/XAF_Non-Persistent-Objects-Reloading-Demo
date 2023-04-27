Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Win
Imports DevExpress.ExpressApp.Design

Namespace NonPersReload.Win

    Public Class ApplicationBuilder
        Implements IDesignTimeApplicationFactory

        Public Shared Function BuildApplication(ByVal connectionString As String) As WinApplication
            Dim builder = WinApplication.CreateBuilder()
            builder.UseApplication(Of Win.NonPersReloadWindowsFormsApplication)()
            builder.Modules.Add(Of NonPersReload.Module.NonPersReloadModule)().Add(Of Win.NonPersReloadWinModule)()
            builder.ObjectSpaceProviders.AddXpo(Function(application, options)
                options.ConnectionString = connectionString
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
