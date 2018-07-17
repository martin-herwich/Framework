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
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides screenshot extensions for the <see cref="WebTreeViewNodeControlObject"/>
  /// </summary>
  public static class WebTreeViewNodeControlObjectExtension
  {
    /// <summary>
    /// Starts the fluent screenshot API for the specified <paramref name="bocTreeViewNode"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocTreeViewNodeControlObject> ForScreenshot (
        [NotNull] this BocTreeViewNodeControlObject bocTreeViewNode)
    {
      ArgumentUtility.CheckNotNull ("bocTreeViewNode", bocTreeViewNode);

      return SelfResolvableFluentScreenshot.Create (
          new ScreenshotBocTreeViewNodeControlObject (
              bocTreeViewNode.ForControlObjectScreenshot(),
              bocTreeViewNode.Scope.ForElementScopeScreenshot()));
    }

    /// <summary>
    /// Returns the label of the <see cref="WebTreeViewNodeControlObject"/> (image and header).
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetLabel (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocTreeViewNodeControlObject> fluentBocTreeView)
    {
      var result = fluentBocTreeView.Target.BocTreeViewNode.Scope.FindCss ("span > span", Options.NoWait);
      result.EnsureExistence();

      return result.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the children of the <see cref="WebTreeViewNodeControlObject"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetChildren (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocTreeViewNodeControlObject> fluentBocTreeView)
    {
      var result = fluentBocTreeView.Target.BocTreeViewNode.Scope.FindCss ("ul", Options.NoWait);
      result.EnsureExistence();

      return result.ForElementScopeScreenshot();
    }
  }
}