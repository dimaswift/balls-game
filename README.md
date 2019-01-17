Project summary:

I've used simple MVC pattern for separating logic, data and rendering in order to make independent simulation, 
that can be used in any project or compiled to any other language, since it doesn't reference Unity's libraries.

Model only describes how data is organized.
Contollers are classes that have some state and logic that changes that states.
View only is responsible for rendering and 'ticking' of the simulation.

The benefit of such system is that it's easily extendable and can be used in many scenarios.
It's very easy to implement such features as rewinding, saving and loading state.

Performance is also one of the biggest benefits of running custom simulations without touching pretty slow
Unity's API.

I also used pool system for MonoBehaviours, because it's really expensive to instantiate Unity objects at runtime.
In simulation, I used interfaces for handling collision events in order to make it easily customizable and performant.

Unity scene consists of SimulationView that glues everything together and creates custom events. 
Also, there's a HUD, made completely abstract from the rest of the code, it subscribes to the events and updates it's data separately.
So, if I delete the HUD - nothing breaks - as it should be!
I made SimulationView  a singleton in order completely separate HUD from it, if I delete it or change it somehow - none of the references will
be lost, because HUD is self-contained, as SimulationView is;


Btw, I made colors thing flexible - you can add as many colors as you want! Just add materials to the colors list in SimulationView.

I had a blast doing this project, I've spent around 7 hours doing it. Thank you!
