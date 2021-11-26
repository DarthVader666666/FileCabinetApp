using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new List<IRecordValidator<FileCabinetEventArgs, object>>()
            {
                new FirstNameValidator(1, 60),
                new LastNameValidator(1, 60),
                new DateOfBirthValidator(1950, 2000),
                new JobExperienceValidator(1, 30),
                new MonthlyPayValidator(0, 4000),
                new GenderValidator('M', 'F'),
            })
        {
        }
    }
}
