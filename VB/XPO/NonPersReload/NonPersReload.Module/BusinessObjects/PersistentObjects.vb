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

        Private _Date As Date

        Public Property [Date] As Date
            Get
                Return _Date
            End Get

            Set(ByVal value As Date)
                SetPropertyValue(NameOf(Order.Date), _Date, value)
            End Set
        End Property

        Private _Address As String

        Public Property Address As String
            Get
                Return _Address
            End Get

            Set(ByVal value As String)
                SetPropertyValue(Of String)(NameOf(Order.Address), _Address, value)
            End Set
        End Property

        Private _Total As Decimal

        Public Property Total As Decimal
            Get
                Return _Total
            End Get

            Set(ByVal value As Decimal)
                SetPropertyValue(NameOf(Order.Total), _Total, value)
            End Set
        End Property

        <Aggregated>
        <Association>
        Public ReadOnly Property Details As XPCollection(Of OrderLine)
            Get
                Return GetCollection(Of OrderLine)(NameOf(Order.Details))
            End Get
        End Property

        Private _Status As OrderStatus

        Public Property Status As OrderStatus
            Get
                Return _Status
            End Get

            Set(ByVal value As OrderStatus)
                SetPropertyValue(NameOf(Order.Status), _Status, value)
            End Set
        End Property
    End Class

    Public Enum OrderStatus
        Active
        Delayed
    End Enum

    <DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(OrderLine.Product))>
    Public Class OrderLine
        Inherits BaseObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub

        Private _Order As Order

        <Association>
        Public Property Order As Order
            Get
                Return _Order
            End Get

            Set(ByVal value As Order)
                SetPropertyValue(NameOf(OrderLine.Order), _Order, value)
            End Set
        End Property

        Private _Product As Product

        Public Property Product As Product
            Get
                Return _Product
            End Get

            Set(ByVal value As Product)
                SetPropertyValue(NameOf(OrderLine.Product), _Product, value)
            End Set
        End Property

        Private _Quantity As Integer

        Public Property Quantity As Integer
            Get
                Return _Quantity
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(Of Integer)(NameOf(OrderLine.Quantity), _Quantity, value)
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

        Public Property Name As String
            Get
                Return _Name
            End Get

            Set(ByVal value As String)
                SetPropertyValue(Of String)(NameOf(Product.Name), _Name, value)
            End Set
        End Property

        Private _Price As Decimal

        Public Property Price As Decimal
            Get
                Return _Price
            End Get

            Set(ByVal value As Decimal)
                SetPropertyValue(NameOf(Product.Price), _Price, value)
            End Set
        End Property
    End Class
End Namespace
