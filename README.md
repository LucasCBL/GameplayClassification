# GameplayClassification
### Lucas Christian Bodson Lobato
### alu0101111254@ull.edu.es
## Entrenar red:

Para entrenar la red neuronal es necesario ejecutar los scripts analisis.py y network.py contenidos en Classification, tras ejecutar analisis.py además de mostrarse las gráficas para analizar los datos se creará el dataset de entrenamiento datos.json, que utilizará network.py para ejecutar el modelo.

Si quisieramos usar datos personalizados debemos de ponerlos todos en la carpeta Classification / Data

Tras ejecutar network.py se creará model.png que es una imagen que contiene la representacion grafica del modelo.

## Probar red:
Para probar la red es necesario usar el script test.py, lo cual ejecutará el modelo de clasificación usando como inputs los datos metidos en Clssification/PLAYER_DATA, los datos se obtienen de la ejecucion de el juego, y hay que meter las carpteas sacadas de Carpeta_del_juego/assets/OUTPUT_DATA (tanto para ejecutable como para proyecto de unity) en Clssification/PLAYER_DATA.

Estos datos se pueden obtener de jugar al juego utilizando el ejecutable, o creando el proyecto de unity.

## Generar datos de un jugador:
Para generar estos datos simplemente es necesario jugar al juego usando el ejecutable contenido en Game Executable.
Es necesario utilizar la puerta izqueirda, ya que la demo solo permite generar datos como jugador, y la puerta derecha sirve como menu para escoger generar datos usando bots.


Tras esto simplemente es necesario jugar la partida normalmente, utilizando wasd como controles de movimiento, las flechas como controles de apuntado, la tecla q como ataque a distancia y la tecla e como ataque cuerpo a cuerpo.


Una vez completado el ultimo nivel se puede cerrar el juego con alt+f4.


Los datos generados estarán contenidos en Game Executable/assets/OUTPUT_DATA/player****

## Generar datos y bots:
Para esto es necesario crear un proyecto de unity y añadir los assets incluidos en este github o descargar el proyecto entero en https://drive.google.com/file/d/1l8n_VcYrQD_gIgMsNvO9-4qBKeAURZaZ/view?usp=sharing,
tras esto si queremos crear o modificar los bots podemos hacerlo en la carpeta assets/bots , y si queremos generar datos simplemente ejecutamso el juego desde el editor y nos metemos por la puerta derecha, tras lo cual comenzará la generación de datos. 
