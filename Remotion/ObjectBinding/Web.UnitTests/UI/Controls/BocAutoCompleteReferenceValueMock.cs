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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Services;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  public class BocAutoCompleteReferenceValueMock : BocAutoCompleteReferenceValue
  {
    public BocAutoCompleteReferenceValueMock (IWebServiceFactory webServiceFactory)
        : base(webServiceFactory)
    {
    }

    public new void EvaluateWaiConformity ()
    {
      base.EvaluateWaiConformity();
    }

    public new object SaveControlState ()
    {
      return base.SaveControlState();
    }

    public new void LoadControlState (object state)
    {
      base.LoadControlState(state);
    }
  }
}
