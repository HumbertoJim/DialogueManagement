Cómo funciona esta wea?

[ApplicationHandler]
Su objetivo es servir como un intermediario entre el controlador del sistema de diálogos (DialogueManager) y el videojuego.
Para esto, se debe crear una clase hereditaria de ApplicationHandler la cual debe ser modificada para que pueda gestionar fundamentalmente los eventos, interacciones del juegador, la cámara, etc.
El paquete cuenta con una clase llamada DefaultApplicationHandler, y hereda de la clase ApplicationHandler. Aunque DefaultApplicationHandler sirve como base, la intención de este es que el usuario lo tome solo como referencia y en su lugar, cree uno nuevo que se adapte de acuerdo a sus necesidades.

[DialogueManager]
Parece ser que esta wea controla la ejecucion de los dialogos. Este es el core, el elemento principal de todo el sistema. 

[DialogueDataManager]
Su objetivo es guardar informacion durante ejecucion como las opciones del jugador o banderas. Esto es importante en situaciones donde se desee generar, por ejemplo, diferentes rutas en los dialogos o cuando simplemente se requiera saber si el jugador hizo una accion o no.

[Module]

[Dialoguer]

[Dialogue]

[TextManager(Type="Dialogue")]