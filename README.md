# Bird-Flocking
A simple simulation for bird flocking behavior built with the Unity Engine.
# Setup
To run outside of the Unity Engine, download and extract the BirdSim-Built.zip file and run "Bird Sim 2.exe".
To edit within the Unity Engine, import the assets and scene from the Assets folder. Camera width adjustment may be necessary.
# Behavior
Each bird can only adjust its bearing; the speed remains constant for simplicty. The flocking behavoir is modeled through 3 basic principles: long-range attraction, short-range repulsion, and orientation alignment. These three factors can be adjusted in BirdMovement.cs by changing the values of the corresponding constants. The attraction and orientation alignment are interpolated together (with the weight of interpolation being called ORIENTATION_WEIGHT), while the repulsive force is added on to the interpolated direction.
