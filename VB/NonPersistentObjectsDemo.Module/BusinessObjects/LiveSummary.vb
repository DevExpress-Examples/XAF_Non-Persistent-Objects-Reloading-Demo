Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DefaultClassOptions>
	<DevExpress.ExpressApp.DC.DomainComponent>
	Public Class LiveSummary
		Inherits NonPersistentObjectBase

'INSTANT VB NOTE: The field id was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private id_Conflict As Guid
		<Browsable(False)>
		<DevExpress.ExpressApp.Data.Key>
		Public ReadOnly Property ID() As Guid
			Get
				Return id_Conflict
			End Get
		End Property
		Public Sub SetKey(ByVal id As Guid)
			Me.id_Conflict = id
		End Sub
		Private privateName As String
		Public Property Name() As String
			Get
				Return privateName
			End Get
			Private Set(ByVal value As String)
				privateName = value
			End Set
		End Property
		Public Sub SetName(ByVal name As String)
			Me.Name = name
		End Sub
		Private privateStatus As OrderStatus
		<Browsable(False)>
		Public Property Status() As OrderStatus
			Get
				Return privateStatus
			End Get
			Private Set(ByVal value As OrderStatus)
				privateStatus = value
			End Set
		End Property
		Public Sub SetStatus(ByVal status As OrderStatus)
			Me.Status = status
		End Sub
		Private privatePeriod As Integer
		<Browsable(False)>
		Public Property Period() As Integer
			Get
				Return privatePeriod
			End Get
			Private Set(ByVal value As Integer)
				privatePeriod = value
			End Set
		End Property
		Public Sub SetPeriod(ByVal period As Integer)
			Me.Period = period
		End Sub
		Private _Count? As Integer
		Public ReadOnly Property Count() As Integer
			Get
				If Not _Count.HasValue AndAlso ObjectSpace IsNot Nothing Then
					Dim pos = DirectCast(ObjectSpace, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(GetType(Order)))
					If pos IsNot Nothing Then
						_Count = Convert.ToInt32(pos.Evaluate(GetType(Order), CriteriaOperator.Parse("Count()"), Criteria))
					End If
				End If
				Return _Count.Value
			End Get
		End Property
		Private _Total? As Decimal
		Public ReadOnly Property Total() As Decimal
			Get
				If Not _Total.HasValue AndAlso ObjectSpace IsNot Nothing Then
					Dim pos = DirectCast(ObjectSpace, NonPersistentObjectSpace).AdditionalObjectSpaces.FirstOrDefault(Function(os) os.IsKnownType(GetType(Order)))
					If pos IsNot Nothing Then
						_Total = Convert.ToDecimal(pos.Evaluate(GetType(Order), CriteriaOperator.Parse("Sum([Total])"), Criteria))
					End If
				End If
				Return _Total.Value
			End Get
		End Property
		Private ReadOnly Property Criteria() As CriteriaOperator
			Get
				Return CriteriaOperator.Parse("DateDiffDay([Date], Now()) <= ? And [Status] = ?", Period, Status)
			End Get
		End Property
		Private _Orders As IList(Of Order)
		Public ReadOnly Property Orders() As IList(Of Order)
			Get
				If _Orders Is Nothing AndAlso ObjectSpace IsNot Nothing Then
					_Orders = ObjectSpace.GetObjects(Of Order)(Criteria).ToArray()
				End If
				Return _Orders
			End Get
		End Property
		Private _LatestOrder As Order
		Public ReadOnly Property LatestOrder() As Order
			Get
				If _LatestOrder Is Nothing AndAlso ObjectSpace IsNot Nothing Then
					_LatestOrder = Orders.OrderBy(Function(o) o.Date).FirstOrDefault()
				End If
				Return _LatestOrder
			End Get
		End Property
	End Class

	Friend Class LiveSummaryAdapter
		Private objectSpace As NonPersistentObjectSpace
		Private objectMap As Dictionary(Of Guid, LiveSummary)

		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			Me.objectSpace = npos
			Me.objectMap = New Dictionary(Of Guid, LiveSummary)()
			AddHandler objectSpace.ObjectsGetting, AddressOf ObjectSpace_ObjectsGetting
			AddHandler objectSpace.ObjectByKeyGetting, AddressOf ObjectSpace_ObjectByKeyGetting
			AddHandler objectSpace.ObjectGetting, AddressOf ObjectSpace_ObjectGetting
			AddHandler objectSpace.Reloaded, AddressOf ObjectSpace_Reloaded
		End Sub
		Private Sub ObjectSpace_Reloaded(ByVal sender As Object, ByVal e As EventArgs)
			objectMap.Clear()
		End Sub
		Private Sub ObjectSpace_ObjectsGetting(ByVal sender As Object, ByVal e As ObjectsGettingEventArgs)
			If e.ObjectType Is GetType(LiveSummary) Then
				Dim keys = LiveSummaryPresetStorage.GetAllKeys()
				e.Objects = keys.Select(Function(key) GetObjectByKey(key)).ToList()
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
				Dim data = LiveSummaryPresetStorage.GetDataByKey(key)
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
			presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {
				.Name = "Tentative",
				.Status = OrderStatus.Pending,
				.Period = 1
			})
			presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {
				.Name = "To produce",
				.Status = OrderStatus.Confirmed,
				.Period = 100
			})
			presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {
				.Name = "To deliver",
				.Status = OrderStatus.Ready,
				.Period = 100
			})
			presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {
				.Name = "Canceled this week",
				.Status = OrderStatus.Canceled,
				.Period = 7
			})
			presets.Add(Guid.NewGuid(), New LiveSummaryPreset() With {
				.Name = "Delivered this week",
				.Status = OrderStatus.Delivered,
				.Period = 7
			})
		End Sub
		Public Function GetAllKeys() As IEnumerable(Of Guid)
			Return presets.Keys
		End Function
		Public Function GetDataByKey(ByVal key As Guid) As LiveSummaryPreset
			Dim result As LiveSummaryPreset = Nothing
			If Not presets.TryGetValue(key, result) Then
				result = Nothing
			End If
			Return result
		End Function
	End Module
End Namespace
