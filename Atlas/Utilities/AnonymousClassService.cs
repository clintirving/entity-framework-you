// // -----------------------------------------------------------------------
// // <copyright file="AnonymousClassService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Atlas.Utilities
{
    public class AnonymousClassService : IAnonymousClassService
    {
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<string, Type> _propertyNamesToAnonymousClassDictionary = new Dictionary<string, Type>();
        
        private const string SerializeToXmlMethod = "SerializeToXml";
        private const string EqualsMethod = "Equals";
        private const string GetHashCodeMethod = "GetHashCode";
        private const string OpEqualityMethod = "op_Equality";
        private const string SetAccessorMethodPrefix = "set_";
        private const string GetAccessorMethodPrefix = "get_";
        private const string DynamicAssemblyName = "DynamicAssembly";
        private const string DynamicAssemblyNamespace = "DynamicNamespace";
        private const string CustomTypeName = "CustomType";

        public Type CreateAnonymousType(List<PropertyInfo> properties)
        {
            GuardAgainstEmptyPropertiesList(properties);

            var propertyNames = string.Join(",", properties.Select(x => x.Name).ToList());

            lock (_dictionaryLock)
            {
                if (_propertyNamesToAnonymousClassDictionary.TryGetValue(propertyNames, out var existingAnonymousType))
                {
                    return existingAnonymousType;
                }

                return ConstructType(properties, propertyNames);
            }
        }

        private void GuardAgainstEmptyPropertiesList(List<PropertyInfo> properties)
        {
            if (properties.Count == 0)
            {
                throw new ApplicationException("No fields provided for anonymous type");
            }
        }

        private Type ConstructType(List<PropertyInfo> properties, string propertyNames)
        {
            var anonymousTypeBuilder = CreateTypeBuilder();

            var anonymousTypeConstructorParameters = new List<Type>();
            var anonymousTypeFieldBuilders = new List<FieldBuilder>();
            var anonymousTypePropertyBuilders = new List<PropertyBuilder>();

            foreach (var propertyInformation in properties)
            {
                anonymousTypeConstructorParameters.Add(propertyInformation.PropertyType);
                anonymousTypeFieldBuilders.Add(anonymousTypeBuilder.DefineField(propertyInformation.Name,
                    propertyInformation.PropertyType, FieldAttributes.Private));
                anonymousTypePropertyBuilders.Add(anonymousTypeBuilder.DefineProperty(propertyInformation.Name,
                    PropertyAttributes.HasDefault, propertyInformation.PropertyType, null));
            }

            SetupAnonymousTypeConstructor(anonymousTypeBuilder, anonymousTypeConstructorParameters, anonymousTypeFieldBuilders);

            SetupAnonymousTypeProperties(anonymousTypePropertyBuilders, anonymousTypeBuilder, anonymousTypeFieldBuilders);

            ImplementIEquatable(anonymousTypeBuilder);

            var anonymousType = anonymousTypeBuilder.CreateType();

            _propertyNamesToAnonymousClassDictionary.Add(propertyNames, anonymousType);

            return anonymousType;
        }
        
        private TypeBuilder CreateTypeBuilder()
        {
            var dynamicAssemblyName = new AssemblyName(DynamicAssemblyName);
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            var dynamicNamespace = dynamicAssembly.DefineDynamicModule(DynamicAssemblyNamespace);

            return dynamicNamespace.DefineType(CustomTypeName, TypeAttributes.Public);
        }

        private void SetupAnonymousTypeConstructor(TypeBuilder anonymousType, List<Type> anonymousTypeConstructorParameters,
            List<FieldBuilder> anonymousTypeFieldBuilders)
        {
            var defaultObjectConstructor = typeof(object).GetConstructor(new Type[0]);
            var anonymousTypeConstructorBuilder = anonymousType.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                anonymousTypeConstructorParameters.ToArray());

            var constructorIntermediateLanguageGenerator = anonymousTypeConstructorBuilder.GetILGenerator();
            constructorIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_0);
            constructorIntermediateLanguageGenerator.Emit(OpCodes.Call, defaultObjectConstructor);

            SetupConstructor(anonymousTypeFieldBuilders, constructorIntermediateLanguageGenerator);

            constructorIntermediateLanguageGenerator.Emit(OpCodes.Ret);

            // Add a private parameterless constructor
            var privateConstructor = anonymousType.DefineConstructor(MethodAttributes.Private, CallingConventions.Standard, new Type[] { });
            var privateConstructorIntermediateLanguageGenerator = privateConstructor.GetILGenerator();
            privateConstructorIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_0);
            privateConstructorIntermediateLanguageGenerator.Emit(OpCodes.Call, defaultObjectConstructor);
            privateConstructorIntermediateLanguageGenerator.Emit(OpCodes.Ret);
        }

        private void SetupConstructor(List<FieldBuilder> groupByTypeFieldBuilders,
            ILGenerator constructorIntermediateLanguageGenerator)
        {
            foreach (var groupByTypeFieldBuilder in groupByTypeFieldBuilders)
            {
                constructorIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_0);
                constructorIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_S, groupByTypeFieldBuilders.IndexOf(groupByTypeFieldBuilder) + 1);
                constructorIntermediateLanguageGenerator.Emit(OpCodes.Stfld, groupByTypeFieldBuilder);
            }
        }

        private void SetupAnonymousTypeProperties(List<PropertyBuilder> anonymousTypePropertyBuilders, TypeBuilder anonymousType,
            List<FieldBuilder> anonymousTypeFieldBuilders)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            foreach (var anonymousTypePropertyBuilder in anonymousTypePropertyBuilders)
            {
                var matchingFieldBuilder =
                    anonymousTypeFieldBuilders.FirstOrDefault(x => x.Name.ToLower() == anonymousTypePropertyBuilder.Name.ToLower());

                if (matchingFieldBuilder != null)
                {
                    var getMethodBuilder =
                        SetupGetMethodForProperty(anonymousType, matchingFieldBuilder, anonymousTypePropertyBuilder, methodAttributes);
                    var setMethodBuilder =
                        SetupSetMethodForProperty(anonymousType, matchingFieldBuilder, anonymousTypePropertyBuilder, methodAttributes);

                    anonymousTypePropertyBuilder.SetSetMethod(setMethodBuilder);
                    anonymousTypePropertyBuilder.SetGetMethod(getMethodBuilder);
                }
            }
        }

        private MethodBuilder SetupGetMethodForProperty(TypeBuilder anonymousType, FieldBuilder anonymousTypeFieldBuilder,
            PropertyBuilder anonymousTypePropertyBuilder, MethodAttributes methodAttributes)
        {
            var getMethodBuilder = anonymousType.DefineMethod(GetAccessorMethodPrefix + anonymousTypePropertyBuilder.Name,
                methodAttributes, anonymousTypePropertyBuilder.PropertyType, Type.EmptyTypes);

            var getMethodIntermediateLanguageGenerator = getMethodBuilder.GetILGenerator();
            getMethodIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_0);
            getMethodIntermediateLanguageGenerator.Emit(OpCodes.Ldfld, anonymousTypeFieldBuilder);
            getMethodIntermediateLanguageGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        private MethodBuilder SetupSetMethodForProperty(TypeBuilder groupByType, FieldBuilder groupByTypeFieldBuilder,
            PropertyBuilder customTypePropertyBuilder, MethodAttributes methodAttributes)
        {
            var setMethodBuilder = groupByType.DefineMethod(SetAccessorMethodPrefix + customTypePropertyBuilder.Name,
                methodAttributes, null, new[] {customTypePropertyBuilder.PropertyType});

            var setMethodIntermediateLanguageGenerator = setMethodBuilder.GetILGenerator();
            setMethodIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_0);
            setMethodIntermediateLanguageGenerator.Emit(OpCodes.Ldarg_1);
            setMethodIntermediateLanguageGenerator.Emit(OpCodes.Ldfld, groupByTypeFieldBuilder);
            setMethodIntermediateLanguageGenerator.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        private void ImplementIEquatable(TypeBuilder anonymousTypeBuilder)
        {
            ImplementEqualsOfType(anonymousTypeBuilder);

            OverrideEquals(anonymousTypeBuilder);

            OverrideGetHashCode(anonymousTypeBuilder);
        }
        
        private static void ImplementEqualsOfType(TypeBuilder anonymousTypeBuilder)
        {
            anonymousTypeBuilder.AddInterfaceImplementation(typeof(IEquatable<>).MakeGenericType(anonymousTypeBuilder));
            var equalsMethodBuilder = anonymousTypeBuilder.DefineMethod(EqualsMethod,
                MethodAttributes.Public | MethodAttributes.Virtual, typeof(bool), new Type[] {anonymousTypeBuilder});
            var ilEquals = equalsMethodBuilder.GetILGenerator();

            ilEquals.Emit(OpCodes.Ldarg_0);
            ilEquals.Emit(OpCodes.Call,
                typeof(SerializationExtensionMethods).GetMethod(SerializeToXmlMethod,
                    BindingFlags.Static | BindingFlags.Public));
            ilEquals.Emit(OpCodes.Ldarg_1);
            ilEquals.Emit(OpCodes.Call,
                typeof(SerializationExtensionMethods).GetMethod(SerializeToXmlMethod,
                    BindingFlags.Static | BindingFlags.Public));
            ilEquals.Emit(OpCodes.Call, typeof(String).GetMethod(OpEqualityMethod, new Type[] {typeof(string), typeof(string)}));
            ilEquals.Emit(OpCodes.Ret);
        }

        private static void OverrideEquals(TypeBuilder anonymousTypeBuilder)
        {
            var equalsObjectMethodBuilder = anonymousTypeBuilder.DefineMethod(EqualsMethod,
                MethodAttributes.Public | MethodAttributes.Virtual, typeof(bool), new Type[] { typeof(object) });

            var ilEqualsObject = equalsObjectMethodBuilder.GetILGenerator();

            ilEqualsObject.Emit(OpCodes.Ldarg_0);
            ilEqualsObject.Emit(OpCodes.Call,
                typeof(SerializationExtensionMethods).GetMethod(SerializeToXmlMethod,
                    BindingFlags.Static | BindingFlags.Public));
            ilEqualsObject.Emit(OpCodes.Ldarg_1);
            ilEqualsObject.Emit(OpCodes.Castclass, anonymousTypeBuilder);
            ilEqualsObject.Emit(OpCodes.Call,
                typeof(SerializationExtensionMethods).GetMethod(SerializeToXmlMethod,
                    BindingFlags.Static | BindingFlags.Public));
            ilEqualsObject.Emit(OpCodes.Call,
                typeof(String).GetMethod(OpEqualityMethod, new Type[] { typeof(string), typeof(string) }));
            ilEqualsObject.Emit(OpCodes.Ret);

            anonymousTypeBuilder.DefineMethodOverride(equalsObjectMethodBuilder,
                typeof(object).GetMethod(EqualsMethod, new Type[] {typeof(object)}));
        }

        private static void OverrideGetHashCode(TypeBuilder anonymousTypeBuilder)
        {
            var getHashCodeMethodBuilder = anonymousTypeBuilder.DefineMethod(GetHashCodeMethod,
                MethodAttributes.Public | MethodAttributes.Virtual, typeof(int), null);

            // Make the GetHashCode method simply return 0.  Comparisons should then use the Equals method instead.
            var ilGetHashCode = getHashCodeMethodBuilder.GetILGenerator();

            ilGetHashCode.Emit(OpCodes.Ldc_I4_0);
            ilGetHashCode.Emit(OpCodes.Ret);

            anonymousTypeBuilder.DefineMethodOverride(getHashCodeMethodBuilder,
                typeof(object).GetMethod(GetHashCodeMethod));
        }
    }
}