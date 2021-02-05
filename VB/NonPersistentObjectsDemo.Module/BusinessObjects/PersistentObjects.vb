Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DefaultClassOptions>
	<DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(Order.Address))>
	Public Class Order
		Inherits BaseObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private _Date As DateTime
		Public Property [Date]() As DateTime
			Get
				Return _Date
			End Get
			Set(ByVal value As DateTime)
				SetPropertyValue(Of DateTime)(NameOf([Date]), _Date, value)
			End Set
		End Property
		Private _Address As String
		Public Property Address() As String
			Get
				Return _Address
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(Address), _Address, value)
			End Set
		End Property
		Private _Total As Decimal
		Public Property Total() As Decimal
			Get
				Return _Total
			End Get
			Set(ByVal value As Decimal)
				SetPropertyValue(Of Decimal)(NameOf(Total), _Total, value)
			End Set
		End Property
		Private _Status As OrderStatus
		Public Property Status() As OrderStatus
			Get
				Return _Status
			End Get
			Set(ByVal value As OrderStatus)
				SetPropertyValue(Of OrderStatus)(NameOf(Status), _Status, value)
			End Set
		End Property
		<Aggregated>
		<Association>
		Public ReadOnly Property Details() As XPCollection(Of OrderLine)
			Get
				Return GetCollection(Of OrderLine)(NameOf(Details))
			End Get
		End Property
	End Class

	Public Enum OrderStatus
		Pending
		Confirmed
		Ready
		Delivered
		Canceled
	End Enum

	<DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(OrderLine.Product))>
	Public Class OrderLine
		Inherits BaseObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private _Order As Order
		<Association>
		Public Property Order() As Order
			Get
				Return _Order
			End Get
			Set(ByVal value As Order)
				SetPropertyValue(Of Order)(NameOf(Order), _Order, value)
			End Set
		End Property
		Private _Product As Product
		Public Property Product() As Product
			Get
				Return _Product
			End Get
			Set(ByVal value As Product)
				SetPropertyValue(Of Product)(NameOf(Product), _Product, value)
			End Set
		End Property
		Private _Quantity As Integer
		Public Property Quantity() As Integer
			Get
				Return _Quantity
			End Get
			Set(ByVal value As Integer)
				SetPropertyValue(Of Integer)(NameOf(Quantity), _Quantity, value)
			End Set
		End Property
	End Class

	<DefaultClassOptions>
	Public Class Product
		Inherits BaseObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(Name), _Name, value)
			End Set
		End Property
		Private _Price As Decimal
		Public Property Price() As Decimal
			Get
				Return _Price
			End Get
			Set(ByVal value As Decimal)
				SetPropertyValue(Of Decimal)(NameOf(Price), _Price, value)
			End Set
		End Property
	End Class

End Namespace
