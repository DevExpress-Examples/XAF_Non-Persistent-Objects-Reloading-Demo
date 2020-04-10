using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    class DemoDataCreator {
        private IObjectSpace ObjectSpace { get; set; }
        public DemoDataCreator(IObjectSpace objectSpace) {
            this.ObjectSpace = objectSpace;
        }

        static string[] productNames = { "Fresh Flesh", "Soaked Souls", "Elixir of Eternity", "Bones Barbeque", "Cranium Cake" };

        public void CreateDemoObjects() {
            var rnd = new Random();
            IList<Product> products = null;
            if(ObjectSpace.GetObjectsCount(typeof(Product), null) == 0) {
                products = new List<Product>();
                for(var i = 0; i < productNames.Length; i++) {
                    var product = ObjectSpace.CreateObject<Product>();
                    product.Name = productNames[i];
                    product.Price = (80 + rnd.Next(2000)) * 0.01m;
                    products.Add(product);
                }
            }
            if(ObjectSpace.GetObjectsCount(typeof(Order), null) == 0) {
                if(products == null) {
                    products = ObjectSpace.GetObjects<Product>();
                }
                var now = DateTime.Now;
                for(var i = 0; i < 1000; i++) {
                    now = now.AddMinutes(-(1 + rnd.Next(20)));
                    var order = ObjectSpace.CreateObject<Order>();
                    order.Date = now;
                    var dnum = 1 + rnd.Next(21);
                    for(var j = 0; j < dnum; j++) {
                        var product = products[rnd.Next(products.Count)];
                        var detail = order.Details.FirstOrDefault(d => d.Product == product);
                        if(detail == null) {
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
            var delay = DateTime.Now.Subtract(now);
            if(delay.TotalMinutes > 120) {
                return (rnd.Next(30) == 0) ? OrderStatus.Canceled : OrderStatus.Delivered;
            }
            else {
                if(delay.TotalMinutes > 20) {
                    if(rnd.Next(30) == 0) {
                        return OrderStatus.Canceled;
                    }
                    else {
                        if(delay.TotalMinutes > 30) {
                            if(delay.TotalMinutes < 60 && rnd.Next(7) == 0) {
                                return OrderStatus.Confirmed;
                            }
                            else {
                                return (rnd.Next(5) == 0) ? OrderStatus.Ready : OrderStatus.Delivered;
                            }
                        }
                        else {
                            return (rnd.Next(4) == 0) ? OrderStatus.Ready : OrderStatus.Confirmed;
                        }
                    }
                }
                else {
                    return (rnd.Next(3) == 0) ? OrderStatus.Confirmed : OrderStatus.Pending;
                }
            }
        }
    }
}
