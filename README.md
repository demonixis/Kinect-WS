# Kinect WS

A server application that send `Kinect` data over network using the `WebSocket` protocol
Any application that support the WebSocket protocol can use it. I have developed this project to use Kinect's data with a Gear VR application.

## Server

- KinectServer: The shared project
- Kinect360Server: The server for Kinect on Xbox 360 or Windows
- KinectOneServer: The server for Kinect on Xbox One (an adapter is required)

By default, the server doesn't send orientation data. If you want to send these data, you have to define the `SEND_ROTATIONS` preprocessor in all projects.

Orientations are not sent to prevent the bandwitch and because it can be computed on the client application.

## Demos

There are some demos for Unity and WebGL (using BABYLON.js). It's not yet completed so don't hesitate to send some `pull requests`.


## Licence
MIT - Take a look at the `LICENSE` file for more informations.