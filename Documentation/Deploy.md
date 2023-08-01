
## Prepare the Deployment
1. Edit the AndroidManifest.xml under the Android project -> Properties
   a. Update the "versionCode" and/or "versionName", as appropriate.
```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android"  
	  android:versionCode="1"  
	  android:versionName="0.3.2"  
	  package="com.companyname.personaltrainerworkouts"  
	  android:installLocation="auto"  
	  android:exported="true">
```
(see below for what these values are used.)
2.  Commit and Push code
3. Open solution in Visual Studio.
4. Right click on the Android project and select Archive...
5. Archiving will be begin immediately.  Wait for it to complete
6. Once complete, click on the archive. This will display the archive details.
7. Click on the Distribute... button
8. Click Ad hoc button
9. Select the Signing Identity and click Save As
10. Save it to the folder you are deploying to (where you save this is not important).
11. It will prompt to for a password.  Enter a password.
12. Leave this open.  We will come back to it later

## Prepare the Release
1. Go to GitHub, and [Create a new Release](https://github.com/bkins/PersonalTrainerWorkouts/releases/new)
2. Create a new tag. Tag should "v" then the "versionName" updated above, followed by versionCode as a word (see which word to pick below)
   Example: v0.3.2-alpha
3. Name the release in such a way that briefly descripts the changes in this release
4. In the Description and a detailed list of the changes in this release.
5. If this release is for anything other than "prod", check the "Set as pre-release" checkbox.
6. Now go back to Visual Studio, and click on Open Folder. This will open the folder that has the deployment.
7. Open the signed-apks folder
8. Put this file in the release by dragging it to the "Attach binaries..." section.
9. Click Publish Release button

## Versioning
**versionCode**: This value must be a number.  I use this to signify the stage the app is in its life cycle:
* 1 = Alpha
* 2 = Beta
* 3 = RC
* 4 = Prod

**versionName**: This value should be formatted as such major.minor.build, each part can be any integer.
* **major**: For major changes.  Usually significant changes to  how the app works or behaves.
* **minor**: For less significant changes, but not trivial. Usually new features.
* **build**: The number will be incremented with every new release, regardless of the small the change was.  This will trigger the app to get this release for update.


> Written with [StackEdit](https://stackedit.io/).
