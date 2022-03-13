using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;

namespace RBS.Auth.UnitTests.TestExtensions;

public static class ObjectValidatorExtensions
{
    public static Expression<Action<IObjectModelValidator>> GetValidatorExpression()
    {
        return o => o.Validate(It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<object>());
    }
}