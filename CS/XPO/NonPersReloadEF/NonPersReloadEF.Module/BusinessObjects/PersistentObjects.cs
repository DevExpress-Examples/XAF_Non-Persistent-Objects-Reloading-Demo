using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.EF;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DefaultClassOptions]
    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(Order.Address))]
    public class Order : BaseObject {
        public virtual DateTime Date { get; set; }
        public virtual string Address { get; set; }
        public virtual decimal Total { get; set; }
        public virtual ObservableCollection<OrderLine> Details { get; set; } = new ObservableCollection<OrderLine>();
        public virtual OrderStatus Status { get; set; }
    }

    public enum OrderStatus {
        Active, Delayed
    }

    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(OrderLine.Product))]
    public class OrderLine : BaseObject {
        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        public virtual int Quantity { get; set; }
    }

    [DefaultClassOptions]
    public class Product : BaseObject {
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
    }

}
