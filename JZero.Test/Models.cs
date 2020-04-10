using System;
using JZero.Model;

namespace JZero.Test {
    public interface IModelFoo {
        string Name { get; set; }
        IArray<float> Floats { get; set; }
        IDict<bool> Flags { get; set; }
        IArray<ISubModel> SubArray { get; set; }
        IDict<ISubModel> SubDict { get; set; }
    }

    public interface ISubModel {
        int X { get; set; }
    }

    public interface IFoo {
        int X { get; set; }
        ISubModel SubModel { get; set; }
    }

    public interface IDated {
        DateTime D { get; set; }
    }
}