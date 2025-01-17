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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexing property to access a <see cref="DomainObject"/>'s transaction-dependent context for a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public struct DomainObjectTransactionContextIndexer
  {
    private readonly DomainObject _domainObject;
    private readonly bool _isInitializedEventExecuting;

    public DomainObjectTransactionContextIndexer (DomainObject domainObject, bool isInitializedEventExecuting)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      _domainObject = domainObject;
      _isInitializedEventExecuting = isInitializedEventExecuting;
    }

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public IDomainObjectTransactionContext this[ClientTransaction clientTransaction]
    {
      get
      {
        var context = new DomainObjectTransactionContext(_domainObject, clientTransaction);
        if (_isInitializedEventExecuting)
          return new InitializedEventDomainObjectTransactionContextDecorator(context);
        else
          return context;
      }
    }
  }
}
