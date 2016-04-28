# DZ.Tools
[![Build status](https://ci.appveyor.com/api/projects/status/j6a9j54pkq4ix3ko?svg=true)](https://ci.appveyor.com/project/Chris24283/dz-tools)

Contains methods and data structures to simplify tagged text comparison, automatically builds confusion matrix. 
Supports html rendering, parsing from html or csv.

## How to use

### Working with string-typed tags
`StringTagsWorker` class allows you to parse general purpose HTML-like tagged corpuses using strings as types for tags.

Example:
```C#
var worker = new StringTagsWorker(
    tagsNames: new[] { "Org", "Geo", "Per", "O" }
    , undefinedTagName: "O");
var expected = worker.Parser.Parse("<Per>Jack</Per> finally visited <Org>McDonald's</Org>, then was sent to the <Geo>Hospital</Geo>.");
var actual = worker.Parser.Parse("<Per>Jack</Per> finally visited <Per>McDonald's</Per>, then was <Org>sent</Org> to the Hospital.");
var report = actual.Tags.CompareTo(expected.Tags, TagsMatchers<string>.Strict, worker.Values, worker.Undefined);
Console.WriteLine(report.Render());
```
Outputs:
```
    Act\Exp         Org         Geo         Per           O 
        Org           0           0           0           1 
        Geo           0           0           0           0 
        Per           1           0           1           0 
          O           0           1           0           1 
        All           1           1           0           1 
===================FMeasure:..............0,33333
*****************       Org:..............0,33333
*****************       Geo:..............0,00000
*****************       Per:..............0,66667
==================Precision:..............0,33333
*****************       Org:..............0,00000
*****************       Geo:..............1,00000
*****************       Per:..............0,50000
=====================Recall:..............0,33333
*****************       Org:..............0,00000
*****************       Geo:..............0,00000
*****************       Per:..............1,00000
==============Matches count:..............0000001
*****************       Org:..............0000000
*****************       Geo:..............0000000
*****************       Per:..............0000001
============Retrieved count:..............0000003
*****************       Org:..............0000001
*****************       Geo:..............0000000
*****************       Per:..............0000002
=============Relevant count:..............0000003
*****************       Org:..............0000001
*****************       Geo:..............0000001
*****************       Per:..............0000001
===========Mismatches count:..............0000003
*****************       Org:..............0000001
*****************       Geo:..............0000001
*****************       Per:..............0000000
*****************         O:..............0000001
```
Report renders metrics table and [Confusion matrix](https://www.wikiwand.com/en/Confusion_matrix).

Confusion matrix could give us information about matches and mismatches found during comparison.
To get more information about matches and mismatches it is possible to render matches and mismatches:

```c#
var text = actual.ClearedText;
Console.WriteLine(report.RenderMatchesAndMismatches(t => text.Substring(t.Begin, t.End - t.Begin)));
```
Outputs:

```
#####################################################################################################
###################################MATCHES###########################################################
#####################################################################################################
'0 - 4, <Per:1> : Jack' <<>> '0 - 4, <Per:1> : Jack'


#####################################################################################################
##################################MISMATCHES#########################################################
#####################################################################################################
Expected  <<>>  Actual
'21 - 31, <Org> : McDonald's' <<>> '21 - 31, <Per> : McDonald's'
'54 - 62, <Geo> : Hospital' <<>> 'NULL'
'NULL' <<>> '42 - 46, <Org> : sent'
```

### Working with enum-typed tags

In case we want to work with types as enums there is a generic class called `EnumsTagWorker<TType>` that contains lots of boilerplate implementaion configurable by generic parameter.

Example:

``` c#
enum Type { O, Org, Geo, Per }
class EnumWorker : EnumsTagWorker<Type>
{
    public EnumWorker() : base(
        undefinedValue: Type.O,
        valuesComparer: (t1, t2) => t1.CompareTo(t2))
    {
        Types.Add("Location", Type.Geo);
    }
}
```

Protected member `EnumsTagWorker<TType>.Types` contains mappings from `string` to `TType` so that we can parse input in more flexible way.
Example from previous section could also be implemented using enum-based approach:

```c#
var worker = new EnumWorker();
var expected = worker.Parser.Parse("<Per>Jack</Per> finally visited <Org>McDonald's</Org>, then was sent to the <Location>Hospital</Location>.");
var actual = worker.Parser.Parse("<Per>Jack</Per> finally visited <Per>McDonald's</Per>, then was <Org>sent</Org> to the Hospital.");
var report = actual.Tags.CompareTo(expected.Tags, TagsMatchers<Type>.Strict, worker.Values, worker.Undefined);
Console.WriteLine(report.Render());

var text = actual.ClearedText;
Console.WriteLine(report.RenderMatchesAndMismatches(t => text.Substring(t.Begin, t.End - t.Begin)));
```

`<Location>Hospital</Location>` will be correctly interpreted as tag with type `Type.Geo`.
Out will be the same.