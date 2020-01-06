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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class OrderPrecedenceValidationCollectorMergerTest
  {
    private ValidationCollectorMergerBase _merger;
    private IAddingComponentPropertyRule _addingPropertyRule1;
    private IRemovingComponentPropertyRule _removingPropertyRule1;
    private IComponentValidationCollector _componentValidationCollectorStub1;
    private IComponentValidationCollector _componentValidationCollectorStub2;
    private Expression<Func<Customer, string>> _firstNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private NotNullValidator _notNullValidator;
    private NotEmptyValidator _notEmptyValidator;
    private IRemovingComponentPropertyRule _removingPropertyRule2;
    private NotEqualValidator _notEqualValidator;
    private IRemovingComponentPropertyRule _removingPropertyRule3;
    private MaximumLengthValidator _maximumLengthValidator;
    private IAddingComponentPropertyRule _addingPropertyRule2;
    private LengthValidator _minimumLengthValidator;
    private IAddingComponentPropertyRule _addingPropertyRule3;
    private IComponentValidationCollector _componentValidationCollectorStub3;
    private IRemovingComponentPropertyRule _removingPropertyRule4;
    private IAddingComponentPropertyRule _addingPropertyRule4;
    private IPropertyValidatorExtractorFactory _propertyValidatorExtractorFactoryMock;
    private IPropertyValidatorExtractor _propertyValidatorExtractorMock;

    [SetUp]
    public void SetUp ()
    {
      _notEmptyValidator = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message"));
      _notNullValidator = new NotNullValidator (new InvariantValidationMessage ("Fake Message"));
      _notEqualValidator = new NotEqualValidator ("test", new InvariantValidationMessage ("Fake Message"));
      _maximumLengthValidator = new MaximumLengthValidator (30, new InvariantValidationMessage ("Fake Message"));
      _minimumLengthValidator = new MinimumLengthValidator (5, new InvariantValidationMessage ("Fake Message"));

      _componentValidationCollectorStub1 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componentValidationCollectorStub2 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componentValidationCollectorStub3 = MockRepository.GenerateStub<IComponentValidationCollector>();

      _firstNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _addingPropertyRule1 = AddingComponentPropertyRule.Create (_firstNameExpression, _componentValidationCollectorStub1.GetType());
      _addingPropertyRule1.RegisterValidator (_ => _notEmptyValidator);
      _addingPropertyRule1.RegisterValidator (_ => _notNullValidator);
      _addingPropertyRule1.RegisterValidator (_ => _notEqualValidator);

      _addingPropertyRule2 = AddingComponentPropertyRule.Create (_lastNameExpression, _componentValidationCollectorStub2.GetType());
      _addingPropertyRule2.RegisterValidator (_ => _maximumLengthValidator);

      _addingPropertyRule3 = AddingComponentPropertyRule.Create (_lastNameExpression, _componentValidationCollectorStub2.GetType());
      _addingPropertyRule3.RegisterValidator (_ => _minimumLengthValidator);

      _addingPropertyRule4 = AddingComponentPropertyRule.Create (_lastNameExpression, _componentValidationCollectorStub2.GetType());
      _addingPropertyRule4.RegisterValidator (_ => _notNullValidator);

      _removingPropertyRule1 = RemovingComponentPropertyRule.Create (_firstNameExpression, typeof (CustomerValidationCollector1));
      _removingPropertyRule1.RegisterValidator (typeof (NotEmptyValidator));

      _removingPropertyRule2 = RemovingComponentPropertyRule.Create (_firstNameExpression, typeof (CustomerValidationCollector1));
      _removingPropertyRule2.RegisterValidator (typeof (NotNullValidator), _componentValidationCollectorStub1.GetType());

      _removingPropertyRule3 = RemovingComponentPropertyRule.Create (_firstNameExpression, typeof (CustomerValidationCollector1));
      _removingPropertyRule3.RegisterValidator (typeof (NotNullValidator), typeof (string)); //Unknow collector type!

      _removingPropertyRule4 = RemovingComponentPropertyRule.Create (_lastNameExpression, typeof (CustomerValidationCollector1));
      _removingPropertyRule4.RegisterValidator (typeof (MaximumLengthValidator));

      _propertyValidatorExtractorFactoryMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractorFactory>();
      _propertyValidatorExtractorMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractor>();

      _merger = new OrderPrecedenceValidationCollectorMerger (_propertyValidatorExtractorFactoryMock);
    }

    [Test]
    public void Merge_RemoveValidatorWithNoCollectorTypeDefinied_ValidatorIsRemoved ()
    {
      _componentValidationCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule1 });
      _componentValidationCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingComponentPropertyRule[0]);

      _componentValidationCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule2, _addingPropertyRule3 });
      _componentValidationCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyRule1 });

      _propertyValidatorExtractorFactoryMock
          .Expect (
              mock =>
                  mock.Create (
                      Arg<IEnumerable<ValidatorRegistrationWithContext>>.Matches (
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorRegistration.ValidatorType == typeof (NotEmptyValidator)),
                      Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyRule1))
          .Return (new[] { _notEmptyValidator });

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub1, typeof (ApiBasedComponentValidationCollectorProvider)) },
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub2, typeof (ApiBasedComponentValidationCollectorProvider)) }
              }).CollectedRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (3));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notNullValidator, _notEqualValidator }));
      Assert.That (result[1].Validators, Is.EquivalentTo (new IPropertyValidator[] { _maximumLengthValidator }));
      Assert.That (result[2].Validators, Is.EquivalentTo (new IPropertyValidator[] { _minimumLengthValidator }));
    }

    [Test]
    public void Merge_RemoveValidatorWithAlreadyRegisteredCollectorTypeDefinied_ValidatorIsRemoved ()
    {
      _componentValidationCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule1 });
      _componentValidationCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new IAddingComponentPropertyRule[0]);

      _componentValidationCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingComponentPropertyRule[0]);
      _componentValidationCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyRule2 });

      _propertyValidatorExtractorFactoryMock
          .Expect (
              mock =>
                  mock.Create (
                      Arg<IEnumerable<ValidatorRegistrationWithContext>>.Matches (
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorRegistration.ValidatorType == typeof (NotNullValidator)),
                      Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyRule1))
          .Return (new[] { _notNullValidator });

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub1, typeof (ApiBasedComponentValidationCollectorProvider)) },
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub2, typeof (ApiBasedComponentValidationCollectorProvider)) }
              }).CollectedRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEmptyValidator, _notEqualValidator }));
    }

    [Test]
    public void Merge_Remove_AllRemoveRulesInSameGroupAreAppliedBeforeAllAddedRulesInSameGroup ()
    {
      _componentValidationCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule1, _addingPropertyRule2 });
      _componentValidationCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule3 });
      _componentValidationCollectorStub3.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule4 });

      _componentValidationCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingComponentPropertyRule[0]);
      _componentValidationCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyRule1, _removingPropertyRule4 });
      _componentValidationCollectorStub3.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyRule2 });

      _propertyValidatorExtractorFactoryMock
          .Expect (mock => mock.Create (Arg<IEnumerable<ValidatorRegistrationWithContext>>.Is.Anything, Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyRule1))
          .Return (new IPropertyValidator[] { _notEmptyValidator, _notNullValidator }).Repeat.Once();
      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyRule2))
          .Return (new[] { _maximumLengthValidator }).Repeat.Once();

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub1, typeof (ApiBasedComponentValidationCollectorProvider)) },
                  new[]
                  {
                      new ValidationCollectorInfo (_componentValidationCollectorStub2, typeof (ApiBasedComponentValidationCollectorProvider)),
                      new ValidationCollectorInfo (_componentValidationCollectorStub3, typeof (ApiBasedComponentValidationCollectorProvider))
                  }
              }).CollectedRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (4));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEqualValidator }));
      Assert.That (result[1].Validators.Any(), Is.False);
      Assert.That (result[2].Validators, Is.EquivalentTo (new IPropertyValidator[] { _minimumLengthValidator }));
      Assert.That (result[3].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notNullValidator }));
    }

    [Test]
    public void Merge_RemoveValidator_NoPropertyValidatorsToRemove_NoValidatorIsRemoved ()
    {
      _componentValidationCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyRule1 });
      _componentValidationCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new IAddingComponentPropertyRule[0]);

      _componentValidationCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingComponentPropertyRule[0]);
      _componentValidationCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyRule3 });

      _propertyValidatorExtractorFactoryMock
          .Expect (mock => mock.Create (Arg<IEnumerable<ValidatorRegistrationWithContext>>.Is.Anything, Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyRule1))
          .Return (new IPropertyValidator[0]);

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub1, typeof (ApiBasedComponentValidationCollectorProvider)) },
                  new[] { new ValidationCollectorInfo (_componentValidationCollectorStub2, typeof (ApiBasedComponentValidationCollectorProvider)) }
              }).CollectedRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEmptyValidator, _notNullValidator, _notEqualValidator }));
    }
  }
}