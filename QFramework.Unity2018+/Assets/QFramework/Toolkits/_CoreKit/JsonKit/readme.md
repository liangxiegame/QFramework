# Author

 Matt Schoen <schoen@defectivestudios.com> of [Defective Studios](http://www.defectivestudios.com)


# Intro

I came across the need to send structured data to and from a server on one of my projects, and figured it would be worth my while to use JSON.  When I looked into the issue, I tried a few of the C# implementations listed on http://json.org, but found them to be too complicated to work with and expand upon.  So, I've written a very simple JSONObject class, which can be generically used to encode/decode data into a simple container.  This page assumes that you know what JSON is, and how it works.  It's rather simple, just go to json.org for a visual description of the encoding format.

# Usage

Users should not have to modify the JSONObject class themselves, and must follow the very simple procedures outlined below:

Sample data (in JSON format):
```JSON
{
    "TestObject": {
        "SomeText": "Blah",
        "SomeObject": {
            "SomeNumber": 42,
            "SomeBool": true,
            "SomeNull": null
        },
        
        "SomeEmptyObject": { },
        "SomeEmptyArray": [ ],
        "EmbeddedObject": "{\"field\":\"Value with \\\"escaped quotes\\\"\"}"
    }
}
```

The test classes provide the best examples for how the API is intended to be used.

## Features

* Decode JSON-formatted strings into a usable data structure
* Encode structured data into a JSON-formatted string
* Interoperable with `Dictionary` and `WWWForm`
* Optimized `parse`/`stringify` functions -- minimal (unavoidable) garbage creation
* Asynchronous `stringify` function for serializing lots of data without frame drops
* `MaxDepth` parsing will skip over nested data that you don't need
* Special (non-compliant) `Baked` object type can store stringified data within parsed objects
* Copy to new `JSONObject`
* Merge with another `JSONObject` (experimental)
* Random access (with `int` or `string`)
* `ToString()` returns JSON data with optional "pretty" flag to include newlines and tabs
* Switch between double and float for numeric storage depending on level of precision needed (and to ensure that numbers are parsed/stringified correctly)
* Supports `Infinity` and `NaN` values
* `JSONTemplates` static class provides serialization functions for common classes like `Vector3`, `Matrix4x4` 
* Object pool implementation (experimental)
* Handy `JSONChecker` window to test parsing on sample data

It should be pretty obvious what this parser can and cannot do.  If anyone reading this is a JSON buff (is there such a thing?) please feel free to expand and modify the parser to be more compliant.  Currently I am using the .NET `System.Convert` namespace functions for parsing the data itself.  It parses strings and numbers, which was all that I needed of it, but unless the formatting is supported by `System.Convert`, it may not incorporate all proper JSON strings.  Also, having never written a JSON parser before, I don't doubt that I could improve the efficiency or correctness of the parser.  It serves my purpose, and hopefully will help you with your project!  Let me know if you make any improvements :)

Also, you JSON buffs (really, who would admit to being a JSON buff...) might also notice from my feature list that this thing isn't exactly to specifications. Here is where it differs:
* "a string" is considered valid JSON.  There is an optional "strict" parameter to the parser which will bomb out on such input, in case that matters to you.
* The `Baked` mode is totally made up.
* The `MaxDepth` parsing is totally made up.
* `NaN` and `Infinity` were introduced in a later version of the standard, and some linters will report them as errors


## Encoding

Encoding is something of a hard-coded process.  This is because I have no idea what your data is!  It would be great if this were some sort of interface for taking an entire class and encoding it's number/string fields, but it's not.  I've come up with a few clever ways of using loops and/or recursive methods to cut down of the amount of code I have to write when I use this tool, but they're pretty project-specific.

The constructor, Add, and AddField functions now support a nested delegate structure.  This is useful if you need to create a nested JSONObject in a single line.  For example:


```C#
void DoRequest(string url, string jsonString) {
	// Web Request logic
}

void Test(string url) {
	DoRequest(url, new JSONObject(request => {
		request.AddField("sort", sort => sort.AddField("_timestamp", "desc"));
		request.AddField("query", new JSONObject(query => query.AddField("match_all", JSONObject.emptyObject)));
		request.AddField("fields", fields => fields.Add("_timestamp"));
	}).ToString());
}
```


## Decoding

Decoding is much simpler on the input end, and again, what you do with the `JSONObject` will vary on a per-project basis.  One of the more complicated way to extract the data is with a recursive function, as drafted below.  Calling the constructor with a properly formatted JSON string will return the root object (or array) containing all of its children, in one neat reference!  The data is in a public `ArrayList` called `list`, with a matching key list (called `keys`!) if the root is an `Object`.  If that's confusing, take a glance over the following code and the `print()` method in the `JSONObject` class.  If there is an error in the JSON formatting (or if there's an error with my code!) the debug console will read "improper JSON formatting".

