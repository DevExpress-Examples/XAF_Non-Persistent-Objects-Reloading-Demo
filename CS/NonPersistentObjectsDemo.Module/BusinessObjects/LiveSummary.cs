using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DefaultClassOptions]
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class LiveSummary : NonPersistentObjectBase {
        private Guid id;
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public Guid ID {
            get { return id; }
        }
        public void SetKey(Guid id) {
            this.id = id;
        }
        public string Name { get; private set; }
        public void SetName(string name) {
            this.Name = name;
        }
        [Browsable(false)]
        public OrderStatus Status { get; private set; }
        public void SetStatus(OrderStatus status) {
            this.Status = status;
        }
        [Browsable(false)]
        public int Period { get; private set; }
        public void SetPeriod(int period) {
            this.Period = period;
        }
        private int? _Count;
        public int Count {
            get {
                if(!_Count.HasValue && ObjectSpace != null) {
                    var pos = ((NonPersistentObjectSpace)ObjectSpace).AdditionalObjectSpaces.FirstOrDefault(os => os.IsKnownType(typeof(Order)));
                    if(pos != null) {
                        _Count = Convert.ToInt32(pos.Evaluate(typeof(Order), CriteriaOperator.Parse("Count()"), Criteria));
                    }
                }
                return _Count.Value;
            }
        }
        private decimal? _Total;
        public decimal Total {
            get {
                if(!_Total.HasValue && ObjectSpace != null) {
                    var pos = ((NonPersistentObjectSpace)ObjectSpace).AdditionalObjectSpaces.FirstOrDefault(os => os.IsKnownType(typeof(Order)));
                    if(pos != null) {
                        _Total = Convert.ToDecimal(pos.Evaluate(typeof(Order), CriteriaOperator.Parse("Sum([Total])"), Criteria));
                    }
                }
                return _Total.Value;
            }
        }
        private CriteriaOperator Criteria {
            get {
                return CriteriaOperator.Parse("DateDiffDay([Date], Now()) <= ? And [Status] = ?", Period, Status);
            }
        }
        private IList<Order> _Orders;
        public IList<Order> Orders {
            get {
                if(_Orders == null && ObjectSpace != null) {
                    _Orders = ObjectSpace.GetObjects<Order>(Criteria).ToArray();
                }
                return _Orders;
            }
        }
        private Order _LatestOrder;
        public Order LatestOrder {
            get {
                if(_LatestOrder == null && ObjectSpace != null) {
                    _LatestOrder = Orders.OrderBy(o => o.Date).FirstOrDefault();
                }
                return _LatestOrder;
            }
        }
    }

    class LiveSummaryAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<Guid, LiveSummary> objectMap;

        public LiveSummaryAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            this.objectMap = new Dictionary<Guid, LiveSummary>();
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(LiveSummary)) {
                var keys = LiveSummaryPresetStorage.GetAllKeys();
                e.Objects = keys.Select(key => GetObjectByKey(key)).ToList();
            }
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.ObjectType == typeof(LiveSummary)) {
                e.Object = GetObjectByKey((Guid)e.Key);
            }
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            if(e.SourceObject is LiveSummary) {
                var obj = (LiveSummary)e.SourceObject;
                if(!objectMap.ContainsValue(obj)) {
                    e.TargetObject = GetObject(obj);
                }
            }
        }
        private LiveSummary GetObject(LiveSummary obj) {
            var link = obj as IObjectSpaceLink;
            if(link == null || link.ObjectSpace == null || link.ObjectSpace.IsNewObject(obj)) {
                return null;
            }
            else {
                return GetObjectByKey(obj.ID);
            }
        }
        private LiveSummary GetObjectByKey(Guid key) {
            LiveSummary obj = null;
            if(!objectMap.TryGetValue(key, out obj)) {
                var data = LiveSummaryPresetStorage.GetDataByKey(key);
                if(data != null) {
                    obj = new LiveSummary();
                    obj.SetKey((Guid)key);
                    obj.SetName(data.Name);
                    obj.SetPeriod(data.Period);
                    obj.SetStatus(data.Status);
                    objectMap.Add(key, obj);
                }
            }
            return obj;
        }
    }

    static class LiveSummaryPresetStorage {
        public class LiveSummaryPreset {
            public string Name;
            public OrderStatus Status;
            public int Period;
        }
        private static Dictionary<Guid, LiveSummaryPreset> presets;
        static LiveSummaryPresetStorage() {
            presets = new Dictionary<Guid, LiveSummaryPreset>();
            presets.Add(Guid.NewGuid(), new LiveSummaryPreset() { Name = "Tentative", Status = OrderStatus.Pending, Period = 1 });
            presets.Add(Guid.NewGuid(), new LiveSummaryPreset() { Name = "To produce", Status = OrderStatus.Confirmed, Period = 100 });
            presets.Add(Guid.NewGuid(), new LiveSummaryPreset() { Name = "To deliver", Status = OrderStatus.Ready, Period = 100 });
            presets.Add(Guid.NewGuid(), new LiveSummaryPreset() { Name = "Canceled this week", Status = OrderStatus.Canceled, Period = 7 });
            presets.Add(Guid.NewGuid(), new LiveSummaryPreset() { Name = "Delivered this week", Status = OrderStatus.Delivered, Period = 7 });
        }
        public static IEnumerable<Guid> GetAllKeys() {
            return presets.Keys;
        }
        public static LiveSummaryPreset GetDataByKey(Guid key) {
            LiveSummaryPreset result;
            if(!presets.TryGetValue(key, out result)) {
                result = null;
            }
            return result;
        }
    }
}
