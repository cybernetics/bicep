// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionOverloadBuilder
    {
        public FunctionOverloadBuilder(string name)
        {
            this.Name = name;
            this.Description = string.Empty;
            this.ReturnType = LanguageConstants.Any;
            this.FixedParameters = ImmutableArray.CreateBuilder<FixedFunctionParameter>();
            this.ReturnTypeBuilder = args => LanguageConstants.Any;
            this.VariableParameter = null;
        }

        protected string Name { get; }

        protected string Description { get; private set; }

        protected TypeSymbol ReturnType { get; private set; }

        protected ImmutableArray<FixedFunctionParameter>.Builder FixedParameters { get; }

        protected VariableFunctionParameter? VariableParameter { get; private set; }

        protected FunctionOverload.ReturnTypeBuilderDelegate ReturnTypeBuilder { get; private set; }

        protected FunctionFlags Flags { get; private set; }

        public virtual FunctionOverload Build() =>
            new FunctionOverload(
                this.Name,
                this.Description,
                this.ReturnTypeBuilder,
                this.ReturnType,
                this.FixedParameters.ToImmutable(),
                this.VariableParameter,
                this.Flags);

        public FunctionOverloadBuilder WithDescription(string description)
        {
            this.Description = description;

            return this;
        }

        public FunctionOverloadBuilder WithReturnType(TypeSymbol returnType)
        {
            this.ReturnType = returnType;
            this.ReturnTypeBuilder = args => returnType;

            return this;
        }

        public FunctionOverloadBuilder WithDynamicReturnType(FunctionOverload.ReturnTypeBuilderDelegate returnTypeBuilder)
        {
            this.ReturnType = returnTypeBuilder(Enumerable.Empty<FunctionArgumentSyntax>());
            this.ReturnTypeBuilder = returnTypeBuilder;

            return this;
        }

        public FunctionOverloadBuilder WithRequiredParameter(string name, TypeSymbol type, string description)
        {
            this.FixedParameters.Add(new FixedFunctionParameter(name, description, type, required: true));
            return this;
        }

        public FunctionOverloadBuilder WithOptionalParameter(string name, TypeSymbol type, string description)
        {
            this.FixedParameters.Add(new FixedFunctionParameter(name, description, type, required: false));
            return this;
        }

        public FunctionOverloadBuilder WithVariableParameter(string namePrefix, TypeSymbol type, int minimumCount, string description)
        {
            this.VariableParameter = new VariableFunctionParameter(namePrefix, description, type, minimumCount);
            return this;
        }

        public FunctionOverloadBuilder WithFlags(FunctionFlags flags)
        {
            this.Flags = flags;

            return this;
        }

        protected virtual void Validate()
        {
            // required must precede optional
            // can't have optional with varargs
        }
    }
}
