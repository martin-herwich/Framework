// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.CodeGeneration.TypePipe.Configuration;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class RequireStrongNamingConfigurationElementTest
  {
#pragma warning disable SYSLIB0017
    private ForceStrongNamingConfigurationElement _element;
#pragma warning restore

    [SetUp]
    public void SetUp ()
    {
#pragma warning disable SYSLIB0017
      _element = new ForceStrongNamingConfigurationElement();
#pragma warning restore
    }

    [Test]
    public void KeyFilePath ()
    {
      var xmlFragment = @"<forceStrongNaming keyFilePath=""C:\key.snk""/>";
      ConfigurationHelper.DeserializeElement(_element, xmlFragment);

#pragma warning disable SYSLIB0017
      Assert.That(_element.KeyFilePath, Is.EqualTo(@"C:\key.snk"));
#pragma warning restore
    }

    [Test]
    public void KeyFilePath_Missing ()
    {
      var xmlFragment = @"<forceStrongNaming />";
      ConfigurationHelper.DeserializeElement(_element, xmlFragment);

#pragma warning disable SYSLIB0017
      Assert.That(_element.KeyFilePath, Is.Empty);
#pragma warning restore
    }
  }
}
