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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public sealed class BocReferenceDataSourceValidationFailureDisptachingValidator
      : BaseValidator, IBusinessObjectBoundEditableWebControlValidationFailureDispatcher
  {
    public BocReferenceDataSourceValidationFailureDisptachingValidator ()
    {
    }

    public IEnumerable<ValidationFailure> DispatchValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var bocControl = GetControlToValidate();
      if (bocControl == null)
      {
        throw new InvalidOperationException (
            "BocReferenceDataSourceValidationFailureDisptachingValidator may only be applied to controls of type BusinessObjectReferenceDataSourceControl");
      }

      var namingContainer = bocControl.NamingContainer;
      var validators =
          EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<IBusinessObjectBoundEditableWebControlValidationFailureDispatcher>();

      var controlsWithValidBinding = bocControl.GetBoundControlsWithValidBinding().Cast<Control>();
      var validatorsMatchingToControls = controlsWithValidBinding.Join (
          validators,
          c => c.ID,
          v => ((BaseValidator) v).ControlToValidate,
          (c, v) => v)
          .Where (c => c != this); // Prevent from finding and dispatching to himself

      return validatorsMatchingToControls.Aggregate (failures, (f, v) => v.DispatchValidationFailures (f)).ToList();
    }

    protected override bool EvaluateIsValid ()
    {
      // This validator is never invalid because it just dispatches the errors.
      return true;
    }

    private BusinessObjectReferenceDataSourceControl GetControlToValidate ()
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      return control as BusinessObjectReferenceDataSourceControl;
    }

    protected override bool ControlPropertiesValid ()
    {
      return true;
    }
  }
}