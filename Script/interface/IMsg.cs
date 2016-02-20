using System;

public interface IMsg {
	string name { get; }
	object Body { get; set; }

	string Type { get; set; }

	string ToString();
}
