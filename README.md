# C# openGL render project
![a](./.docs/a.jpg)

## About
I created this project for a lecture in my master course. It was not necessary to do everything from scratch, but I wanted to know exactly how to setup a render pipeline. The goal of the project was to create an application that can load a GLTF file of any content and render it in real-time.

The project has been created with C#, because I am very familiar with C# and I wanted to focus myself to learn the principles of openGL, the communication of CPU and GPU and how a render pipeline has to be setup to achieve particular graphic effects. openGL was used because it was part of the lecture and it seemed to be a good starting point to make my first steps in rendering pixels.

I used an ECS system to represent the scene which was given by the GLTF file. The choice of using an ECS system instead of an object oriented / game object approach was made, because I was very interested about the style of programming, which differs significant. I also read about the performance gains that can be archived by these system. Unfortunately, I did not make used of the use of an ECS system, because no heavy CPU behavior, like a flock simulation was implemented.

Further I abstracted openGL objects and concepts to use and create them by the need of the scene file. The abstraction includes objects like Framebuffers, Materials, Shaders, Textures and Vertexbuffers. After the base of the project was created I started out to implement a render pipeline. It started out with simple shader mapping and rendering materials with a Blinn-Phong shader. After the base line of rendering has been achieved, I added more advanced effects and techniques, like PCF shadows, Shadow-Atlas map, Postprocessing and Reflection Probes.

All in all, this project was a huge joy and at the same time devastating for my mind. But at the bottom line, there was more joy than frustration. Especially reading and recreating papers and blogposts about rendering effects like PCF-Shadows or Volumetric lighting was the most fun part for me. I got never tired of writing multiple shaders, which process their results to achieve awesome effects. This project definitely proved for me my interest in a job as graphic engineer.

## Implemented features
- CPU
    - openGL object abstraction
    - GLTF to ECS converted
    - Scene managed with ECS
    - Hierarchy between entities
- GPU
    - Shadows
        - Direct, Point, Spot
        - Single shadow map for all light sources
        - PCF
    - Dynamic reflection probs
        - Realtime or first frame creation
        - Also with shadow mapping
    - Forward rendering
    - Deferred rendering
    - Postprocessing:
        - Ambient occlusion
        - Bloom
        - Volumetric lights
        - ACES Tonemapping


## Some more pictures:
![a](./.docs/b.jpg)
![a](./.docs/c.jpg)