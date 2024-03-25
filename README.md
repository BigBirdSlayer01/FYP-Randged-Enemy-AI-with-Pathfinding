Ranged Enemy Behavioural System with Pathfinding User Manual

Step 1:
Add the grid prefab to the scene and ensure that the transform is in the centre of where the grid needs to be. Then set the size of the grid, this should be 10x the scale of the plane e.g. if the plane is scaled 10 x 8, then the grid size should be 100 x 80. Set the node size but the de-fault is recommended. Debugging should only be used for testing

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/c94a0979-b792-47b7-b3c1-0839e33154e1)

Step 2:
Add the obstacle script to any obstacles.	 

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/406a42f9-887e-4ba6-a78b-8b46b1698a33)

Step 3a (Option 1):
Add the State Machine script to the enemy, choose the patrol type and enter point co-ordinates if not using random, select the viewing angle and viewing distance. Then add the bullet prefab and set how often the path should be updated.

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/e38b7f45-12d1-468d-8008-08964b4cd9e5)

Step 3b (Option 1)	
Add the A Star script to the enemy object and add the player game object as its target. Optionally choose preferred heuristic meth-od, though octile is recommended.	 

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/a3b34aa8-f912-4d99-a603-17c68e132224)

Step 3 (Option b):
Alternatively, add the Enemy Example prefab to the scene and customise the above examples as desired, this is an al-ready set up enemy game object and can be dragged and dropped into the scene.

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/ca8278c3-9c32-482e-8b6a-de74030e15c5)

The setup is now complete, test dif-ferent settings for what suits best to the project that is currently being built. Different heuristics will change what path the enemy may take, and how quickly the path up-dates will affect its accuracy.	 

![image](https://github.com/BigBirdSlayer01/FYP-Test-Environment/assets/118056946/2149586c-a150-419b-8842-48384ca7a760)
