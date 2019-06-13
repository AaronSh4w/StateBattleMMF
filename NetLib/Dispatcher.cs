using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetLib
{
    public class Dispatcher<T>
    {
        Dictionary<Type, Delegate> _methods = new Dictionary<Type, Delegate>();//словарь

        public Dispatcher() : this(m => m.Name == "Handle" ? m.GetParameters()[0].ParameterType : null) { }

        public Dispatcher(Func<MethodInfo, Type> selector)
        {
            RecursiveRegisterMethods(typeof(T), selector);
        }

        private void RecursiveRegisterMethods(Type t, Func<MethodInfo, Type> selector)//рекурсия реги
        {
            var baseType = t.BaseType;
            if (baseType != null)
            {
                RecursiveRegisterMethods(baseType, selector);
            }

            foreach (var item in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))//данные значения с помощью побитовой операции ИЛИ можно комбинировать вывод
            {
                if (item.GetParameters().Length == 1 && item.ReturnType == typeof(void))
                {
                    var argType = selector(item);
                    if (argType != null)
                    {
                        _methods[argType] = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(T), argType), item);
                    }
                }
            }
        }

        public bool Dispatch(T handler, object obj)
        {
            Delegate method;//ссылку на метод
            if (_methods.TryGetValue(obj.GetType(), out method))
                method.DynamicInvoke(handler, obj);

            return method != null;
        }
    }
}
