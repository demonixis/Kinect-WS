# Kinect WS

A server application that send `Kinect` data over network using the `WebSocket` protocol
Any application that support the WebSocket protocol can use it. I have developed this project to use Kinect's data with a Gear VR application.

![GunSpinning VR](https://lh3.googleusercontent.com/proxy/wCb9Cn-8XlfMgcuQwReoZdwGEkkvV-XVz1t5ST56gKiKHi8C854Mq6wQ8HYXsA=w426-h264)

## Server

- KinectServer: The shared project
- Kinect360Server: The server for Kinect on Xbox 360 or Windows
- KinectOneServer: The server for Kinect on Xbox One (an adapter is required)

By default, the server doesn't send orientation data. If you want to send these data, you have to define the `SEND_ROTATIONS` preprocessor in all projects.

Orientations are not sent to prevent the bandwitch and because it can be computed on the client application.

## Demos

There are some demos for Unity and WebGL (using BABYLON.js). It's not yet completed so don't hesitate to send some `pull requests`.

![Multiplayer demo](http://67.media.tumblr.com/d7338fcd586477cf7846bdaf638a6abf/tumblr_o4ljixmDh91s15knro1_500.gif)

![Multiplayer demo](https://lh3.googleusercontent.com/-tn8d1mfYXqA/VwJQMKZ6f5I/AAAAAAAATKs/79y9wYf-tHErgUjgtMRwRCZ3kimRHy88w/w426-h256/Animation.gif)

## Licence
MIT - Take a look at the `LICENSE` file for more informations.