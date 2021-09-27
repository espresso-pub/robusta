# Installation

Remove the following folders in your Unity project, if they present:
- Assets/External Dependency Manager
- Assets/Play Services Resolver

## Adding a package to your unity project
Add this GIT link with the package manager: https://github.com/espresso-pub/robusta.git?path=/Plugin/Robusta#v0.3.6

![alt text](https://github.com/espresso-pub/robusta/raw/master/Static/images/image3.png?raw=true)

## Applying configuration
1. Paste a link from Espresso application setup page in the wizard and check if everything is fine

![alt text](https://github.com/espresso-pub/robusta/blob/master/Static/images/image2.png?raw=true)

2. Turn on iOS Platform in File / Build Settings. Even if you build your application for Google Play only

![alt text](https://github.com/espresso-pub/robusta/blob/master/Static/images/Static/images/iOs_Support.png?raw=true)

## Checking the app
Start an application on a device or in the editor if ping works

![alt text](https://github.com/espresso-pub/robusta/blob/master/Static/images/image1.png?raw=true)

Optional - report level achieved by a player
Just add the following code at the level up

    RobustaAnalytics.SetLevel(23);
