*Files to look at*:

* [LiveSummary.cs](./CS/NonPersistentObjectsDemo.Module/BusinessObjects/LiveSummary.cs)
* [NonPersistentObjectBase.cs](./CS/NonPersistentObjectsDemo.Module/BusinessObjects/NonPersistentObjectBase.cs)
* [Module.cs](./CS/NonPersistentObjectsDemo.Module/Module.cs)


# How to refresh Non-Persistent Objects and reload nested Persistent Objects

## Scenario

It is often required to cancel changes made to [Non\-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/116516/concepts/business-model-design/non-persistent-objects?v=20.1) in a view. When a Non-Persistent object has links to Persistent Objects, it is often required to reload these linked objects too. However, the built-in Refresh action has no effect in these scenarios by default.

## Solution

To restore a previous Non-Persistent object state, this state should be preserved separately. In this example, data for Non-Persistent Objects are stored in a static collection, and object instances are filled with these data when they are created and reloaded.

We create an object map for each [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) to keep separate Non-Persistent object instances. In the [ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting?v=20.1), [ObjectGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectGetting), and [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event handlers, non-persistent objects are looked up and added to the object map. In the [Reloaded](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.Reloaded) event handler, the object map is cleared. Therefore, subsequent object queries trigger the creation of new non-persistent object instances that will be filled with data from the storage.

The [NonPersistentObjectSpace\.AutoReloadAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoReloadAdditionalObjectSpaces?v=20.1) property is set to *true* to automatically refresh persistent object spaces added to the [NonPersistentObjectSpace\.AdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AdditionalObjectSpaces) collection. So, when non-persistent objects are re-created on refresh, they get fresh copies of nested persistent objects. 

The [NonPersistentObjectSpace\.AutoDisposeAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoDisposeAdditionalObjectSpaces?v=20.1) property is set to *true* to automatically dispose of additional object spaces when the non-persistent object space is disposed of. Disposing of unused object spaces is required to avoid memory leaks.

The non-persistent *LiveSummary* object in this example contains a collection of persistent objects and some derived properties calculated from persistent objects that match parameters. After modifying persistent objects, use the built-in [Refresh](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.SystemModule.RefreshController.RefreshAction) action to completely reload non-persistent object views.
