<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/255628686/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T967905)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->


# XAF - How to refresh non-persistent objects and reload nested persistent objects


It is often necessary to undo changes made to [non\-persistent objects](https://docs.devexpress.com/eXpressAppFramework/116516/concepts/business-model-design/non-persistent-objects?v=20.1) in a view. If a non-persistent object has links to persistent objects, it is often required to reload these linked objects as well. However, by default, the built-in **Refresh** action has no effect in these scenarios. This example shows how to refresh data in such cases.

<kbd>![devenv_8Nt2WqQLBv](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo/assets/14300209/0ad2e8ab-ac17-4844-a9d1-a369afe6beca)</kbd>

> [!WARNING]
> We created this example for demonstration purposes and it is not intended to address all possible usage scenarios.
> If this example does not have certain functionality or you want to change its behavior, you can extend this example. This can be a complex task that requires good knowledge of XAF: [UI Customization Categories by Skill Level](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#ui-customization-categories), and you may need to research how our components work. Refer to the following help topic for more information: [Debug DevExpress .NET Source Code with PDB Symbols](https://docs.devexpress.com/GeneralInformation/403656/support-debug-troubleshooting/debug-controls-with-debug-symbols).
> We are unable to help with such tasks as custom programming is outside our Support Service scope: [Technical Support Scope](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#support).


## Implementation Details

To restore a previous non-persistent object state, that state should be kept separately. In this example, data for non-persistent objects is stored in a static collection, and object instances are populated with this data when they are created and reloaded.

1. Create an object map for each [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) to keep non-persistent object instances separate. The [ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting?v=20.1), [ObjectGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectGetting), and [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event handlers look for non-persistent objects and add them to the object map. The [Reloaded](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.Reloaded) event handler clears the object map. Subsequent object queries trigger the creation of new non-persistent object instances that are populated with data from storage.
2. Set the [NonPersistentObjectSpace\.AutoReloadAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoReloadAdditionalObjectSpaces?v=20.1) property to `true` to automatically refresh persistent object spaces added to the [NonPersistentObjectSpace\.AdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AdditionalObjectSpaces) collection. So, when non-persistent objects are re-created on refresh, they get fresh copies of nested persistent objects. 
3. Set the [NonPersistentObjectSpace\.AutoDisposeAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoDisposeAdditionalObjectSpaces?v=20.1) property to `true` to automatically dispose of additional object spaces when the non-persistent object space is disposed of. You should dispose of unused object spaces to avoid memory leaks.
4. The non-persistent `LiveSummary` object in this example contains a collection of persistent objects and some derived properties calculated from persistent objects that match parameters. After modifying persistent objects, use the built-in [Refresh](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.SystemModule.RefreshController.RefreshAction) action to reload non-persistent object views.

## Files to Review

- [LiveSummary.cs](./CS/EFCore/NonPersReloadEF/NonPersReloadEF.Module/BusinessObjects/LiveSummary.cs)
- [Module.cs](./CS/EFCore/NonPersReloadEF/NonPersReloadEF.Module/Module.cs)


## Documentation

- [Non-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/116516/business-model-design-orm/non-persistent-objects)


## More Examples

- [How to implement CRUD operations for Non-Persistent Objects stored remotely in eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Editing-Demo)
- [How to edit Non-Persistent Objects nested in a Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo)
- [How to: Display a List of Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_how-to-display-a-list-of-non-persistent-objects-e980)
- [How to filter and sort Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Filtering-Demo)
- [How to refresh Non-Persistent Objects and reload nested Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo)
- [How to edit a collection of Persistent Objects linked to a Non-Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Edit-Linked-Persistent-Objects-Demo)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=XAF_Non-Persistent-Objects-Reloading-Demo&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=XAF_Non-Persistent-Objects-Reloading-Demo&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
