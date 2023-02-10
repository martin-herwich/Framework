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
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class NotEmptyOrWhitespaceValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateString_WithPropertyValueIsEmptyString_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext("");
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      AssertValidationFailure(validationFailures, propertyValidatorContext);
    }

    [Test]
    public void ValidateString_WithPropertyValueIsLetter_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext("y");
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateString_WithPropertyValueHasLeadingAndTrailingWhitespace_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext("\ty  ");
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateString_WithPropertyValueIsWhitespaceString_ReturnsSingleValidationFailure ()
    {
      var whitespaceString = " \t\r\n \u00a0";
      var propertyValidatorContext = CreatePropertyValidatorContext(whitespaceString);
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      AssertValidationFailure(validationFailures, propertyValidatorContext);
    }

    [Test]
    public void ValidateArray_WithPropertyValueIsEmptyArray_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(Array.Empty<string>());
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      AssertValidationFailure(validationFailures, propertyValidatorContext);
    }

    [Test]
    public void ValidateArray_WithSingleEntryNull_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new string[] { null });
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      AssertValidationFailure(validationFailures, propertyValidatorContext);
    }

    [Test]
    public void ValidateArray_WithSingleEntryIsLetter_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new[]{"y"});
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateArray_WithSingleEntryHasLeadingAndTrailingWhitespace_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new[]{"\ty  "});
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateArray_WithMultipleEntriesMixedTextAndWhitespace_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new[]{"   ", "\ty  ", "\t\t"});
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void ValidateArray_WithMultipleEntriesOnlyWhitespace_ReturnsSingleValidationFailure ()
    {
      var whitespaceString = " \t\r\n \u00a0";
      var propertyValidatorContext = CreatePropertyValidatorContext(new[] { whitespaceString, whitespaceString, whitespaceString });
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      AssertValidationFailure(validationFailures, propertyValidatorContext);
    }

    [Test]
    public void Validate_WithObject_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    private static void AssertValidationFailure (PropertyValidationFailure[] validationFailures, PropertyValidatorContext propertyValidatorContext)
    {
      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be empty and cannot consist entirely of whitespace characters."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message."));
      Assert.That(validationFailures[0].ValidatedObject, Is.SameAs(propertyValidatorContext.Instance));
      Assert.That(validationFailures[0].ValidatedProperty, Is.SameAs(propertyValidatorContext.Property));
      Assert.That(validationFailures[0].ValidatedPropertyValue, Is.EqualTo(propertyValidatorContext.PropertyValue));
    }
  }
}