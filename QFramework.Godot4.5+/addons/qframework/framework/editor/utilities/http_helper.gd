extends Node

class_name HTTPHelper

#signal on_response(result, response_code, response)

#var http: HTTPRequest

static func start_body()->PackedByteArray:
	return PackedByteArray()

static func add_field(body: PackedByteArray,boundary_key:String, field_name: String, value: String) -> void:
	var content = \
	"\r\n--%s\r\n" % boundary_key + \
	"Content-Disposition: form-data; name=\"%s\"\r\n" % field_name + \
	"Content-Type: text/plain; charset=UTF-8\r\n\r\n" + \
	value 
	body.append_array(content.to_utf8_buffer())
	
static func add_file(body: PackedByteArray,boundary_key:String, field_name: String, file: PackedByteArray, filename: String, content_type: String) -> void:
	var content = \
	"\r\n--%s\r\n" % boundary_key + \
	"Content-Disposition: form-data; name=\"%s\"; filename=\"%s\"\r\n" % [field_name, filename] + \
	"Content-Type: %s\r\n\r\n" % [content_type]
	body.append_array(content.to_utf8_buffer())
	body.append_array(file)
	
static func end_body(body: PackedByteArray,boundary_key:String):
	body.append_array(("\r\n--%s--\r\n" % boundary_key).to_utf8_buffer())
	
	
static func post(url:String,headers:PackedStringArray,form:Dictionary,on_response:Callable):
	var request = HTTPRequest.new()
	QF.get_tree().root.add_child(request)

	if form.size() > 0:	
		url += "?"
		for key in form.keys():
			url += key + "=" + form[key] + "&"
		url = url.substr(0,url.length() - 1)

	request.request_completed.connect(func(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray):
		if result == OK:
			on_response.call(body.get_string_from_utf8())
	)
	
	request.request(url,headers,HTTPClient.METHOD_POST,"{}")
		
	await request.request_completed
	request.queue_free()
