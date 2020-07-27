---
page_type: sample
name: Surface Duo - Xamarin samples
languages:
- kotlin
products:
- surface-duo
- xamarin
description: "Learn to develop dual-screen apps for Surface Duo with Xamarin and Xamarin.Forms."
keywords: dual-screen, surface duo, xamarin, xamarin.forms
---
# Surface Duo - Xamarin samples

This repo contains Xamarin and Xamarin.Forms Android samples for Surface Duo.

## Get Started

To learn how to load your app on to emulator, and to use it, refer to the [documentation](https://docs.microsoft.com/dual-screen).

## Build and Test

To use the DualView and ExtendCanvas samples, you will first need to create an Google Map API key. Follow the instructions outlined [here](https://developers.google.com/maps/documentation/javascript/get-api-key) to create an API key.

After you have an API key, put it in the following files:

- DualView/Resources/Assets/googlemap.html
- ExtendCanvas/Resources/Assets/googlemapsearch.html
- XamarinForms/Xamarin.Duo.Forms.Samples.Android/Assets/googlemap.html
- XamarinForms/Xamarin.Duo.Forms.Samples.Android/Assets/googlemapsearch.html

By replacing `YOUR_API_KEY` string with your actual key the map views will be displayed.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
