using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Services
{
    public class ModalResult
    {

        // Properties

        public Type DataType { get; }

        public bool Success { get; }

        public object Data { get; }

        // Constructors

        protected ModalResult(object data, Type resultType, bool success)
        {
            DataType = resultType;
            Success = success;
            Data = data;
        }

        // Methods

        public static ModalResult OK<T>(T result) =>
            new ModalResult(result, typeof(T), true);

        public static ModalResult OK() =>
            new ModalResult(default, typeof(object), true);

        public static ModalResult Cancel() =>
            new ModalResult(default, typeof(object), false);


    }
}
