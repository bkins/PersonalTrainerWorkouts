# PersonalTrainerWorkouts
This project has 2 purposes:
 1. Create an app for my son to use for his business
 2. Learn Xamarin and mobile development

I Just start this so the code is a bit of a mess, and some serious refactoring needs to occur.
For instance:
  1. Need to implement more of a MVVM design pattern
  2. In conjunction with 1, I need to have better layering within the architecture. (The areas of concerns are a bit blured.)
      
General TODOs:

Current state:  Most of the Viewing/Editting/Adding of Workouts and Exercises, and the relationship between them, is working.  I need to do more testing of that.  For instance, I am not 100% sure the Junction table between Workouts and Exercies is being appled correctly.  It seems to be working, but until I see it I won't feel confident.  I have done most of the testing against an emulated android devise; and I have not figured out if I can view that DB directly. 
  
 1. Since this is a learning project, there has been a lot of exploration.  So I have not followed my normal rules for myself.  With that said, the unit tests have been neglected.  I need to rectify this!
 2. I need to implement a Toast-like feature, not only to inform the user of certain actions, but for debugging.
 3. Need to better understand how migrations work in Xamarin.  I need to make sure changes to the DB schema will not cause any loss data between updates.
 4. Figure out a way to synchronize the DBs on each device.  Currently I only care about Android and UWP synchronizing.  iOS may come later, if at all.
