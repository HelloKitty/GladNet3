#Libraries/Dependency Directory

This directory contains the external dependencies that aren't NuGet packages that GladNet2.0 depends on. 
A windows only batch file is provided that builds all of the provides and copies the output in the [subdirectory](https://github.com/HelloKitty/GladNet2.0/tree/master/lib/Dependency%20Builds)
GladNetV2.sln expects.

Feel free to make a pull request for Linux/Mac equivalents of the batch file.

#Dependencies

- [.NetLoggingServices](https://github.com/HelloKitty/.NetLoggingServices/tree/4ded05c877021a761cc2a6d373b06c899dedd8e6) [![Build Status](https://travis-ci.org/HelloKitty/.NetLoggingServices.svg?branch=master)](https://travis-ci.org/HelloKitty/.NetLoggingServices) is a simple .Net message logging
library aimed at providing a universal API for logging general messages to various streams. 
Some of its design intents were to reduce GC pressure and thread logging.

- [Lidgren-gen3](https://github.com/HelloKitty/lidgren-network-gen3/tree/7b6d1c2e1dfe8b851886ea7a3a62782cba4e858a) [![Build Status](https://travis-ci.org/HelloKitty/lidgren-network-gen3.svg?branch=Unity)](https://travis-ci.org/HelloKitty/lidgren-network-gen3) GladNet2.0 depends on a .Net 3.5 subset of Lidgren-gen3 which was stripped of non-Unity3D compatible code. Lidgren is a C# UDP message library built on .Net sockets. It is the core network foundation for GladNet and handles low level network communications through sockets, messages and pseudo-connection management and more. The original repo can be found [here](https://github.com/lidgren/lidgren-network-gen3) and more information on how Lidgren works can be found [here](https://code.google.com/p/lidgren-network-gen3/w/list).



*For information on additional dependencies consult documentation about the NuGet packages GladNet2.0 utilizes.
