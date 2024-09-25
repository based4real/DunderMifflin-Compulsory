using System.Runtime.Serialization;

namespace Service;

[Serializable]
public class NotFoundException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    https://msdn.microsoft.com/en-us/library/ms229064(v=vs.100).aspx
    //

    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public static NotFoundException FromJson(dynamic json)
    {
        string text = ""; // parse from json here

        return new NotFoundException(text);
    }
}