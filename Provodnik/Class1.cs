

    using ReactiveValidation.Helpers;
using ReactiveValidation.Validators;
using System;

namespace ReactiveValidation.Validators
    {
        public class MyNotEmptyStringValidator<TObject> : PropertyValidator<TObject, string>
            where TObject : IValidatableObject
        {
            public MyNotEmptyStringValidator(ValidationMessageType validationMessageType)
                : base(new LanguageStringSource("Поле {PropertyName} не заполнено"), validationMessageType)
            { }


            protected override bool IsValid(ValidationContext<TObject, string> context)
            {
                return string.IsNullOrEmpty(context.PropertyValue) == false;
            }
        }
        public class MyNotEmptyStringValidatorD<TObject> : PropertyValidator<TObject, DateTime?>
            where TObject : IValidatableObject
        {
            public MyNotEmptyStringValidatorD(ValidationMessageType validationMessageType)
                : base(new LanguageStringSource("Поле {PropertyName} не заполнено"), validationMessageType)
            { }


            protected override bool IsValid(ValidationContext<TObject, DateTime?> context)
            {
            return context.PropertyValue.HasValue;
            }
        }
    }

namespace ReactiveValidation.Extensions
{
    public static class CommonValidatorsExtensions
    {
        public static TNext MyNotEmpty<TNext, TObject>(
    this IRuleBuilderInitial<TObject, string, TNext> ruleBuilder,
    ValidationMessageType validationMessageType = ValidationMessageType.SimpleError)
        where TNext : IRuleBuilder<TObject, string, TNext>
        where TObject : IValidatableObject
        {
            return ruleBuilder.SetValidator(new MyNotEmptyStringValidator<TObject>(validationMessageType));
        }
    
        public static TNext MyNotEmptyDat<TNext, TObject>(
    this IRuleBuilderInitial<TObject, DateTime?, TNext> ruleBuilder,
    ValidationMessageType validationMessageType = ValidationMessageType.SimpleError)
        where TNext : IRuleBuilder<TObject, DateTime?, TNext>
        where TObject : IValidatableObject
        {
            return ruleBuilder.SetValidator(new MyNotEmptyStringValidatorD<TObject>(validationMessageType));
        }
    }
}