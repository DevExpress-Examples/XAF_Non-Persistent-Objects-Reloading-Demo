Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	Friend Class DemoDataCreator
		Private Property ObjectSpace() As IObjectSpace
'INSTANT VB NOTE: The variable objectSpace was renamed since Visual Basic does not handle local variables named the same as class members well:
		Public Sub New(ByVal objectSpace_Conflict As IObjectSpace)
			Me.ObjectSpace = objectSpace_Conflict
		End Sub

		Private Shared productNames() As String = { "Fresh Flesh", "Soaked Souls", "Elixir of Eternity", "Bones Barbeque", "Cranium Cake" }

		Public Sub CreateDemoObjects()
			Dim rnd = New Random()
			Dim products As IList(Of Product) = Nothing
			If ObjectSpace.GetObjectsCount(GetType(Product), Nothing) = 0 Then
				products = New List(Of Product)()
				For i = 0 To productNames.Length - 1
					Dim product = ObjectSpace.CreateObject(Of Product)()
					product.Name = productNames(i)
					product.Price = (80 + rnd.Next(2000)) * 0.01D
					products.Add(product)
				Next i
			End If
			If ObjectSpace.GetObjectsCount(GetType(Order), Nothing) = 0 Then
				If products Is Nothing Then
					products = ObjectSpace.GetObjects(Of Product)()
				End If
				Dim now = DateTime.Now
				For i = 0 To 999
					now = now.AddMinutes(-(1 + rnd.Next(20)))
					Dim order = ObjectSpace.CreateObject(Of Order)()
					order.Date = now
					Dim dnum = 1 + rnd.Next(21)
					For j = 0 To dnum - 1
						Dim product = products(rnd.Next(products.Count))
						Dim detail = order.Details.FirstOrDefault(Function(d) d.Product Is product)
						If detail Is Nothing Then
							detail = ObjectSpace.CreateObject(Of OrderLine)()
							detail.Product = product
							order.Details.Add(detail)
						End If
						detail.Quantity += 1
					Next j
					order.Address = String.Format("{0} W {1} St", (1 + rnd.Next(15)) * 100 + 1 + rnd.Next(30), rnd.Next(100))
					order.Total = order.Details.Sum(Function(d) d.Quantity * d.Product.Price)
					order.Status = CalcStatus(now, rnd)
				Next i
			End If
			ObjectSpace.CommitChanges()
		End Sub
		Private Function CalcStatus(ByVal now As DateTime, ByVal rnd As Random) As OrderStatus
			Dim delay = DateTime.Now.Subtract(now)
			If delay.TotalMinutes > 120 Then
				Return If(rnd.Next(30) = 0, OrderStatus.Canceled, OrderStatus.Delivered)
			Else
				If delay.TotalMinutes > 20 Then
					If rnd.Next(30) = 0 Then
						Return OrderStatus.Canceled
					Else
						If delay.TotalMinutes > 30 Then
							If delay.TotalMinutes < 60 AndAlso rnd.Next(7) = 0 Then
								Return OrderStatus.Confirmed
							Else
								Return If(rnd.Next(5) = 0, OrderStatus.Ready, OrderStatus.Delivered)
							End If
						Else
							Return If(rnd.Next(4) = 0, OrderStatus.Ready, OrderStatus.Confirmed)
						End If
					End If
				Else
					Return If(rnd.Next(3) = 0, OrderStatus.Confirmed, OrderStatus.Pending)
				End If
			End If
		End Function
	End Class
End Namespace
