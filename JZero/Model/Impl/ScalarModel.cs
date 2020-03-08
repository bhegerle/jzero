namespace JZero.Model.Impl {
    public abstract class ScalarModel<T> : ModelBase {
        private T val;

        public T Value {
            get => val;
            set {
                val = value;
                TriggerChangedEvents(this, value != null);
            }
        }

        protected ScalarModel(T val) { this.val = val; }
    }

    public sealed class ScalarBool : ScalarModel<bool> {
        public ScalarBool() : base(default(bool)) { }
        public ScalarBool(bool value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarSByte : ScalarModel<sbyte> {
        public ScalarSByte() : base(default(sbyte)) { }
        public ScalarSByte(sbyte value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarByte : ScalarModel<byte> {
        public ScalarByte() : base(default(byte)) { }
        public ScalarByte(byte value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarUShort : ScalarModel<ushort> {
        public ScalarUShort() : base(default(ushort)) { }
        public ScalarUShort(ushort value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarShort : ScalarModel<short> {
        public ScalarShort() : base(default(short)) { }
        public ScalarShort(short value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarUInt : ScalarModel<uint> {
        public ScalarUInt() : base(default(uint)) { }
        public ScalarUInt(uint value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarInt : ScalarModel<int> {
        public ScalarInt() : base(default(int)) { }
        public ScalarInt(int value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarULong : ScalarModel<ulong> {
        public ScalarULong() : base(default(ulong)) { }
        public ScalarULong(ulong value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarLong : ScalarModel<long> {
        public ScalarLong() : base(default(long)) { }
        public ScalarLong(long value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarFloat : ScalarModel<float> {
        public ScalarFloat() : base(default(float)) { }
        public ScalarFloat(float value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarDouble : ScalarModel<double> {
        public ScalarDouble() : base(default(double)) { }
        public ScalarDouble(double value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNBool : ScalarModel<bool?> {
        public ScalarNBool() : base(default(bool?)) { }
        public ScalarNBool(bool? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNSByte : ScalarModel<sbyte?> {
        public ScalarNSByte() : base(default(sbyte?)) { }
        public ScalarNSByte(sbyte? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNByte : ScalarModel<byte?> {
        public ScalarNByte() : base(default(byte?)) { }
        public ScalarNByte(byte? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNUShort : ScalarModel<ushort?> {
        public ScalarNUShort() : base(default(ushort?)) { }
        public ScalarNUShort(ushort? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNShort : ScalarModel<short?> {
        public ScalarNShort() : base(default(short?)) { }
        public ScalarNShort(short? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNUInt : ScalarModel<uint?> {
        public ScalarNUInt() : base(default(uint?)) { }
        public ScalarNUInt(uint? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNInt : ScalarModel<int?> {
        public ScalarNInt() : base(default(int?)) { }
        public ScalarNInt(int? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNULong : ScalarModel<ulong?> {
        public ScalarNULong() : base(default(ulong?)) { }
        public ScalarNULong(ulong? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNLong : ScalarModel<long?> {
        public ScalarNLong() : base(default(long?)) { }
        public ScalarNLong(long? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNFloat : ScalarModel<float?> {
        public ScalarNFloat() : base(default(float?)) { }
        public ScalarNFloat(float? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarNDouble : ScalarModel<double?> {
        public ScalarNDouble() : base(default(double?)) { }
        public ScalarNDouble(double? value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    public sealed class ScalarString : ScalarModel<string> {
        public ScalarString() : base(default(string)) { }
        public ScalarString(string value) : base(value) { }
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }
}
