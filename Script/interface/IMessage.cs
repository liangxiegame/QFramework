using System;

public interface IMessage {
	string name { get; }
	object Body { get; set; }

	string Type { get; set; }

	string ToString();
}
