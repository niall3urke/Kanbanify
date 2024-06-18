using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Services
{
    public class ModalParameters
    {

        // Fields

        private readonly Dictionary<string, object> parameters;

        // Constructors

        public ModalParameters() =>
            parameters = new Dictionary<string, object>();

        // Methods

        public void Add(string name, object value) =>
            parameters.Add(name, value);

        public T Get<T>(string name)
        {
            if (parameters.TryGetValue(name, out object result))
            {
                return (T)result;
            }
            return (T)Activator.CreateInstance(typeof(T));
        }

        public bool TryGet<T>(string name, out T value)
        {
            if (parameters.TryGetValue(name, out object result))
            {
                value = (T)result;
                return true;
            }

            value = (T)Activator.CreateInstance(typeof(T));
            return false;
        }

    }
}
