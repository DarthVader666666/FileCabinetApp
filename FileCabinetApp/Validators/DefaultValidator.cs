using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new List<IRecordValidator>()
            {
                new FirstNameValidator(2, 50),
                new LastNameValidator(2, 50),
                new DateOfBirthValidator(1960, 1990),
                new JobExperienceValidator(0, 20),
                new MonthlyPayValidator(20, 5000),
                new GenderValidator('m', 'f'),
            })
        {
        }
    }
}
