Imports DevExpress.ExpressApp.ApplicationBuilder
Imports DevExpress.ExpressApp.Blazor.ApplicationBuilder
Imports DevExpress.ExpressApp.Blazor.Services
Imports DevExpress.Persistent.Base
Imports Microsoft.AspNetCore.Authentication.Cookies
Imports Microsoft.AspNetCore.Components.Server.Circuits
Imports Microsoft.EntityFrameworkCore
Imports NonPersReloadEF.Blazor.Server.Services
Imports DevExpress.ExpressApp.Core

Namespace NonPersReloadEF.Blazor.Server

    Public Class Startup

        Public Sub New(ByVal configuration As IConfiguration)
            Me.Configuration = configuration
        End Sub

        Public ReadOnly Property Configuration As IConfiguration

        ' This method gets called by the runtime. Use this method to add services to the container.
        ' For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        Public Sub ConfigureServices(ByVal services As IServiceCollection)
            services.AddSingleton(GetType(Microsoft.AspNetCore.SignalR.HubConnectionHandler(Of )), GetType(ProxyHubConnectionHandler(Of )))
            services.AddRazorPages()
            services.AddServerSideBlazor()
            services.AddHttpContextAccessor()
            services.AddScoped(Of CircuitHandler, CircuitHandlerProxy)()
            services.AddXaf(Configuration, Function(builder)
                builder.UseApplication(Of NonPersReloadEFBlazorApplication)()
                builder.Modules.Add(Of NonPersReloadEF.Module.NonPersReloadEFModule)().Add(Of NonPersReloadEFBlazorModule)()
                builder.ObjectSpaceProviders.AddEFCore().WithDbContext(Of NonPersReloadEF.Module.BusinessObjects.NonPersReloadEFEFCoreDbContext)(Function(serviceProvider, options)
                    ' Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
                    ' Do not use this code in production environment to avoid data loss.
                    ' We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
                    'options.UseInMemoryDatabase("InMemory");
                    Dim connectionString As String = Nothing
                    If Configuration.GetConnectionString("ConnectionString") IsNot Nothing Then
                        connectionString = Configuration.GetConnectionString("ConnectionString")
                    End If

#If EASYTEST
                    if(Configuration.GetConnectionString("EasyTestConnectionString") != null) {
                        connectionString = Configuration.GetConnectionString("EasyTestConnectionString");
                    }
#End If
                    ArgumentNullException.ThrowIfNull(connectionString)
                    options.UseSqlServer(connectionString)
                    options.UseChangeTrackingProxies()
                    options.UseObjectSpaceLinkProxies()
                    options.UseLazyLoadingProxies()
                End Function).AddNonPersistent()
            End Function)
        End Sub

        ' This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        Public Sub Configure(ByVal app As IApplicationBuilder, ByVal env As IWebHostEnvironment)
            If env.IsDevelopment() Then
                app.UseDeveloperExceptionPage()
            Else
                app.UseExceptionHandler("/Error")
                ' The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
                app.UseHsts()
            End If

            app.UseHttpsRedirection()
            app.UseRequestLocalization()
            app.UseStaticFiles()
            app.UseRouting()
            app.UseXaf()
            app.UseEndpoints(Function(endpoints)
                endpoints.MapXafEndpoints()
                endpoints.MapBlazorHub()
                endpoints.MapFallbackToPage("/_Host")
                endpoints.MapControllers()
            End Function)
        End Sub
    End Class
End Namespace
