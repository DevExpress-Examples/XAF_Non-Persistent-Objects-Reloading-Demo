using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.EF;
using DevExpress.Persistent.BaseImpl.EF;
using NonPersistentObjectsDemo.Module.BusinessObjects;

namespace NonPersReloadEF.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        var rnd = new Random();
        //IList<Product> products = null;
        if (ObjectSpace.GetObjectsCount(typeof(Product), null) == 0) {
            var products = new List<Product>();
            for (var i = 0; i < 5; i++) {
                var product = ObjectSpace.CreateObject<Product>();
                product.Name = "Product" + i;
                product.Price = i + 100;
                products.Add(product);
            }

            for (var i = 0; i < 5; i++) {
                var order = ObjectSpace.CreateObject<Order>();
                var now = DateTime.Today.AddDays(i);
                order.Date = now;
                for (var k = 0; k < 2; k++) {
                    var product = products[rnd.Next(products.Count)];
                    var detail = order.Details.FirstOrDefault(d => d.Product == product);
                    if (detail == null) {
                        detail = ObjectSpace.CreateObject<OrderLine>();
                        detail.Product = product;
                        order.Details.Add(detail);
                    }
                    detail.Quantity++;
                }
                order.Address = string.Format("{0} W {1} St", (1 + rnd.Next(15)) * 100 + 1 + rnd.Next(30), rnd.Next(100));
                order.Total = order.Details.Sum(d => d.Quantity * d.Product.Price);
                order.Status = CalcStatus(now, rnd);
            }
        }

        ObjectSpace.CommitChanges();
    }
    private OrderStatus CalcStatus(DateTime now, Random rnd) {
        if (now == DateTime.Today) {
            return OrderStatus.Active;
        } else {
            return OrderStatus.Delayed;
        }
    }
    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
    }
}
