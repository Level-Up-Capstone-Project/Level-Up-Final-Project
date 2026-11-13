# LevelUp Android App — Marker Installation Guide

This package contains the release version of the LevelUp Android application for assessment.

## Package contents

- `LevelUp-1.0-release.apk` — the Android installation file

APK details:

- Application name: LevelUp
- Version: 1.0
- Package ID: `za.co.levelupnow.app`
- File size: approximately 74 MiB
- SHA-256: `383BB00261CC7F1F6F96D16855D19708AF4D281C96C2C49492F707CDD61EE5E2`

## Device requirements

- An Android device or emulator running Android 7.0 or newer
- Approximately 150 MB of free storage for downloading and installing the app
- An internet connection for signing in and using connected features

The APK cannot be installed on an iPhone or iPad.

## Before the assessment

The project team must switch on the production API at `https://api.levelupnow.co.za`.

The frontend website does not need to be running because the frontend is bundled inside the APK. If the API is offline, the app will open, but registration, sign-in, uploads, saved data, and other connected features will not work.

## Installation instructions

1. Download `LevelUp-1.0-release.apk` from the project's GitHub Release.
2. Open the downloaded file from the browser notification or the device's Downloads folder.
3. If Android blocks the installation, select Settings when prompted and temporarily enable Allow from this source for the browser or file manager used to open the APK.
4. Return to the installation screen and select Install.
5. If Google Play Protect displays a warning because the app was supplied outside Google Play, confirm that the filename and SHA-256 value match those listed above. Only continue with the APK received from the project team.
6. Select Open, or launch LevelUp from the device's app list.
7. Sign in using assessment credentials supplied by the project team, or register an account if instructed to do so.

## Troubleshooting

### The app opens, but sign-in or data features fail

- Confirm that the device has an internet connection.
- Confirm with the project team that `https://api.levelupnow.co.za` is online.
- Close and reopen the app after the API has been started.

### Android reports that the app cannot be installed

- Confirm that the device runs Android 7.0 or newer.
- Make sure there is enough free storage.
- Remove any older debug installation of LevelUp and retry.
- Download the APK again in case the original download was incomplete.

### The app displays an old or unexpected session

Open the device's Settings, find Apps > LevelUp > Storage, and select Clear data, or uninstall and reinstall the APK.

## Security note

Install only `LevelUp-1.0-release.apk` downloaded from the official project Release. The private signing keystore and signing credentials are not part of the marker package and must never be shared.
