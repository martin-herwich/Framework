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
using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Security.BindableObject;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests
{
  [TestFixture]
  public class IBindablePropertReadAccessStrategyTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var strategy = _serviceLocator.GetInstance<IBindablePropertyReadAccessStrategy>();

      Assert.That(strategy, Is.TypeOf(typeof(CompundBindablePropertyReadAccessStrategy)));
      var compoundStrategies = ((CompundBindablePropertyReadAccessStrategy)strategy).BindablePropertyReadAccessStrategies;
      Assert.That(compoundStrategies.Select(s=>s.GetType()), Has.Member(typeof(BindableDomainObjectPropertyReadAccessStrategy)));
      Assert.That(compoundStrategies.Select(s=>s.GetType()), Has.Member(typeof(SecurityBasedBindablePropertyReadAccessStrategy)));

      var indexOfDomainObjectsStrategy =
          compoundStrategies.Select((s, i) => new { Strategy = s, Index = i })
              .Where(o => o.Strategy is BindableDomainObjectPropertyReadAccessStrategy)
              .Select(o => o.Index)
              .Single();

      var indexOfSecurityStrategy =
          compoundStrategies.Select((s, i) => new { Strategy = s, Index = i })
              .Where(o => o.Strategy is SecurityBasedBindablePropertyReadAccessStrategy)
              .Select(o => o.Index)
              .Single();

      Assert.That(indexOfDomainObjectsStrategy, Is.LessThan(indexOfSecurityStrategy));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var strategy1 = _serviceLocator.GetInstance<IBindablePropertyReadAccessStrategy>();
      var strategy2 = _serviceLocator.GetInstance<IBindablePropertyReadAccessStrategy>();

      Assert.That(strategy1, Is.SameAs(strategy2));
    }
  }
}
