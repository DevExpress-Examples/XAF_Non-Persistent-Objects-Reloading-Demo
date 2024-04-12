﻿using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using NonPersistentObjectsDemo.Module.BusinessObjects;

namespace NonPersReload.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class NonPersReloadModule : ModuleBase {
    public NonPersReloadModule() {
		// 
		// NonPersReloadModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        application.SetupComplete += Application_SetupComplete;
        // Manage various aspects of the application UI and behavior at the module level.
    }
    private void Application_SetupComplete(object sender, EventArgs e) {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
    }
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
        var npos = e.ObjectSpace as NonPersistentObjectSpace;
        if (npos != null) {
            if(!(npos.Owner is CompositeObjectSpace)) {
                npos.PopulateAdditionalObjectSpaces((XafApplication)sender);
            }
            npos.AutoDisposeAdditionalObjectSpaces = true;
            npos.AutoRefreshAdditionalObjectSpaces = true;
            new LiveSummaryAdapter(npos);
        }
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
