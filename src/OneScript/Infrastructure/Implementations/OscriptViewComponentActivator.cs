﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Reflection;

namespace OneScript.WebHost.Infrastructure.Implementations
{
    public class OscriptViewComponentActivator : IViewComponentActivator
    {
        private readonly Dictionary<Type, ConstructorInfo> _constructorsCache;

        private class DynTypeEqualityComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return Object.ReferenceEquals(x, y);
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }


        public OscriptViewComponentActivator()
        {
            _constructorsCache = new Dictionary<Type, ConstructorInfo>(new DynTypeEqualityComparer());
        }

        public object Create(ViewComponentContext context)
        {
            var type = context.ViewComponentDescriptor.TypeInfo;
            if (_constructorsCache.TryGetValue(type.AsType()
                , out var constructor))
            {
                return constructor.Invoke(new object[0]);
            }

            var cInfo = type.GetConstructors().OfType<ReflectedConstructorInfo>().FirstOrDefault();
            if(cInfo == null)
                throw new RuntimeException($"No constructor found in type {type}");

            constructor = cInfo;
            _constructorsCache[type] = constructor;

            return constructor.Invoke(new object[0]); ;
        }

        public void Release(ViewComponentContext context, object viewComponent)
        {
            
        }
    }
}
