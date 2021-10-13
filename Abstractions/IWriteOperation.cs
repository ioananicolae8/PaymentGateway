using System;

namespace Abstractions
{
    public interface IWriteOperation<TCommand>
    {
        void PerformOperation(TCommand operation);
    }
}
