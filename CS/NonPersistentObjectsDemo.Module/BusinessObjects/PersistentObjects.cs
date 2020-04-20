using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DefaultClassOptions]
    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(Order.Address))]
    public class Order : BaseObject {
        public Order(Session session) : base(session) { }

        private DateTime _Date;
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue<DateTime>(nameof(Date), ref _Date, value); }
        }
        private string _Address;
        public string Address {
            get { return _Address; }
            set { SetPropertyValue<string>(nameof(Address), ref _Address, value); }
        }
        private decimal _Total;
        public decimal Total {
            get { return _Total; }
            set { SetPropertyValue<decimal>(nameof(Total), ref _Total, value); }
        }
        private OrderStatus _Status;
        public OrderStatus Status {
            get { return _Status; }
            set { SetPropertyValue<OrderStatus>(nameof(Status), ref _Status, value); }
        }
        [Aggregated]
        [Association]
        public XPCollection<OrderLine> Details {
            get { return GetCollection<OrderLine>(nameof(Details)); }
        }
    }

    public enum OrderStatus {
        Pending, Confirmed, Ready, Delivered, Canceled
    }

    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(OrderLine.Product))]
    public class OrderLine : BaseObject {
        public OrderLine(Session session) : base(session) { }

        private Order _Order;
        [Association]
        public Order Order {
            get { return _Order; }
            set { SetPropertyValue<Order>(nameof(Order), ref _Order, value); }
        }
        private Product _Product;
        public Product Product {
            get { return _Product; }
            set { SetPropertyValue<Product>(nameof(Product), ref _Product, value); }
        }
        private int _Quantity;
        public int Quantity {
            get { return _Quantity; }
            set { SetPropertyValue<int>(nameof(Quantity), ref _Quantity, value); }
        }
    }

    [DefaultClassOptions]
    public class Product : BaseObject {
        public Product(Session session) : base(session) { }

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private decimal _Price;
        public decimal Price {
            get { return _Price; }
            set { SetPropertyValue<decimal>(nameof(Price), ref _Price, value); }
        }
    }

}
