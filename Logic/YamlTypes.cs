namespace InterServer.Logic;

public class Request
{
    public int start { get; set; }
    public int end { get; set; }
    public int mb_functioncode { get; set; }
}

public class Parameter
{
    public string group { get; set; }
    public List<Item> items { get; set; }
}

public class Item
{
    public string name { get; set; }
    public string @class { get; set; }
    public string state_class { get; set; }
    public string? uom { get; set; }
    public float scale { get; set; }
    public int rule { get; set; }
    public List<string> registers { get; set; }
    public string icon { get; set; }
    public bool isstr { get; set; }
    public List<Lookup> lookup { get; set; }
    public int? offset { get; set; }
    public int? mask { get; set; }
    public Validation validation { get; set; }  // Added Validation property
}

public class Lookup
{
    public int key { get; set; }
    public string value { get; set; }
}

public class YamlRootObject
{
    public List<Request> requests { get; set; }
    public List<Parameter> parameters { get; set; }
}

public class Validation
{
    public double? min { get; set; }
    public double? max { get; set; }
    public List<string> invalidate_all { get; set; }
}