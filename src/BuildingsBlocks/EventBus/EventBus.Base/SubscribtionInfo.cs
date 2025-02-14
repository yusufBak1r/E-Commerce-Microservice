using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class SubscribtionInfo
    {
        public Type type { get; set; }

        public SubscribtionInfo(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));   

        }
        public static SubscribtionInfo Typed(Type handlerType) 
        {
        return new SubscribtionInfo(handlerType);   
        }
    }
}
