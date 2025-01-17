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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NullExecutionListenerTest
  {
    private IWxeFunctionExecutionListener _executionListener;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory contextFactory = new WxeContextFactory();
      _context = contextFactory.CreateContext(new TestFunction());

      _executionListener = NullExecutionListener.Null;
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _executionListener.OnExecutionPlay(_context);
    }

    [Test]
    public void OnExecutionStop ()
    {
      _executionListener.OnExecutionStop(_context);
    }

    [Test]
    public void OnExecutionPause ()
    {
      _executionListener.OnExecutionPause(_context);
    }

    [Test]
    public void OnExecutionFail ()
    {
      _executionListener.OnExecutionFail(_context, new Exception());
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_executionListener.IsNull);
    }

    [Test]
    public void IsSerializeable ()
    {
      Assert.That(Serializer.SerializeAndDeserialize(_executionListener), Is.Not.Null);
    }
  }
}
