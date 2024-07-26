# Photino.NET Window Location Issue

*Update*: This isn't an issue with Photino.  You need to call `SetUseOsDefaultLocation(false)` on the PhotinoWindow instance before attempting to explicitly set the location.

This application demonstrates an issue whereby the location of the Window is saved to a location .json file.

When the app starts up again, it will get the location and size information saved above, and set the current Window to use those value.

Photino.NET version: 3.0.14

