IoT in .NET with a Raspberry Pi, Azure IoT Hub, and Xamarin
===
Quick start sample projects for getting started with a Raspberry Pi running a .NET Core 2 app as a service interfacing with the GPIO, and a Xamarin Forms app for controlling it.

This is code from my home security projects [blog post](https://medium.com/@dimoss/iot-in-net-with-a-raspberry-pi-azure-iot-hub-and-xamarin-3bf1cfb2514f).

## The .NET Core 2 console application, a service running on the Raspberry Pi Appliance

This sample project consists of:

* Azure IoT Hub integration
* Interfacing with a relay board
* Interfacing with NO/NC sensors and buttons
* Blinking a LED
* Measuring temperature via a DHT22/AM2303 component
* A `TimerService` ticking every second
* Sample code to poll a Ring Video Doorbell
* Sample code to get a camera snapshot of Ubiquiti Video Cameras
* Sample code to get sunsise/sunset data from OpenWeather

Please read my blog post how to run this console application as a service on a Raspberry Pi.

## The Xamarn Forms app, to interact with the Raspberry Pi Appliance

This sample project consists of:
* An iOS app (you can add an Android app if you desire)
* Azure IoT Hub integration
* Turn Relays on and off
* Geo Location services
* Estimote Beacon integration (Secure and Estimote Monitoring packets)

## Getting started
Please read my blog post, and update the code to suit your needs, including adding your Azure IoT Hub credentials to get a working copy.

Special thanks to various sources for help in building this as detailed in my blog post.