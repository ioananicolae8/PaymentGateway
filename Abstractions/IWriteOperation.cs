using System;

//namespace Abstractions
//{
//    public interface IWriteOperation<TCommand>
//    {
//        void PerformOperation(TCommand operation); // IRequest<TCommand>
//    }
//}

namespace Abstractions
{
    public interface ITest<TCommand> // IRequest<TCommand>
    {
        void PerformOperation(TCommand operation);
    }
}