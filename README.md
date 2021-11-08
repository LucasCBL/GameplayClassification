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

# ENGLISH: 
## Train network:

To train the neural network it is necessary to run the scripts analisis.py and network.py contained in Classification, after running analisis.py besides showing the graphs to analyze the data the training dataset datos.json will be created, which will be used by network.py to run the model.

If we would like to use custom data we must put them all in the Classification / Data folder.

After running network.py it will create model.png which is an image containing the graphical representation of the model.

## Test network:
To test the network it is necessary to use the script test.py, which will run the classification model using as inputs the data put in Clssification/PLAYER_DATA, the data is obtained from running the game, and you have to put the folders taken from Game_folder/assets/OUTPUT_DATA (both for executable and unity project) in Clssification/PLAYER_DATA.

This data can be obtained from playing the game using the executable, or creating the unity project.

## Generate player data:
To generate this data you simply need to play the game using the executable contained in Game Executable.
It is necessary to use the left door, since the demo only allows to generate data as a player, and the right door serves as a menu to choose to generate data using bots.


After that you simply need to play the game normally, using wasd as movement controls, arrows as aiming controls, q key as ranged attack and e key as melee attack.


Once you have completed the last level you can close the game with alt+f4.


The generated data will be contained in Game Executable/assets/OUTPUT_DATA/player****.

## Generate data and bots:
For this it is necessary to create an unity project and add the assets included in this github or download the whole project at https://drive.google.com/file/d/1l8n_VcYrQD_gIgMsNvO9-4qBKeAURZaZ/view?usp=sharing,
after that if we want to create or modify the bots we can do it in the assets/bots folder, and if we want to generate data we just run the game from the editor and we get into the right door, after that the data generation will start. 

Translated with www.DeepL.com/Translator (free version)
