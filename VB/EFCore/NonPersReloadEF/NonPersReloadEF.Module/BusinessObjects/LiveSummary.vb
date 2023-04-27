Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports Microsoft.EntityFrameworkCore

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

    <DefaultClassOptions>
    <DC.DomainComponent>
    Public Class LiveSummary
        Inherits NonPersistentObjectBase

        Private _Name As String, _Status As OrderStatus, _Period As Integer

        Private idField As Guid

        <Browsable(False)>
        <DevExpress.ExpressApp.Data.Key>
        Public ReadOnly Property ID As Guid
            Get
                Return idField
            End Get
        End Property

        Public Sub SetKey(ByVal id As Guid)
            idField = id
        End Sub

        Public Property Name As String
            Get
                Return _Name
            End Get

            Private Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Sub SetName(ByVal name As String)
            Me.Name = name
        End Sub

        <Browsable(False)>
        Public Property Status As OrderStatus
            Get
                Return _Status
            End Get

            Private Set(ByVal value As OrderStatus)
                _Status = value
            End Set
        End Property

        Public Sub SetStatus(ByVal status As OrderStatus)
            Me.Status = status
        End Sub

        <Browsable(False)>
        Public Property Period As Integer
            Get
                Return _Period
            End Get

            Private Set(ByVal value As Integer)
                _Period = value
            End Set
        End Property

        Public Sub SetPeriod(ByVal period As Integer)
            Me.Period = period
        End Sub

        Private _Count As Integer?

        Public ReadOnly Property Count As Integer
            Get
                If Not _Count.HasValue AndAlso ObjectSpaceProp IsNot Nothing Then
                    Dim pos = CType(ObjectSpaceProp, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(GetType(Order)))
                    If pos IsNot Nothing Then
                        _Count = GetOrdersQuery(pos).Count()
                    End If
                End If

                Return _Count.Value
            End Get
        End Property

        Private _Total As Decimal?

        Public ReadOnly Property Total As Decimal
            Get
                If Not _Total.HasValue AndAlso ObjectSpaceProp IsNot Nothing Then
                    Dim pos = CType(ObjectSpaceProp, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(GetType(Order)))
                    If pos IsNot Nothing Then
                        _Total = GetOrdersQuery(pos).Sum(Function(t) t.Total)
                    End If
                End If

                Return _Total.Value
            End Get
        End Property

        Private Function GetOrdersQuery(ByVal objectSpace As IObjectSpace) As IQueryable(Of Order)
            Return objectSpace.GetObjectsQuery(Of Order)().Where(Function(t) EF.Functions.DateDiffDay(t.[Date], Date.Now) <= Period AndAlso t.Status = Status)
        End Function

        Private _Orders As IList(Of Order)

        Public ReadOnly Property Orders As IList(Of Order)
            Get
                If _Orders Is Nothing AndAlso ObjectSpaceProp IsNot Nothing Then
                    _Orders = GetOrdersQuery(ObjectSpaceProp).ToList()
                End If

                Return _Orders
            End Get
        End Property

        Private _LatestOrder As Order

        Public ReadOnly Property LatestOrder As Order
            Get
                If _LatestOrder Is Nothing AndAlso ObjectSpaceProp IsNot Nothing Then
                    _LatestOrder = Enumerable.OrderBy(Of Order, Global.System.DateTime)(Orders, Function(o) o.Date).FirstOrDefault()
                End If

                Return _LatestOrder
            End Get
        End Property
    End Class

    Friend Class LiveSummaryAdapter

        Private objectSpace As NonPersistentObjectSpace

        Private objectMap As Dictionary(Of Guid, LiveSummary)

        Public Sub New(ByVal npos As NonPersistentObjectSpace)
            objectSpace = npos
            objectMap = New Dictionary(Of Guid, LiveSummary)()
            Me.objectSpace.ObjectsGetting += AddressOf ObjectSpace_ObjectsGetting
            Me.objectSpace.ObjectByKeyGetting += AddressOf ObjectSpace_ObjectByKeyGetting
            Me.objectSpace.ObjectGetting += AddressOf ObjectSpace_ObjectGetting
            Me.objectSpace.Reloaded += AddressOf ObjectSpace_Reloaded
        End Sub

        Private Sub ObjectSpace_Reloaded(ByVal sender As Object, ByVal e As EventArgs)
            objectMap.Clear()
        End Sub

        Private Sub ObjectSpace_ObjectsGetting(ByVal sender As Object, ByVal e As ObjectsGettingEventArgs)
            If e.ObjectType Is GetType(LiveSummary) Then
                Dim keys = GetAllKeys()
                e.Objects = keys.[Select](Function(key) GetObjectByKey(key)).ToList()
            End If
        End Sub

        Private Sub ObjectSpace_ObjectByKeyGetting(ByVal sender As Object, ByVal e As ObjectByKeyGettingEventArgs)
            If e.ObjectType Is GetType(LiveSummary) Then
                e.Object = GetObjectByKey(CType(e.Key, Guid))
            End If
        End Sub

        Private Sub ObjectSpace_ObjectGetting(ByVal sender As Object, ByVal e As ObjectGettingEventArgs)
            If TypeOf e.SourceObject Is LiveSummary Then
                Dim obj = CType(e.SourceObject, LiveSummary)
                If Not objectMap.ContainsValue(obj) Then
                    e.TargetObject = GetObject(obj)
                End If
            End If
        End Sub

        Private Function GetObject(ByVal obj As LiveSummary) As LiveSummary
            Dim link = TryCast(obj, IObjectSpaceLink)
            If link Is Nothing OrElse link.ObjectSpace Is Nothing OrElse link.ObjectSpace.IsNewObject(obj) Then
                Return Nothing
            Else
                Return GetObjectByKey(obj.ID)
            End If
        End Function

        Private Function GetObjectByKey(ByVal key As Guid) As LiveSummary
            Dim obj As LiveSummary = Nothing
            If Not objectMap.TryGetValue(key, obj) Then
                Dim data = GetDataByKey(key)
                If data IsNot Nothing Then
                    obj = New LiveSummary()
                    obj.SetKey(CType(key, Guid))
                    obj.SetName(data.Name)
                    obj.SetPeriod(data.Period)
                    obj.SetStatus(data.Status)
                    objectMap.Add(key, obj)
                End If
            End If

            Return obj
        End Function
    End Class

    Friend Module LiveSummaryPresetStorage

        Public Class LiveSummaryPreset

            Public Name As String

            Public Status As OrderStatus

            Public Period As Integer
        End Class

        Private presets As Dictionary(Of Guid, LiveSummaryPreset)

        Sub New()
            presets = New Dictionary(Of Guid, LiveSummaryPreset)()
            Call presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {.Name = "To deliver", .Status = OrderStatus.Active, .Period = 100})
            Call presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {.Name = "Delayed this week", .Status = OrderStatus.Delayed, .Period = 7})
        End Sub

        Public Function GetAllKeys() As IEnumerable(Of Guid)
            Return presets.Keys
        End Function

        Public Function GetDataByKey(ByVal key As Guid) As LiveSummaryPreset
            Dim result As LiveSummaryPreset
            If Not presets.TryGetValue(key, result) Then
                result = Nothing
            End If

            Return result
        End Function
    End Module
End Namespace
