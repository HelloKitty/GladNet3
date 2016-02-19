# GladNet2

Message based networking API library for for Unity3D/.Net developers. Defines an API from which other lowerlevel network libraries can be adapted to.

Come chat: [![https://gitter.im/HelloKitty/GladNet2.0y](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/HelloKitty/GladNet2.0?utm_source=share-link&utm_medium=link&utm_campaign=share-link)

## Implementations

Lidgren's: https://github.com/HelloKitty/GladNet2-Lidgren

Photon Server's: https://github.com/HelloKitty/GladNet.PhotonServer

## Setup

To use this project you'll first need a couple of things:
  - Visual Studio 2015
  - Git for Windows
  - Properly setup MSBuild 14 paths
  
Once you clone this reposistory you'll need to do the following before opening the .sln file:
  - Run lib/BuildDepedencies.bat it will init submodules and compile dependencies.

##Tests

#### Linux/Mono - Unit Tests
||Debug x86|Debug x64|Release x86|Release x64|
|:--:|:--:|:--:|:--:|:--:|:--:|
|**master**| N/A | N/A | N/A | [![Build Status](https://travis-ci.org/HelloKitty/GladNet2.0.svg?branch=master)](https://travis-ci.org/HelloKitty/GladNet2.0) |
|**dev**| N/A | N/A | N/A | [![Build Status](https://travis-ci.org/HelloKitty/GladNet2.0.svg?branch=dev)](https://travis-ci.org/HelloKitty/GladNet2.0)|

#### Windows - Unit Tests

(Done locally)

##Licensing

This project is licensed under the MIT license.
