﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure;
using Remotion.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure.ControlSetups;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class GenericTest : WxePage
  {
    private class TestInformationDto
    {
      public static readonly TestInformationDto Fail = new TestInformationDto (TestConstants.First, null);

      private readonly string _status;
      private readonly TestParameter[] _parameters;

      public TestInformationDto (string status, TestParameter[] parameters)
      {
        _status = status;
        _parameters = parameters;
      }

      public string Status
      {
        get { return _status; }
      }

      public TestParameter[] Parameters
      {
        get { return _parameters; }
      }
    }

    [Flags]
    public enum GenericPageTypes
    {
      All = 7,

      HiddenSection = 1,
      ShownSection = 2,
      AmbiguousSection = 4
    }

    private static readonly Dictionary<string, Func<IPageSetup>> s_pageSetupLookup =
        new Dictionary<string, Func<IPageSetup>>();

    static GenericTest ()
    {
      s_pageSetupLookup.Add ("anchor", () => new SimplePageSetup ((p, n) => new AnchorControlSetup (p, n)));
      s_pageSetupLookup.Add ("command", () => new SimplePageSetup ((p, n) => new CommandControlSetup (p, n)));
      s_pageSetupLookup.Add ("dropDownList", () => new SimplePageSetup<DropDownList>());
      s_pageSetupLookup.Add ("dropDownMenu", () => new SimplePageSetup ((p, n) => new DropDownMenuControlSetup (p, n)));
      s_pageSetupLookup.Add ("formGrid", () => new SimplePageSetup ((p, n) => new FormGridControlSetup (p, n)));
      s_pageSetupLookup.Add ("imageButton", () => new SimplePageSetup<ImageButton>());
      s_pageSetupLookup.Add ("image", () => new SimplePageSetup<Image>());
      s_pageSetupLookup.Add ("label", () => new SimplePageSetup<Label>());
      s_pageSetupLookup.Add ("listMenu", () => new SimplePageSetup<ListMenu>());
      s_pageSetupLookup.Add ("scope", () => new SimplePageSetup<Panel>());
      s_pageSetupLookup.Add ("singleView", () => new SimplePageSetup<SingleView>());
      s_pageSetupLookup.Add ("tabbedMenu", () => new SimplePageSetup<TabbedMenu>());
      s_pageSetupLookup.Add ("tabbedMultiView", () => new SimplePageSetup<TabbedMultiView>());
      s_pageSetupLookup.Add ("textBox", () => new SimplePageSetup<TextBox>());
      s_pageSetupLookup.Add ("treeView", () => new SimplePageSetup<TreeView>());
      s_pageSetupLookup.Add ("webButton", () => new SimplePageSetup ((p, n) => new WebButtonControlSetup (p, n), true, TestParameter.ItemID));
      s_pageSetupLookup.Add ("webTabStrip", () => new SimplePageSetup<WebTabStrip>());
      s_pageSetupLookup.Add ("webTreeView", () => new SimplePageSetup<WebTreeView>());
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var control = Request.Params["control"];

      Func<IPageSetup> pageSetupFactory;
      if (control == null || !s_pageSetupLookup.TryGetValue (control, out pageSetupFactory))
      {
        SetTestOutput (TestInformationDto.Fail);
        return;
      }

      int type;
      GenericPageTypes pageType;
      if (int.TryParse (Request.Params["type"], out type))
        pageType = (GenericPageTypes) type;
      else
        pageType = GenericPageTypes.All;

      var pageSetup = pageSetupFactory();

      var visibleControlPageSetup = pageSetup.CreateControlSetup (TestOptions.VisibleControl);
      if ((pageType & GenericPageTypes.ShownSection) != 0)
        visibleControlPageSetup.AddToContainer (PanelVisibleControl);

      var hiddenControlPageSetup = pageSetup.CreateControlSetup (TestOptions.HiddenControl);
      if ((pageType & GenericPageTypes.HiddenSection) != 0)
        hiddenControlPageSetup.AddToContainer (PanelHiddenControl);

      var ambiguousControlPageSetup = pageSetup.CreateControlSetup (TestOptions.AmbiguousControl);
      if ((pageType & GenericPageTypes.AmbiguousSection) != 0)
        ambiguousControlPageSetup.AddToContainer (PanelAmbiguousControl);

      SetTestOutput (new TestInformationDto(TestConstants.Ok, pageSetup.Parameters));
    }

    private void SetTestOutput ([NotNull] TestInformationDto information)
    {
      ArgumentUtility.CheckNotNull ("information", information);

      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException ("The master page does not support test information.");

      var serializer = new JavaScriptSerializer();
      master.SetTestInformation (serializer.Serialize (information));
    }
  }
}