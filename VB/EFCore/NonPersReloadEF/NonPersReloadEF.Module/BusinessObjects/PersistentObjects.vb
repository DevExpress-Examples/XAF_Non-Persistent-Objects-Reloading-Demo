Imports System.Collections.ObjectModel
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl.EF

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

    <DefaultClassOptions>
    <DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(Order.Address))>
    Public Class Order
        Inherits BaseObject

        Public Overridable Property [Date] As Date

        Public Overridable Property Address As String

        Public Overridable Property Total As Decimal

        Public Overridable Property Details As ObservableCollection(Of OrderLine) = New ObservableCollection(Of OrderLine)()

        Public Overridable Property Status As OrderStatus
    End Class

    Public Enum OrderStatus
        Active
        Delayed
    End Enum

    <DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(OrderLine.Product))>
    Public Class OrderLine
        Inherits BaseObject

        Public Overridable Property Order As Order

        Public Overridable Property Product As Product

        Public Overridable Property Quantity As Integer
    End Class

    <DefaultClassOptions>
    Public Class Product
        Inherits BaseObject

        Public Overridable Property Name As String

        Public Overridable Property Price As Decimal
    End Class
End Namespace
