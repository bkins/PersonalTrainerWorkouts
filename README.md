# PersonalTrainerWorkouts
This project has 2 purposes:
 1. Create an app for my son to use for his business
 2. Learn Xamarin and mobile development

General notes while viewing the code:

  1. I use Resharper and it customized To do list.  If you would like to see the same comment highlighting you can import the file 'PersonalTrainerWorkouts.sln.DotSettings.team' in the root folder of this solution.  I have not had someone try to import this file to ensure that it gets all my settings. So let me know if you do it.
  2. Review the Visio document: \PersonalTrainerWorkouts\Documentation\Complete Architecture.vsdx.  This describes the View -> ViewModel -> DataAcces -> Database connections.  
  3. I have not done a lot of UI development, so be kind when reviewing, especially, the Views (xaml).
  4. In general, I have purposely tried to stay out of the Android/UWP/iOS projects.  I would like to have as little custom code outside of the shared project.  Obviously, I will have (and had to with  "toast") to update these projects, but I would like to keep that to a minimum.

General TODOs:

Current state:  Most of the Viewing/Editing/Adding of Workouts and Exercises, and the relationship between them, is working.  I need to do more testing of that.  For instance, I am not 100% sure the Junction table between Workouts and Exercises are being applied correctly.  It seems to be working, but until I see it I won't feel confident.  

 1. Since this is a learning project, there has been a lot of exploration.  So I have not followed my normal rules for myself.  Most of this has been rectified, but more to do.

 2. Figure out a way to synchronize the DBs on each device.  Currently I only care about Android and UWP synchronizing.  iOS may come later, if at all.
