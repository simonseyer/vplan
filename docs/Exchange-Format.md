# Exchange Format

The Vplan app supports a deprecated XML format and the JSON based format described in this document to exchange vplan information with a server. Requests are either triggered by the user when he forces a refresh or are happening automatically on the apps schedule.

So far, the app only relies on polling to update the content. In the future, remote notifications should be supported to push updates to the client whenever the information on the server changes. This would allow fewer requests by the app and faster update-notifications for the user.
    
## Caching

To make the current polling mechanism as efficient as possible the caching capabilities of HTTP are leveraged. This requires the server to provide the `Last-Modified` header field when responding to a request. This information is also used by the app to inform the user when the plan was updated the last time.

The client on the other hand sends the `If-Modified-Since` header field. If the plan didn't change since the provided date, the server should respond with the `304 Not Modified` status and shouldn't send any data. This avoids sending the same plan to the client twice.
    
## Security

Requests are only supported via HTTPS.

## Models
```ruby
Change {
	school_class*	SchoolClass
	date*		string 		# the date in accordance to ISO 8601. e.g. "2018-03-19"
	hours*		[integer]	# list of hours (in the schools timetable) that if affected
	lesson*		Lesson		# information about the normally scheduled lesson
	changes*	Lesson		# changes to this original lesson (all sub-fields optional)
	canceled*	boolean		# whether this lesson got canceled completely
	information*	[string]	# list of additional lines of information
}
```

```ruby
Lesson {
	teacher		string		# teacher id (see Teacher model)
	subject		string		# subject id (see Subject model)
	room		string		# the room number (free format)
}
```

```ruby
SchoolClass {
	name*		string		# the name of on school class (group of pupil)
	school*		string		# if the school is devided into multile parts, the
					# name (or shortcode) of the division. Otherwise
					# the name (or shortcode) of the school
}
```

```ruby
Subject {
	id*		string		# the identifier of the subject, e.g. DEUT for Deutsch
	name		string		# the name of the subject, e.g. Deutsch
}
```

```ruby
Teacher {
	id*		string		# the identifier of the teacher, e.g. SCHA
	firstname	string		# first name (usually only the first letter is shown)
	lastname	string		# last name (if undefined, the identifier is shown)
}
```

`* = required`

## Request Header

```
GET /vplan
Accept: application/json; charset=utf-8
Accept-Encoding: gzip,deflate
If-Modified-Since: Mon, 18 Jul 2016 02:36:04 GMT
```

## Response Header

### Up to date

```
304 Not Modified
```

No body content.

### New data available

```
200 OK
Last-Modified: Wed, 21 Oct 2015 07:28:00 GMT
Content-Type: application/json; charset=utf-8
Content-Encoding: gzip
```

Body content depends on the endpoint below.

## Endpoints

### /vplan

Provides the latest version of the plan. Only the current and the next few days are made available.

#### Response
```
[Change]
```

#### Example

```json
[
	{
		"school_class": {
			"name": "11/5",
			"school": "BG"
		},
		"date": "2018-03-19",
		"hours": [3, 4],
		"lesson": {
			"teacher": "SCHA",
			"subject": "DEUT",
			"room": "C05"
		},
		"changes": {
			"teacher": "MAX",
			"subject": "MAT",
			"room": "C51"
		},
		"canceled": false,
		"information": [
			"Eine Information",
			"Noch eine Information"
		]
	},
	{
		"school_class": {
			"name": "1336",
			"school": "HBFS"
		},
		"date": "2018-03-20",
		"hours": [5, 6, 7],
		"lesson": {
			"teacher": "SDA",
			"subject": "DEUT",
			"room": "D4"
		},
		"changes": {
		},
		"canceled": true,
		"information": [
		]
	}
]
```

### /school

Provides a set of metadata of the school. This includes a list of all classes, teachers and subjects. The client is expected to request this data
* on a regular interval, e.g. once a week
* when a class/teacher/subject id is encountered in a vplan that is not part of the locally cached metadata

#### Response
```
{
	"classes": [SchoolClass],
	"teachers": [Teacher],
	"subjects": [Subject]
}

```

#### Example

```json
{
	"classes": [
		{
			"name": "11/5",
			"school": "BG"
		},
		{
			"name": "1336",
			"school": "HBFS"
		}
	],
	"teachers": [
		{
			"firstname": "Matthias",
			"lastname": "Schatz",
			"identifier": "SCHA"
		},
		{
			"firstname": "Sonja",
			"lastname": "Max",
			"identifier": "MAX"
		}
	],
	"subjects": [
		{
			"name": "Deutsch",
			"identifier": "DEUT"
		},
		{
			"name": "Mathe",
			"identifier": "MAT"
		}
	]	
}
```