```C#
void Test() {
	var encodedString = "{\"field1\": 0.5,\"field2\": \"sampletext\",\"field3\": [1,2,3]}";
	var jsonObject = new JSONObject(encodedString);
	AccessData(jsonObject);
}

void AccessData(JSONObject jsonObject) {
	switch (jsonObject.type) {
		case JSONObject.Type.Object:
			for (var i = 0; i < jsonObject.list.Count; i++) {
				var key = jsonObject.keys[i];
				var value = jsonObject.list[i];
				Debug.Log(key);
				AccessData(value);
			}
			break;
		case JSONObject.Type.Array:
			foreach (JSONObject element in jsonObject.list) {
				AccessData(element);
			}
			break;
		case JSONObject.Type.String:
			Debug.Log(jsonObject.stringValue);
			break;
		case JSONObject.Type.Number:
			Debug.Log(jsonObject.floatValue);
			break;
		case JSONObject.Type.Bool:
			Debug.Log(jsonObject.boolValue);
			break;
		case JSONObject.Type.Null:
			Debug.Log("Null");
			break;
		case JSONObject.Type.Baked:
			Debug.Log(jsonObject.stringValue);
			break;
	}
}
```

Decoding also supports a delegate format which will automatically check if a field exists before processing the data, providing an optional parameter for an OnFieldNotFound response.  For example:

```C#
void Test(string jsonString) {
	var list = new JSONObject(jsonString);
	list.GetField("users", users => {
		foreach (var user in users.list) {
			var thisUser = user;
			users.GetField("sessions", sessions => {
				foreach (JSONObject gameSession in sessions.list) {
					Debug.Log(gameSession);
				}
			}, name => Debug.LogWarning(string.Format("No sessions for user {0}", thisUser["name"].stringValue)));
		}
		
	});
}
```

## `(O(n))` Random access

I've added a string and int [] index to the class, so you can now retrieve data as such (from above):

```C#
void Test() {
	var jsonObject = new JSONObject("{\"field\":[0,1,2]");
	var array = jsonObject["field"];
	Debug.Log(array[2].intValue); //Should output "2"
}
```

## Change Log

### v2.1.3
* Fix exception where input is just an empty array or object
* Pool and null out list and keys field when clearing JSONObject for consistency with a fresh object;
This was causing the MaxDepthWithExcessLevels test to fail randomly when checking results because some pooled objects had list but not keys
* Update JSONChecker with information about pools and a note about not validating standard JSON

### v2.1.2
* Fix issue parsing json strings with whitespace after colon characters

### v2.1.1
* Fix issue parsing nested arrays with multiple elements
* Refactor MaxDepth tests
* Fix issues with MaxDepth

### v2.1
* Add async parsing method
* Rewrite and optimize parser
* Fix parsing errors
* Refactor VectorTemplates to use extension methods
* Add more tests

### v2.0
* Add JSONObject to Defective.JSON namespace
* Update APIs to be more descriptive
* Fix parsing errors
* Add tests

### v1.4
Big update!

* Better GC performance.  Enough of that garbage!
	* Remaining culprits are internal garbage from `StringBuilder.Append`/`AppendFormat`, `String.Substring`, `List.Add`/`GrowIfNeeded`, `Single.ToString`
* Added asynchronous `Stringify` function for serializing large amounts of data at runtime without frame drops
* Added `Baked` type
* Added `MaxDepth` to parsing function
* Various cleanup refactors recommended by ReSharper

### v1.3.2
* Added support for `NaN`
* Added strict mode to fail on purpose for improper formatting.  Right now this just means that if the parse string doesn't start with `[` or `{`, it will print a warning and return a `null` `JSONObject`.
* Changed `infinity` and `NaN` implementation to use `float` and `double` instead of `Mathf`
* Handles empty objects/arrays better
* Added a flag to print and `ToString` to turn on/off pretty print.  The define on top is now an override to system-wide disable
