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
using System.Diagnostics;
using Coypu;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  public abstract class IntegrationTest
  {
    private WebTestHelper _webTestHelper;
    private IDisposable _aspNetRequestErrorDetectionScope;

    protected string SharedProjectWebRoot => _webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot + "res/Remotion.Web.Development.WebTesting.TestSite.Shared/";

    protected virtual bool MaximizeMainBrowserSession
    {
      get { return true; }
    }

    protected WebTestHelper Helper
    {
      get { return _webTestHelper; }
    }

    protected IDriver Driver
    {
      get { return _webTestHelper.MainBrowserSession?.Driver; }
    }

    [OneTimeSetUp]
    public void IntegrationTestOneTimeSetUp ()
    {
      DriverConfigurationOverride driverConfigurationOverride = null;
      if (Debugger.IsAttached || RequiresUserInterfaceAttribute.GetPropertyValue(TestContext.CurrentContext))
        driverConfigurationOverride = new DriverConfigurationOverride { Headless = false };

      _webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();
      _webTestHelper.OnFixtureSetUp(MaximizeMainBrowserSession, driverConfigurationOverride);
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp(GetType().Name + "_" + TestContext.CurrentContext.Test.Name);

      var requestErrorDetection =
          (DiagnosticInformationCollectioningRequestErrorDetectionStrategy)Helper.TestInfrastructureConfiguration.RequestErrorDetectionStrategy;

      _aspNetRequestErrorDetectionScope = requestErrorDetection.CreateAspNetRequestErrorDetectionStrategyScope();
    }

    [TearDown]
    public void IntegrationTestTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed;
      _webTestHelper.OnTearDown(hasSucceeded);
      _aspNetRequestErrorDetectionScope.Dispose();
    }

    [OneTimeTearDown]
    public void IntegrationTestTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected TPageObject Start<TPageObject> (string page)
      where TPageObject : PageObject
    {
      var isWxe = page.EndsWith(".wxe", StringComparison.OrdinalIgnoreCase);
      var url = isWxe
          ? _webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot + page
          : SharedProjectWebRoot + page;
      _webTestHelper.MainBrowserSession.Window.Visit(url);

      return _webTestHelper.CreateInitialPageObject<TPageObject>(_webTestHelper.MainBrowserSession);
    }
  }
}
