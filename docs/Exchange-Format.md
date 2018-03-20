# Exchange Format

The App supports a deprecated XML format and the JSON based format described in this document to exchange vplan information with a server. Requests are either triggered by the user when he forces a refresh or are happening automatically on the apps schedule.

So far, the app only relies on polling to update the content. In the future, remote notifications should be supported to push updates to the client whenever the information on the server changes. This would allow fewer requests by the app and faster update-notifications for the user.

## Caching

To make the current polling mechanism as efficient as possible the caching capabilities of HTTP are leveraged. This requires the server to provide the `Last-Modified` header field when responding to a request. This information is also used by the app to inform the user when the plan was updated the last time.

The app on the other hand sends the `If-Modified-Since` header field. If the plan didn't change since the provided date, the server should respond with the `304 Not Modified` status and shouldn't send any data. This avoids sending the same plan to the mobile twice.

## Security

The server must support HTTPS.


## Request Header

```
GET /vplan.json HTTP/1.1
Accept: application/json; charset=utf-8
Accept-Encoding: gzip,deflate
If-Modified-Since: Mon, 18 Jul 2016 02:36:04 GMT
```

## Response Header

### Up to date

```
HTTP/1.x 304 Not Modified
```

### New data available

```
HTTP/1.x 200 OK
Last-Modified: Wed, 21 Oct 2015 07:28:00 GMT
Content-Type: application/json; charset=utf-8
Content-Encoding: gzip
```

## Endpoints

### /vplan.json

```json
[
	{
		"class": {
			"name": "11/5 W/Bili",
			"school": "BG"
		},
		"day": "2018-03-19",
		"hours": [3, 4],
		"lesson": {
			"teacher": {
				"firstname": "Matthias",
				"lastname": "Schatz",
				"identifier": "SCHA"
			},
			"subject": {
				"name": "Deutsch",
				"identifier": "DEUT"
			},
			"room": "C05"
		},
		"changes": {
			"teacher": {
				"firstname": "Sonja",
				"lastname": "Max",
				"identifier": "MAX"
			},
			"subject": {
				"name": "Mathe",
				"identifier": "MAT"
			},
			"room": "C51"
		},
		"canceled": false,
		"information": [
			"Eine Information",
			"Noch eine Information"
		]
	}
]
```

### /classes.json

```json
[
	{
		"name": "11/5 W/Bili",
		"school": "BG"
	}
]
```

### /teacher.json

```json
[
	{
		"firstname": "Matthias",
		"lastname": "Schatz",
		"identifier": "SCHA"
	},
]
```
