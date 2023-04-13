# ARCarnival

<p align="center">
   <img src="img/arcarnival_banner.jpg" alt="AR Carnival Main Menu" style="center"/>
</p>

A simple AR game made with Unity and [Vuforia](https://developer.vuforia.com/#)! Plates spawn on a table, shoot at them with your toy gun to earn points!

This game was an experiment for my AR class where I tried to recreate an old arcade-style game with weird input peripherals. The AR gun is the weird input in this case. To achieve this, my janky experiment consisted of attaching a small marker to a ruler to use as the gun, and a big marker to a table to mark the spawn point of plates.

One of the hardest parts was encoding user input in an AR experience since you don't have easy access to buttons, specially holding a ruler-gun in one hand and your phone in the other. In this case, the inputs are shooting the gun and reloading it. For this I focused on simple gestures that can be easily checked with tracking states: You shoot by keeping the gun in the screen, and you can reload by taking it off the screen for a moment. To aim, you use the gun orientation and sight line to point at plates.

The 3D models were also made by myself with [blockbench](https://www.blockbench.net). I recently discovered this 3D modeling app and used this opportunity to learn 3D modeling with a simple program. It was a lot of fun! 

# Samples 

In this section I will show up some samples of the game, so you can see how it works!

## Full gameplay (Click to watch in YouTube):
<p align="center">
<a href="https://www.youtube.com/watch?v=gGf-r-gn8gM" target="_blank" align="center">
 <img src="http://i1.ytimg.com/vi/gGf-r-gn8gM/0.jpg" alt="Watch the gameplay video in YouTube" border="10" />
</a>
</p>

## Reloading

As I mentioned earlier, to reload your gun, you simply need to take it off the screen for a moment. However, if you leave it off the screen for too long, it won't reload. This means that you can also save ammo. Additionally, it's important to note that the color of the highlights on the gun indicates how many shots you have left.

| Reloading Example | Non-Reloading Example |
|-------------------|-----------------------|
| ![Reloading Example](https://user-images.githubusercontent.com/41093870/231807635-e27f9982-1d4d-4e04-92a8-2e823b507638.gif) | ![Non-reload example](https://user-images.githubusercontent.com/41093870/231807746-b272cf27-8ba8-4ae8-9eb4-4fd51d520292.gif) |





