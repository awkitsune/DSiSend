# ![App icon](https://github.com/awkitsune/DSiSend/blob/master/DSiSend/icon.ico) DSiSend

Simple .net 6.0 app to send files to DSi

![App screenshot](https://github.com/awkitsune/DSiSend/blob/master/appScreenshot1.png)

### Notes

⚠This application requires administrator priveleges to run `HttpListener` to process request from DSi

Your PC needs to be in the same network with your DSi

## Usage
0. Download compiled app from [Release page](https://github.com/awkitsune/DSiSend/releases/latest)
1. Launch the app
2. Select desired file with a corresponding button
3. Use [DSi Downloader](https://github.com/Epicpkmn11/dsidl) to scan QR code
4. Download file to your DSi✨

## Bugs
1. Empty downloading file name in [DSi Downloader](https://github.com/Epicpkmn11/dsidl)
    - __Note:__ The bug itself seems to be in a way [DSi Downloader](https://github.com/Epicpkmn11/dsidl) parsing downloadable file name. Way to solve it is to use `"Content-Disposition", $"attachment; filename={filename}"` header parsing in DSi Downloader itself, maybe I'll do a PR in the program repository 
2. __To be tested:__ Sometimes app crashes when opening new file

## TODO
1. Add workaround for UAC prompt on every startup, because it's so annoying
2. Check ECC levels on QR codes, since big ones may me recognized in inappropriate way

## Thanks
[Pk11](https://github.com/Epicpkmn11) for [DSi Downloader](https://github.com/Epicpkmn11/dsidl)
