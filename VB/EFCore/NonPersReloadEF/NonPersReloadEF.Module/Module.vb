Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Persistent.BaseImpl.EF
Imports NonPersistentObjectsDemo.Module.BusinessObjects

Namespace NonPersReloadEF.Module

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    Public NotInheritable Class NonPersReloadEFModule
        Inherits ModuleBase

        Public Sub New()
            ' 
            ' NonPersReloadEFModule
            ' 
            RequiredModuleTypes.Add(GetType(SystemModule.SystemModule))
            RequiredModuleTypes.Add(GetType(Objects.BusinessClassLibraryCustomizationModule))
        End Sub

        Public Overrides Function GetModuleUpdaters(ByVal objectSpace As IObjectSpace, ByVal versionFromDB As Version) As IEnumerable(Of ModuleUpdater)
            Dim updater As ModuleUpdater = New DatabaseUpdate.Updater(objectSpace, versionFromDB)
            Return New ModuleUpdater() {updater}
        End Function

        Public Overrides Sub Setup(ByVal application As XafApplication)
            MyBase.Setup(application)
            ' Manage various aspects of the application UI and behavior at the module level.
            application.SetupComplete += AddressOf Application_SetupComplete
        End Sub

        Private Sub Application_SetupComplete(ByVal sender As Object, ByVal e As EventArgs)
            Me.Application.ObjectSpaceCreated += AddressOf Application_ObjectSpaceCreated
            NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = True
        End Sub

        Private Sub Application_ObjectSpaceCreated(ByVal sender As Object, ByVal e As ObjectSpaceCreatedEventArgs)
            Dim npos = TryCast(e.ObjectSpace, NonPersistentObjectSpace)
            If npos IsNot Nothing Then
                If Not npos.AdditionalObjectSpaces.Any(Function(os) os.IsKnownType(GetType(BaseObject))) Then
                    Dim persistentObjectSpace As IObjectSpace = Application.CreateObjectSpace(GetType(BaseObject))
                    npos.AdditionalObjectSpaces.Add(persistentObjectSpace)
                End If

                npos.AutoDisposeAdditionalObjectSpaces = True
                npos.AutoRefreshAdditionalObjectSpaces = True
                Dim tmp_LiveSummaryAdapter = New LiveSummaryAdapter(npos)
            End If
        End Sub
    End Class
End Namespace
