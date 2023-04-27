Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Updating
Imports NonPersistentObjectsDemo.Module.BusinessObjects

Namespace NonPersReloadEF.Module.DatabaseUpdate

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    Public Class Updater
        Inherits ModuleUpdater

        Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
            MyBase.New(objectSpace, currentDBVersion)
        End Sub

        Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
            MyBase.UpdateDatabaseAfterUpdateSchema()
            Dim rnd = New Random()
            'IList<Product> products = null;
            If ObjectSpace.GetObjectsCount(GetType(Product), Nothing) = 0 Then
                Dim products = New List(Of Product)()
                For i = 0 To 5 - 1
                    Dim product = ObjectSpace.CreateObject(Of Product)()
                    product.Name = "Product" & i
                    product.Price = i + 100
                    products.Add(product)
                Next

                For i = 0 To 5 - 1
                    Dim order = ObjectSpace.CreateObject(Of Order)()
                    Dim now = DateTime.Today.AddDays(i)
                    order.Date = now
                    For k = 0 To 2 - 1
                        Dim product = products(rnd.[Next](products.Count))
                        Dim detail = order.Details.FirstOrDefault(Function(d) d.Product Is product)
                        If detail Is Nothing Then
                            detail = ObjectSpace.CreateObject(Of OrderLine)()
                            detail.Product = product
                            order.Details.Add(detail)
                        End If

                        detail.Quantity += 1
                    Next

                    order.Address = String.Format("{0} W {1} St", (1 + rnd.[Next](15)) * 100 + 1 + rnd.[Next](30), rnd.[Next](100))
                    order.Total = order.Details.Sum(Function(d) d.Quantity * d.Product.Price)
                    order.Status = CalcStatus(now, rnd)
                Next
            End If

            ObjectSpace.CommitChanges()
        End Sub

        Private Function CalcStatus(ByVal now As DateTime, ByVal rnd As Random) As OrderStatus
            If now Is DateTime.Today Then
                Return OrderStatus.Active
            Else
                Return OrderStatus.Delayed
            End If
        End Function

        Public Overrides Sub UpdateDatabaseBeforeUpdateSchema()
            MyBase.UpdateDatabaseBeforeUpdateSchema()
        End Sub
    End Class
End Namespace
