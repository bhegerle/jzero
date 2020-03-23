# jzero

Provides a zero-allocation JSON parser and formatter. Intended for use
in servers and other environments needing predictable performance. 

## Simple Parsing Example
```C#
var json = @"{""Foo"":9}";

var rdr = new JsonReader(json);
rdr.ReadObjectStart();
rdr.NextProperty(); // == true
rdr.ReadPropertyName(); // == "Foo"
rdr.ReadInt(); // == 9
rdr.NextProperty(); // == false
rdr.ReadEof();
```

## Simple Writing Example
```C#
var buffer = new char[1000];
var wrt = new JsonWriter(buffer);
wrt.WriteObjectStart();
wrt.WritePropertyName("Foo");
wrt.Write(9);
wrt.WriteObjectEnd();

Console.WriteLine(wrt.WrittenString); // -> {"Foo":9}
```

## Model Namespace

This namespace contains observable data structures which know how to serialize themselves 
into JSON.